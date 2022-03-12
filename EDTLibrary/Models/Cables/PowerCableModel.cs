
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeTables;
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
        private string _type;

        public string Type
        {
            get { return _type; }
            set 
            {
                if (value != "") {
                    _type = value;
                }
                TypeModel = TypeManager.GetCableType(_type);

                if (GlobalConfig.GettingRecords == false) {
                    var variant = this.Load;
                    CalculateCableQtySizeNew();
                }
            }
        }

        public CableTypeModel TypeModel { get; set; }
        public string UsageType { get; set; }
        public int ConductorQty { get; set; }
        public double VoltageClass { get; set; }
        public double Insulation { get; set; }

        private int _qtyParallel;
        //public int QtyParallel { get; set; }
        public int QtyParallel
        {
            get { return _qtyParallel; }
            set
            {
                _qtyParallel = value;
                if (_qtyParallel < 1) {
                    _qtyParallel = 1;
                }
                if (_qtyParallel == null) {
                    _qtyParallel = 1;
                }

                if (GlobalConfig.GettingRecords == false) {
                }
            }
        }

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

        public bool Outdoor { get; set; }

        private string _installationType = EdtSettings.DefaultCableInstallationType;

        public string InstallationType
        {
            get { return _installationType; }
            set 
            { 
                _installationType = value;
                if (GlobalConfig.GettingRecords == false) {
                    CalculateCableQtySizeNew();
                }
            }
        }

        public string AmpacityTable { get; set; }
        public string InstallationDiagram { get; set; }


        private IPowerConsumer _load;

        public IPowerConsumer Load
        {
            get { return _load; }
            set { _load = value; }
        }


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
            _load = load;
            AssignTagging(load);
            AssignOwner(load);
            GetRequiredAmps(load);

            RequiredSizingAmps = GetRequiredSizingAmps();
            Spacing = CableSizeManager.CableSizer.GetDefaultCableSpacing(this);
            AmpacityTable =  CableSizeManager.CableSizer.GetAmpacityTable(this);
        }
        public void GetRequiredAmps(IPowerConsumer load)
        {
            RequiredAmps = load.Fla;
            if (load.Type == LoadTypes.MOTOR.ToString() | load.Type == LoadTypes.TRANSFORMER.ToString()) {
                RequiredAmps *= 1.25;
            }

            RequiredAmps = Math.Min(load.PdSizeTrip, RequiredAmps);
        }
        public double GetRequiredSizingAmps()
        {
            Derating = CableSizeManager.CableSizer.GetDerating(this);
            RequiredSizingAmps = RequiredAmps / Derating;
            RequiredSizingAmps = Math.Round(RequiredSizingAmps, 1);
            return RequiredSizingAmps;
        }

        public string SelectCableType(double voltageClass, int conductorQty, double insulation, string usageType)
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
        public void CalculateCableQtySizeNew()
        {
            RequiredSizingAmps = GetRequiredSizingAmps();
            AmpacityTable = CableSizeManager.CableSizer.GetAmpacityTable(this);
            InstallationDiagram = "";

            string ampsColumn = "Amps75";

            if (InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
                CableQtySize_LadderTray(this, ampsColumn);
            }

            else if (InstallationType == GlobalConfig.CableInstallationType_DirectBuried
                  || InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {

                CableQtySize_DirectBuried(this, ampsColumn);
            }
        }

        private void CableQtySize_LadderTray(IPowerCable cable, string ampsColumn)
        {
            DataTable cableAmpacityTable = LibraryTables.CecCableAmpacities.Copy();
            DataTable cablesWithHigherAmpsInProject = new DataTable();

            foreach (DataColumn column in cableAmpacityTable.Columns) {
                DataColumn columnToadd = new DataColumn();
                columnToadd.ColumnName = column.ColumnName;
                cablesWithHigherAmpsInProject.Columns.Add(columnToadd);
            }

            // 1 - filter cables larger than RequiredAmps first iteration
            SelectValidCables_SizeAmps(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject);

            // 3 - increase quantity until a valid cable is found
            cable.QtyParallel = 1;
            GetCableQty(cable.QtyParallel);


            // Helper - 3 Recursive method
            void GetCableQty(int cableQty)
            {
                if (cableQty < 99) {
                    if (cablesWithHigherAmpsInProject.Rows.Count > 0) {
                        cable.Derating = CableSizeManager.CableSizer.GetDerating(cable);
                        //select smallest of 
                        cablesWithHigherAmpsInProject = cablesWithHigherAmpsInProject.Select($"{ampsColumn} = MIN({ampsColumn})").CopyToDataTable();

                        cable.BaseAmps = double.Parse(cablesWithHigherAmpsInProject.Rows[0][ampsColumn].ToString());
                        cable.BaseAmps = Math.Round(cable.BaseAmps, 1);
                        cable.DeratedAmps = cable.BaseAmps * cable.Derating;
                        cable.DeratedAmps = Math.Round(cable.DeratedAmps, 1);
                        cable.QtyParallel = cableQty;
                        cable.Size = cablesWithHigherAmpsInProject.Rows[0]["Size"].ToString();
                    }
                    else {
                        cable.QtyParallel += 1;
                        cableAmpacityTable = LibraryTables.CecCableAmpacities.Copy();
                        foreach (DataRow row in cableAmpacityTable.Rows) {
                            double amps75 = (double)row[ampsColumn];
                            string size = row["Size"].ToString();
                            amps75 *= cable.QtyParallel;
                            row[ampsColumn] = amps75;
                        }
                        SelectValidCables_SizeAmps(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject);

                        GetCableQty(cable.QtyParallel);
                    }
                    
                }
            }
        }
        private void SelectValidCables_SizeAmps(string ampsColumn, DataTable cableAmpacityTable, DataTable cablesWithHigherAmpsInProject)
        {
            var cablesWithHigherAmps = cableAmpacityTable.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                                                    && x.Field<double>(ampsColumn) >= GetRequiredSizingAmps()
                                                                                    && x.Field<string>("AmpacityTable") == AmpacityTable); // ex: Table2 (from CEC)

            // remove cable if size is not in project
            foreach (var cableSizeInProject in EdtSettings.CableSizesUsedInProject) {
                foreach (var cableWithHigherAmps in cablesWithHigherAmps) {

                    if (cableSizeInProject.Size == cableWithHigherAmps.Field<string>("Size") &&
                        cableSizeInProject.Type == Type &&
                        cableSizeInProject.UsedInProject == true) {
                        //var cableRow = cableWithHigherAmps;
                        cablesWithHigherAmpsInProject.Rows.Add(cableWithHigherAmps.ItemArray);
                        var count = cablesWithHigherAmpsInProject.Rows.Count;
                    }
                }
            }
        }
       


        private void CableQtySize_DirectBuried(IPowerCable cable, string ampsColumn)
        {
            DataTable cableAmpacityTable = LibraryTables.CecCableAmpacities.Copy();
            DataTable cablesWithHigherAmpsInProject = new DataTable();

            foreach (DataColumn column in cableAmpacityTable.Columns) {
                DataColumn columnToadd = new DataColumn();
                columnToadd.ColumnName = column.ColumnName;
                cablesWithHigherAmpsInProject.Columns.Add(columnToadd);
            }

            // 1 - filter cables larger than RequiredAmps first iteration
            // 2 - increase quantity until a valid cable is found
            cable.QtyParallel = 1;
            SelectValidCables_SizeAmpsQty(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject, cable.QtyParallel);

            GetCableQty(cable.QtyParallel);

            // Helper - 3 Recursive method
            void GetCableQty(int cableQty)
            {
                if (cableQty < 10) {
                    if (cablesWithHigherAmpsInProject.Rows.Count > 0) {
                        cable.Derating = CableSizeManager.CableSizer.GetDerating(cable);
                        //select smallest of 
                        cablesWithHigherAmpsInProject = cablesWithHigherAmpsInProject.Select($"{ampsColumn} = MIN({ampsColumn})").CopyToDataTable();

                        cable.BaseAmps = double.Parse(cablesWithHigherAmpsInProject.Rows[0][ampsColumn].ToString());
                        cable.BaseAmps = Math.Round(cable.BaseAmps, 1);
                        cable.DeratedAmps = cable.BaseAmps * cable.Derating;
                        cable.DeratedAmps = Math.Round(cable.DeratedAmps, 1);
                        cable.QtyParallel = cableQty;
                        cable.Size = cablesWithHigherAmpsInProject.Rows[0]["Size"].ToString();
                        cable.InstallationDiagram = cablesWithHigherAmpsInProject.Rows[0]["Diagram"].ToString();
                    }
                    else {
                        cable.QtyParallel += 1;
                        cableAmpacityTable = LibraryTables.CecCableAmpacities.Copy();
                        foreach (DataRow row in cableAmpacityTable.Rows) {
                            double amps75 = (double)row[ampsColumn];
                            string size = row["Size"].ToString();
                            if (row["QtyParallel"].ToString() == cable.QtyParallel.ToString()) {
                                amps75 *= cable.QtyParallel;
                                row[ampsColumn] = amps75;
                            }
                            
                        }
                        SelectValidCables_SizeAmpsQty(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject, cable.QtyParallel);

                        GetCableQty(cable.QtyParallel);
                    }

                }
            }
        }
        private void SelectValidCables_SizeAmpsQty(string ampsColumn, DataTable cableAmpacityTable, DataTable cablesWithHigherAmpsInProject, int qtyParallel)
        {
            var cablesWithHigherAmps = cableAmpacityTable.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                                                    && x.Field<double>(ampsColumn) >= GetRequiredSizingAmps()
                                                                                    && x.Field<string>("AmpacityTable") == AmpacityTable
                                                                                    && x.Field<long>("QtyParallel").ToString() == qtyParallel.ToString());

            // remove cable if size is not in project
            foreach (var cableSizeInProject in EdtSettings.CableSizesUsedInProject) {
                foreach (var cableWithHigherAmps in cablesWithHigherAmps) {

                    if (cableSizeInProject.Size == cableWithHigherAmps.Field<string>("Size") &&
                        cableSizeInProject.Type == Type &&
                        cableSizeInProject.UsedInProject == true) {

                         
                        //var cableRow = cableWithHigherAmps;
                        cablesWithHigherAmpsInProject.Rows.Add(cableWithHigherAmps.ItemArray);
                        var count = cablesWithHigherAmpsInProject.Rows.Count;
                    }
                }
            }
        }
        

        public void CalculateAmpacityNew(IPowerConsumer load)
        {
            Load = load;
            string ampsColumn = "Amps75";

            if (InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
                CalculateAmpacity_LadderTray(this, ampsColumn);
            }

            else if (InstallationType == GlobalConfig.CableInstallationType_DirectBuried
                  || InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {

                CalculateAmpacity_DirectBuried(this, ampsColumn);
            }
        }

        private void CalculateAmpacity_LadderTray(IPowerCable cable, string ampsColumn)
        {
            cable.Derating = CableSizeManager.CableSizer.GetDerating(cable);

            DataTable cableAmps = LibraryTables.CecCableAmpacities.Copy(); //the created cable ampacity table

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<string>("AmpacityTable") == cable.AmpacityTable
                                                          && x.Field<string>("Size") == cable.Size);
            cableAmps = null;
            try {
                cableAmps = cables.CopyToDataTable();
                //select smallest of the valid results
                cable.BaseAmps = double.Parse(cableAmps.Rows[0][ampsColumn].ToString()) * cable.QtyParallel;
                cable.BaseAmps = Math.Round(BaseAmps, 1);
                cable.DeratedAmps = cable.BaseAmps * cable.Derating;
                cable.DeratedAmps = Math.Round(cable.DeratedAmps, GlobalConfig.SigFigs);
            }
            catch { }
        }

        private void CalculateAmpacity_DirectBuried(IPowerCable cable, string ampsColumn)
        {
            cable.Derating = CableSizeManager.CableSizer.GetDerating(cable);


            DataTable cableAmps = LibraryTables.CecCableAmpacities.Copy(); //the created cable ampacity table

            //filter cables larger than RequiredAmps          
            var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                          && x.Field<string>("AmpacityTable") == cable.AmpacityTable
                                                          && x.Field<string>("Size") == cable.Size
                                                          && x.Field<long>("QtyParallel").ToString() == cable.QtyParallel.ToString());
            cableAmps = null;
            try {
                cableAmps = cables.CopyToDataTable();
                //select smallest of the valid results
                cable.BaseAmps = double.Parse(cableAmps.Rows[0][ampsColumn].ToString()) * cable.QtyParallel;
                cable.BaseAmps = Math.Round(BaseAmps, 1);
                cable.DeratedAmps = cable.BaseAmps * cable.Derating;
                cable.DeratedAmps = Math.Round(cable.DeratedAmps, GlobalConfig.SigFigs);
                cable.InstallationDiagram = cableAmps.Rows[0]["Diagram"].ToString();
            }
            catch { }
        }
    }
}
