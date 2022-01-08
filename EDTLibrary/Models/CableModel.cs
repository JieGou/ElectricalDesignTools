using EDTLibrary.LibraryData;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LM = EDTLibrary.ListManager;

namespace EDTLibrary.Models
{
    [AddINotifyPropertyChangedInterface]
    public class CableModel
    {
        public CableModel() {
            CableQty = 1;
            CableDerating = 1;
            ConductorQty = 3;
        }


        public CableModel(ILoadModel load)
        {
            Load = load;

            //Tag
            Source = load.FedFrom;
            Destination = load.Tag;
            CreateTag();

            UsageType = CableUsageTypes.Power.ToString();

            //TODO - get proper conductor Qty in equipment calculations
            ConductorQty = load.ConductorQty;
            CableQty = 1;

            GetCableParameters(load);
            CalculateCableQtySize();
            CalculateAmpacity();

        }

        #region Properties
        public int Id { get; set; }
        public string Tag { get; set; }

        [Browsable(false)]
        public string Category { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string CableType { get; set; }
        public string UsageType { get; set; }
        public int ConductorQty { get; set; }
        public double VoltageClass { get; set; }
        public double Insulation { get; set; }

        public int CableQty { get; set; }
        public string CableSize { get; set; }
        public double CableBaseAmps { get; set; }

        public double CableSpacing { get; set; }
        public double CableDerating { get; set; }
        public double CableDeratedAmps { get; set; }
        public double CableRequiredAmps { get; set; }
        public double CableRequiredSizingAmps { get; set; }
        //TODO - CableRequiresSizingAmps

        [Browsable(false)]
        public ILoadModel Load { get; set; }


        //TODO - code Table Rule by cable type
        string codeTable = "Table2";

        #endregion


        public void CreateTag() {
            Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
        }
        
        /// <summary>
        /// Gets the Source Eq Derating, Destination Eq FLA
        /// </summary>
        public void GetCableParametersOld() {

            //TODO cable - Move defaults to constructor
            //Defautl parameters (move to constructor)
            ConductorQty = 3;
            UsageType = CableUsageTypes.Power.ToString();
            Insulation = 100;

            //TODO = Cable - move Source and Destination data to constructor
            //gets source derating
            if (LM.dteqDict.ContainsKey(Source)) {
                CableDerating = ListManager.dteqDict[Source].CableDerating;
            }
            //get load FLA
            if (LM.iLoadDict.ContainsKey(Destination)) {
                CableRequiredAmps = ListManager.iLoadDict[Destination].Fla;
            }

            

            //DesignAmps - Load FLA, 1.25 factor, qty conductors

            //TODO - coordinate DesignAmps vs RequiredAmps
            ILoadModel load;
            load = ListManager.iLoadDict[Destination];

            if (load != null) {
                CableRequiredAmps = load.Fla;
                if (load.Category == Categories.LOAD1P.ToString()) {
                    //TODO - algorithm to determine conductor count for 1Phase loads
                }
                if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                    CableRequiredSizingAmps *= 1.25;
                }
                if (load.Category == Categories.LOAD3P.ToString()) {
                    ConductorQty = 3;
                    UsageType = "Power";
                }
            }

            CableRequiredSizingAmps = CableRequiredAmps / CableDerating;
            CableRequiredSizingAmps = Math.Round(CableRequiredSizingAmps, 1);


            //Cable Type - selects the cabletype based on VoltageClass, Conuctors, Insulation and UsageType
            DataTable cableType = LibraryTables.CableTypes.Select($"VoltageClass >= {load.Voltage}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                                         $"AND Conductors ={ConductorQty}" +
                                         $"AND Insulation ={Insulation}" +
                                         $"AND UsageType = '{UsageType}'").CopyToDataTable();

            CableType = cableType.Rows[0]["Type"].ToString();

            VoltageClass = Int32.Parse(cableType.Rows[0]["VoltageClass"].ToString());
        }

        /// <summary>
        /// Gets the Source Eq Derating, Destination Eq FLA
        /// </summary>
        public void GetCableParameters(ILoadModel load)
        {

            CableDerating = load.CableDerating;
            //gets source derating

            if (load.GetType() == typeof(DteqModel)) {
                //CableDerating = 1;
            }

            else if (load.GetType() == typeof(LoadModel)) {

                if (LM.dteqDict.ContainsKey(Source)) {
                    CableDerating = ListManager.dteqDict[Source].CableDerating;
                }

            }

            CableRequiredAmps = load.Fla;

            if (load.Category == Categories.LOAD3P.ToString()) {
                ConductorQty = 3;
            }
            else if (load.Category == Categories.LOAD1P.ToString()) {
                //TODO - algorithm to determine conductor count for 1Phase loads
            }


            if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                CableRequiredAmps *= 1.25;
            }
            CableRequiredAmps = Math.Max(load.PdSizeTrip, CableRequiredAmps);

