using EDTLibrary.Calculators;
using EDTLibrary.Calculators.Cables;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.Generic;
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
        public string InstallationType { get; set; } = EdtSettings.DefaultCableInstallationType;
        public string AmpacityTable { get; set; }


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

        }
        public PowerCableModel(IPowerConsumer load)
        {
            _load = load;
            AssignOwner(load);
            AssignTagging(load);
            Type = CableSizeManager.CableSizer.GetDefaultCableType(load);
            UsageType = CableUsageTypes.Power.ToString();
            QtyParallel = 1;

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

            Source = load.FedFrom.Tag;
            Destination = load.Tag;
            CreateTag();
        }

        public void SetCableParameters(IPowerConsumer load)
        {
            AssignTagging(load);
            AssignOwner(load);
            GetRequiredAmps(load);
            GetRequiredSizingAmps();
           
            Derating = 0.666;

            CableSizeManager.CableSizer.GetAmpacityTable(this);

            var test = AmpacityTable;
        }
        private void GetRequiredAmps(IPowerConsumer load)
        {
            RequiredAmps = load.Fla;
            if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                RequiredAmps *= 1.25;
            }
            RequiredAmps = Math.Max(load.PdSizeTrip, RequiredAmps);
        }
        private void GetRequiredSizingAmps()
        {
            RequiredSizingAmps = RequiredAmps / Derating;
            RequiredSizingAmps = Math.Round(RequiredSizingAmps, 1);
        }

        private string SelectCableType(double voltageClass, int conductorQty, double insulation, string usageType)
        {
            //TODO - null/error check for this data

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
                                                                        && x.Field<string>("AmpacityTable") == codeTable); // ex: Table2 (from CEC)
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
                        BaseAmps = Math.Round(BaseAmps, 1);
                        DeratedAmps = BaseAmps * Derating;
                        DeratedAmps = Math.Round(DeratedAmps, 1);
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
                                                                                    && x.Field<string>("AmpacityTable") == codeTable);
                        }
                        GetCableQty(QtyParallel);
                    }
                    _qtyParallel = QtyParallel;
                }
            }
        }
        public void CalculateAmpacity()
        {
            //TODO - if cables are already created and calbe size is removed it causes an error


            DataTable cableAmps = EdtSettings.CableAmpsUsedInProject_3C1kV.Copy(); //the created cable ampacity table

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<string>("AmpacityTable") == codeTable
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

        public void CalculateCableQtySizeNew()
        {
            GetRequiredSizingAmps();
            CableSizeManager.CableSizer.GetAmpacityTable(this);

            string ampsColumn = "Amps75";

            DataTable cableAmpacityTable = LibraryTables.CecCableAmpacities.Copy();
            DataTable cablesWithHigherAmpsInProject = new DataTable();

            foreach (DataColumn column in cableAmpacityTable.Columns) {
                DataColumn columnToadd = new DataColumn();
                columnToadd.ColumnName = column.ColumnName;
                cablesWithHigherAmpsInProject.Columns.Add(columnToadd);
            }

            // 1 - filter cables larger than RequiredAmps
            EnumerableRowCollection<DataRow> cablesWithHigherAmps = SelectCablesWithValidSizeAndAmps(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject);

            // 3 - increase quantity until a valid cable is found
            QtyParallel = 1;
            GetCableQty(QtyParallel);


            // Helper - 3 Recursive method
            void GetCableQty(int cableQty)
            {
                if (cableQty < 99) {
                    if (cablesWithHigherAmpsInProject.Rows.Count > 0) {

                        //select smallest of 
                        cablesWithHigherAmpsInProject = cablesWithHigherAmpsInProject.Select($"{ampsColumn} = MIN({ampsColumn})").CopyToDataTable();

                        BaseAmps = double.Parse(cablesWithHigherAmpsInProject.Rows[0][ampsColumn].ToString());
                        BaseAmps = Math.Round(BaseAmps, 1);
                        DeratedAmps = BaseAmps * Derating;
                        DeratedAmps = Math.Round(DeratedAmps, 1);
                        QtyParallel = cableQty;
                        Size = cablesWithHigherAmpsInProject.Rows[0]["Size"].ToString();
                    }
                    else {
                        QtyParallel += 1;
                        cableAmpacityTable = LibraryTables.CecCableAmpacities.Copy();
                        foreach (DataRow row in cableAmpacityTable.Rows) {
                            double amps75 = (double)row[ampsColumn];
                            string size = row["Size"].ToString();
                            amps75 *= QtyParallel;
                            row[ampsColumn] = amps75;
                        }
                        cablesWithHigherAmps = SelectCablesWithValidSizeAndAmps(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject);

                        GetCableQty(QtyParallel);
                        var test = "asdf";
                    }
                    _qtyParallel = QtyParallel;
                }
            }
        }
        private EnumerableRowCollection<DataRow> SelectCablesWithValidSizeAndAmps(string ampsColumn, DataTable cableAmpacityTable, DataTable cablesWithHigherAmpsInProject)
        {
            var cablesWithHigherAmps = cableAmpacityTable.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                                                    && x.Field<double>(ampsColumn) >= RequiredSizingAmps
                                                                                    && x.Field<string>("AmpacityTable") == AmpacityTable); // ex: Table2 (from CEC)

            // remove cable if size is not in project
            foreach (var cableSizeInProject in EdtSettings.CableSizesUsedInProject) {
                foreach (var cableWithHigherAmps in cablesWithHigherAmps) {

                    if (cableSizeInProject.Size == cableWithHigherAmps.Field<string>("Size") &&
                        cableSizeInProject.Type == Type &&
                        cableSizeInProject.UsedInProject == true) {
                        //var cableRow = cableWithHigherAmps;
                        cablesWithHigherAmpsInProject.Rows.Add(cableWithHigherAmps.ItemArray);
                        var coutn = cablesWithHigherAmpsInProject.Rows.Count;
                    }
                }
            }
            return cablesWithHigherAmps;
        }
        public void CalculateAmpacityNew()
        {
            string ampsColumn = "Amps75";
            //TODO - if cables are already created and calbe size is removed it causes an error


            DataTable cableAmps = LibraryTables.CecCableAmpacities.Copy(); //the created cable ampacity table

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<string>("AmpacityTable") == AmpacityTable
                                                          && x.Field<string>("Size") == Size);

            cableAmps = null;
            try {
                cableAmps = cables.CopyToDataTable();
                //select smallest of the valid results
                BaseAmps = double.Parse(cableAmps.Rows[0][ampsColumn].ToString()) * QtyParallel;
                BaseAmps = Math.Round(BaseAmps, 1);
                DeratedAmps = BaseAmps * Derating;
                DeratedAmps = Math.Round(DeratedAmps, GlobalConfig.SigFigs);
            }
            catch { }
        }
    }
}
