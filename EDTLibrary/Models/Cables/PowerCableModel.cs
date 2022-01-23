using EDTLibrary.Calculators;
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
    public class PowerCableModel
    {

        #region Properties
        [Browsable(false)]
        public int Id { get; set; }
        public string Tag { get; set; }
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


        [Browsable(false)]
        public PowerConsumer Load { get; set; }


        //TODO - code Table Rule by cable type
        string codeTable = "Table2";

        #endregion


        public PowerCableModel()
        {
            CableQty = 1;
            CableDerating = 1;
            ConductorQty = 3;
        }


        public PowerCableModel(PowerConsumer load)
        {
            Load = load;

            //Tag
            Source = load.FedFrom ?? "";
            Destination = load.Tag ?? "";
            CreateTag();

            UsageType = CableUsageTypes.Power.ToString();
            CableQty = 1;

            GetCableParameters(load);
            CalculateCableQtySize();
            CalculateAmpacity();
        }
        public void CreateTag() {
            Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
        }
        
        /// <summary>
        /// Gets the Source Eq Derating, Destination Eq FLA
        /// </summary>
        
        public void GetCableParameters(PowerConsumer load)
        {
            //gets source derating
            CableDeratingCalculator cableDeratingCalculator = new CableDeratingCalculator();
            CableDerating = cableDeratingCalculator.Calculate(load);


            //Conductor Qty
            ConductorQtyCalculator conductorQtyCalculator = new ConductorQtyCalculator();
            ConductorQty = conductorQtyCalculator.Calculate(load);


            CableRequiredAmps = load.Fla;

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
            else {
                Insulation = 100;
            }




            //Cable Type - selects the cabletype based on VoltageClass, ConuctorQty, Insulation and UsageType
            //TODO - get these parameters from settings selected cable type???
            //TODO - null/error check for this data
            DataTable cableType = LibraryTables.CableTypes.Select($"VoltageClass >= {VoltageClass}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                                         $"AND Conductors ={ConductorQty}" +
                                         $"AND Insulation ={Insulation}" +
                                         $"AND UsageType = '{UsageType}'").CopyToDataTable();

            CableType = cableType.Rows[0]["Type"].ToString();

        }



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