            CableRequiredSizingAmps = CableRequiredAmps / CableDerating;
            CableRequiredSizingAmps = Math.Round(CableRequiredSizingAmps, 1);


            //Voltage Class based on load

            VoltageClass = LibraryManager.GetCableVoltageClass(load.Voltage);

            //Insulation Based on settings
            if (VoltageClass == 1000) {
                Insulation = int.Parse(EdtSettings.CableInsulation1kVPower.ToString());
            }
            else if (VoltageClass == 5000) {
                Insulation = int.Parse(EdtSettings.CableInsulation5kVPower.ToString());
            }
            else if (VoltageClass == 15000) {
                Insulation = int.Parse(EdtSettings.CableInsulation15kVPower.ToString());
            }
            else if (VoltageClass == 35000) {
                Insulation = int.Parse(EdtSettings.CableInsulation35kVPower.ToString());
            }




            //Cable Type - selects the cabletype based on VoltageClass, ConuctorQty, Insulation and UsageType
            //TODO - get these parameters from settings selected cable type
            DataTable cableType = LibraryTables.CableTypes.Select($"VoltageClass >= {VoltageClass}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                                         $"AND Conductors ={ConductorQty}" +
                                         $"AND Insulation ={Insulation}" +
                                         $"AND UsageType = '{UsageType}'").CopyToDataTable();

            CableType = cableType.Rows[0]["Type"].ToString();

        }





        // TODO - Move "CalculatePowerCableSize to ILoadModel

        /// <summary>
        /// Recursive function that gets the cable qty and size from Ampacity Table based on cable type and required amps
        /// </summary>
        public void CalculateCableQtySize() {

            //TODO - determine which AmpacityTable to select from
            // 1
            DataTable cableAmps = EdtSettings.CableAmpsUsedInProject.Copy();

            // 2
            //filter cables larger than RequiredAmps
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<double>("Amps75") >= CableRequiredSizingAmps
                                                          && x.Field<string>("CodeTable") == codeTable); // ex: Table2 (from CEC)
            // 3
            GetCableQty(CableQty);
            //EdtSettings.InitializeSettings(); //Ensures the cable ampacity table for the project is created


            //Recursive method
            void GetCableQty(int cableQty) {
                if (cableQty < 20) {
                    if (cables.Any()) {
                        cableAmps = null;
                        cableAmps = cables.CopyToDataTable();

                        //select smallest of 
                        cableAmps = cableAmps.Select($"Amps75 = MIN(Amps75)").CopyToDataTable();

                        CableBaseAmps = Double.Parse(cableAmps.Rows[0]["Amps75"].ToString());
                        CableBaseAmps = Math.Round(CableBaseAmps, 2);
                        CableDeratedAmps = CableBaseAmps * CableDerating;
                        CableDeratedAmps = Math.Round(CableDeratedAmps, 2);
                        CableQty = cableQty;
                        CableSize = cableAmps.Rows[0]["Size"].ToString();
                    }
                    else {
                        CableQty += 1;
                        cableAmps = EdtSettings.CableAmpsUsedInProject.Copy();
                        foreach (DataRow row in cableAmps.Rows) {
                            double amps75 = (double)row["Amps75"];
                            string size = row["Size"].ToString();
                            amps75 *= CableQty;
                            row["Amps75"] = amps75;
                            cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EDTLibrary.ProjectSettings.EdtSettings.Code
                                                              && x.Field<double>("Amps75") >= CableRequiredSizingAmps
                                                              && x.Field<string>("CodeTable") == codeTable);
                        }
                        GetCableQty(CableQty);
                    }
                }
            }
        }

        public void CalculateAmpacity()
        {
            //TODO - if cables are already created and calbe size is removed it causes an error
            //GetCableParametersOld();

            //created cable ampacity table
            DataTable cableAmps = EdtSettings.CableAmpsUsedInProject.Copy();

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EDTLibrary.ProjectSettings.EdtSettings.Code
                                                          && x.Field<string>("CodeTable") == codeTable
                                                          && x.Field<string>("Size") == CableSize);

            cableAmps = null;
            try {
                cableAmps = cables.CopyToDataTable();
                //select smallest of the valid results
                CableBaseAmps = Double.Parse(cableAmps.Rows[0]["Amps75"].ToString()) * CableQty;
                CableDeratedAmps = CableBaseAmps * CableDerating;
                CableDeratedAmps = Math.Round(CableDeratedAmps, GlobalConfig.SigFigs);
            }
            catch { }
        }

    }
}
