using EDTLibrary.Calculators;
using EDTLibrary.Calculators.Cables;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace EDTLibrary.Models.Cables
{
    [AddINotifyPropertyChangedInterface]
    public class PowerCableModel: IPowerCable
    {

        #region Properties
        [Browsable(false)]
        public int Id { get; set; }
        public int OwnedById { get; set; }
        public string OwnedByType { get; set; }
        public string Tag { get; set; }
        //public string Type { get; set; } //Not Used
        public string Category { get; set; }

        private string _source;
        public string Source
        {
            get { return _source; }
            
            set { _source = value; }
        }

        public string Destination { get; set; }
        public string Type { get; set; }
        public string UsageType { get; set; }
        public int ConductorQty { get; set; }
        public double VoltageClass { get; set; }
        public double Insulation { get; set; }

        private int _qtyParallel;
        public int QtyParallel { get; set; }
        //public int QtyParallel
        //{
        //    get { return _qtyParallel; }
        //    set
        //    {
        //        _qtyParallel = value;
        //        if(_qtyParallel < 1) {
        //            _qtyParallel = 1;
        //        }
        //        if (_qtyParallel == null) {
        //            _qtyParallel = 1;
        //        }

        //        if (GlobalConfig.GettingRecords==false) {
        //            CalculateAmpacity();
        //        }
        //    }
        //}

        private string _size;

        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public double BaseAmps { get; set; }
        public double Spacing { get; set; }
        public double Derating { get; set; }
        public double DeratedAmps { get; set; }
        public double RequiredAmps { get; set; }
        public double RequiredSizingAmps { get; set; }

        public bool Indoor { get; set; }
        public string InstallationType { get; set; }


        [Browsable(false)]
        private IPowerConsumer _load;

        public IPowerConsumer Load
        {
            get { return _load; }
            set { _load = value; }
        }

        


        //TODO - code Table Rule by cable type
        string codeTable = GlobalConfig.CodeTable;

        #endregion


        public PowerCableModel()
        {
            QtyParallel = 1;
            Derating = 1;
            ConductorQty = 3;
        }


        public PowerCableModel(IPowerConsumer load)
        {
            _load = load;
            OwnedById = load.Id;
            OwnedByType = load.GetType().ToString();

            //Tag
            Source = load.FedFrom ?? "";
            Destination = load.Tag ?? "";
            CreateTag();

            UsageType = CableUsageTypes.Power.ToString();
            QtyParallel = 1;
            //Insulation = 100;
        }

        public void CreateTag()
        {
            if (Source!=null && Destination != null) {
                Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
            }
        }

        public void AssignOwner(IPowerConsumer load)
        {
            OwnedById = load.Id;
            OwnedByType = load.GetType().ToString();
        }
        /// <summary>
        /// Gets the Source Eq Derating, Destination Eq FLA
        /// </summary>
        public void AssignTagging(IPowerConsumer load)
        {
            Source = load.FedFrom;
            Destination = load.Tag;
            CreateTag();
        }
        public void GetCableParameters(IPowerConsumer load)
        {
            AssignTagging(load);

            OwnedById = load.Id;
            OwnedByType = load.GetType().ToString();
            UsageType = CableUsageTypes.Power.ToString();


            RequiredAmps = load.Fla;

            if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                RequiredAmps *= 1.25;
            }

            RequiredAmps = Math.Max(load.PdSizeTrip, RequiredAmps);

            RequiredSizingAmps = RequiredAmps / Derating;
            RequiredSizingAmps = Math.Round(RequiredSizingAmps, 1);


            //TODO - select all based on Cable.Type

            //CableDeratingCalculator cableDeratingCalculator = new CableDeratingCalculator();
            //CableDerating = cableDeratingCalculator.Calculate(load);
            Derating = 0.666;

            //Conductor Qty
            ConductorQtyCalculator conductorQtyCalculator = new ConductorQtyCalculator();
            ConductorQty = conductorQtyCalculator.Calculate(load);

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

            //TODO - null/error check for this data
            Type = SelectCableType(VoltageClass, ConductorQty, Insulation, UsageType);

        }

        private string SelectCableType(double voltageClass, int conductorQty, double insulation, string usageType)
        {
            DataTable cableType = LibraryTables.CableTypes.Select($"VoltageClass >= {voltageClass}").CopyToDataTable();
            cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                                         $"AND Conductors ={conductorQty}" +
                                         $"AND Insulation ={insulation}" +
                                         $"AND UsageType = '{usageType}'").CopyToDataTable();

            return cableType.Rows[0]["Type"].ToString();
        }



        /// <summary>
        /// Recursive function that gets the cable qty and size from Ampacity Table based on cable type and required amps
        /// </summary>
        public void CalculateCableQtySize()
        {

            //TODO - determine which AmpacityTable to select from
            // 1
            DataTable cableAmps = EdtSettings.CableAmpsUsedInProject_3C1kV.Copy();

            // 2
            //filter cables larger than RequiredAmps
            var cablesWithHigherAmps = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<double>("Amps75") >= RequiredSizingAmps
                                                          && x.Field<string>("CodeTable") == codeTable); // ex: Table2 (from CEC)

            // 3
            QtyParallel = 1;
            GetCableQty(QtyParallel);

            //Recursive method
            void GetCableQty(int cableQty)
            {
                if (cableQty < 20) {
                    if (cablesWithHigherAmps.Any()) {
                        cableAmps = null;
                        cableAmps = cablesWithHigherAmps.CopyToDataTable();

                        //select smallest of 
                        cableAmps = cableAmps.Select($"Amps75 = MIN(Amps75)").CopyToDataTable();

                        BaseAmps = double.Parse(cableAmps.Rows[0]["Amps75"].ToString());
                        BaseAmps = Math.Round(BaseAmps, 2);
                        DeratedAmps = BaseAmps * Derating;
                        DeratedAmps = Math.Round(DeratedAmps, 2);
                        QtyParallel = cableQty;
                        Size = cableAmps.Rows[0]["Size"].ToString();
                    }
                    else {
                        QtyParallel += 1;
                        cableAmps = EdtSettings.CableAmpsUsedInProject_3C1kV.Copy();
                        foreach (DataRow row in cableAmps.Rows) {
                            double amps75 = (double)row["Amps75"];
                            string size = row["Size"].ToString();
                            amps75 *= QtyParallel;
                            row["Amps75"] = amps75;
                            cablesWithHigherAmps = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                              && x.Field<double>("Amps75") >= RequiredSizingAmps
                                                              && x.Field<string>("CodeTable") == codeTable);
                        }
                        GetCableQty(QtyParallel);
                    }
                    _qtyParallel = QtyParallel;
                }
            }
        }
        public void CalculateCableQtySizeNew()
        {

            // 1
            DataTable cableAmps = LibraryTables.CableAmpacities.Copy();

            // 2
            //filter cables larger than RequiredAmps
            var cablesWithHigherAmps = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<double>("Amps75") >= RequiredSizingAmps
                                                          && x.Field<string>("CodeTable") == codeTable); // ex: Table2 (from CEC)

            DataTable cableSizesUsedInProject = SettingManager.SettingDict[nameof(EdtSettings.CableSizesUsedInProject_3C1kV)].TableValue;
            DataTable cablesWithHigherAmpsTable = cablesWithHigherAmps.CopyToDataTable();

            string size;
            DataRow cable;

            //TODO - determine which AmpacityTable to select from
            foreach (DataRow cableInProject in EdtSettings.CableSizesUsedInProject_3C1kV.Rows) {
                if (cableInProject.Field<bool>("UsedInProject") == false) {
                    size = cableInProject.Field<string>("Size");

                    for (int i = cablesWithHigherAmpsTable.Rows.Count - 1; i >= 0; i--) {
                        cable = cablesWithHigherAmpsTable.Rows[i];
                        if (cable["Size"].ToString() == size) {
                            cablesWithHigherAmpsTable.Rows.Remove(cable);
                        }

                    }
                }
            }
            

            // 3
            QtyParallel = 1;
            GetCableQty(QtyParallel);

            //Recursive method
            void GetCableQty(int cableQty)
            {
                if (cableQty < 20) {
                    if (cablesWithHigherAmps.Any()) {
                        cableAmps = null;
                        cableAmps = cablesWithHigherAmps.CopyToDataTable();

                        //select smallest of 
                        cableAmps = cableAmps.Select($"Amps75 = MIN(Amps75)").CopyToDataTable();

                        BaseAmps = double.Parse(cableAmps.Rows[0]["Amps75"].ToString());
                        BaseAmps = Math.Round(BaseAmps, 2);
                        DeratedAmps = BaseAmps * Derating;
                        DeratedAmps = Math.Round(DeratedAmps, 2);
                        QtyParallel = cableQty;
                        Size = cableAmps.Rows[0]["Size"].ToString();
                    }
                    else {
                        QtyParallel += 1;
                        cableAmps = EdtSettings.CableAmpsUsedInProject_3C1kV.Copy();
                        foreach (DataRow row in cableAmps.Rows) {
                            double amps75 = (double)row["Amps75"];
                            string size = row["Size"].ToString();
                            amps75 *= QtyParallel;
                            row["Amps75"] = amps75;
                            cablesWithHigherAmps = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                              && x.Field<double>("Amps75") >= RequiredSizingAmps
                                                              && x.Field<string>("CodeTable") == codeTable);
                        }
                        GetCableQty(QtyParallel);
                    }
                }
            }
        }
        public void CalculateAmpacity()
        {
            //TODO - if cables are already created and calbe size is removed it causes an error

            
            DataTable cableAmps = EdtSettings.CableAmpsUsedInProject_3C1kV.Copy(); //the created cable ampacity table

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<string>("CodeTable") == codeTable
                                                          && x.Field<string>("Size") == Size);

            cableAmps = null;
            try {
                cableAmps = cables.CopyToDataTable();
                //select smallest of the valid results
                BaseAmps = double.Parse(cableAmps.Rows[0]["Amps75"].ToString()) * QtyParallel;
                DeratedAmps = BaseAmps * Derating;
                DeratedAmps = Math.Round(DeratedAmps, GlobalConfig.SigFigs);
            }
            catch { }
        }

    }
}
