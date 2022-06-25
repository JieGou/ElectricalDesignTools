
using EdtLibrary.Commands;
using EDTLibrary;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Managers;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EDTLibrary.Models.Cables;

[AddINotifyPropertyChangedInterface]
public class CableModel : ICable
{

    public CableModel()
    {
        AutoSizeCableCommand = new RelayCommand(AutoSize);

    }
    public CableModel(IPowerConsumer load)
    {
        _load = load;
        AssignOwner(load);
        AssignTagging(load);
        GlobalConfig.GettingRecords = true;
        Type = CableManager.CableSizer.GetDefaultCableType(load);
        GlobalConfig.GettingRecords = false;

        UsageType = CableUsageTypes.Power.ToString();
        QtyParallel = 1;
        Spacing = 100;

        AutoSizeCableCommand = new RelayCommand(AutoSize);

    }
    public ICommand AutoSizeCableCommand { get; }

    #region Properties
    [Browsable(false)]
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public string Tag { get; set; }
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
        get
        {
            if (_typeModel == null) {
                return _type;
            }
            return _typeModel.Type;
        }
        set
        {
            _type = value;
            if (string.IsNullOrEmpty(_type) == false) {
                TypeModel = TypeManager.GetCableTypeModel(_type);
                TypeModel = TypeManager.GetCableTypeModel(_type);
            }

        }

    }

    private CableTypeModel _typeModel;
    public CableTypeModel TypeModel
    {

        get { return _typeModel; }
        set
        {
            if (value == null) 
                return;

            var oldValue = _typeModel;
            _typeModel = value;
            _type = _typeModel.Type;
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(TypeModel), OldValue = oldValue, NewValue = _typeModel };
                Undo.AddUndoCommand(cmd);
            }
            if (GlobalConfig.GettingRecords == false) {
                if (UsageType==CableUsageTypes.Power.ToString()) {
                    AutoSize();
                }
                if (CableManager.IsUpdatingPowerCables == false) {
                    CableManager.UpdateLoadPowerComponentCablesAsync(Load as IPowerConsumer, ScenarioManager.ListManager);
                }
            }
            CreateSizeList();
            OnPropertyUpdated();
        }
    }

    public List<CableTypeModel> TypeList { get; set; } = new List<CableTypeModel>();
    public List<string> SizeList { get; set; } = new List<string>();
    
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
            var oldValue = _qtyParallel;
            _qtyParallel = value;
            if (_qtyParallel == null || _qtyParallel < 1 ) {
                _qtyParallel = 1;
            }
          
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(QtyParallel), OldValue = oldValue, NewValue = _qtyParallel };
                Undo.AddUndoCommand(cmd);
            }
            //if (GlobalConfig.GettingRecords == false && _calculating == false) {
            //    CalculateAmpacityNew(_load);
            //}
        }
    }

    private bool _calculating;
    private string _size;

    public string Size
    {
        get { return _size; }
        set
        {
            var oldValue = _size;
            _size = value;

            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Size), OldValue = oldValue, NewValue = _size };
                Undo.AddUndoCommand(cmd);
            }

            if (GlobalConfig.GettingRecords == false && UsageType == CableUsageTypes.Power.ToString()) {
                CalculateAmpacity(Load);
            }
            OnPropertyUpdated();
        }
    }
    public bool SizeIsValid { get; set; }

    public double BaseAmps { get; set; }

    private double _spacing;

    public double Spacing
    {
        get { return _spacing; }
        set
        {
            var oldValue = _spacing;
            _spacing = value;
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Spacing), OldValue = oldValue, NewValue = _spacing };
                Undo.AddUndoCommand(cmd);
            }
        }
    }
    public double Length { get; set; }

    public double Derating { get; set; }
    public double DeratedAmps { get; set; }
    public string DeratedAmpsToolTip
    {
        get { return BaseAmps + " A x " + Derating; }

    }
    public double RequiredAmps { get; set; }

    public string RequiredAmpsToolTip
    {
        get
        {
            if (_load != null) {
                return $"OCDP Trip = {_load.PdSizeTrip} A";

            }
            else {
                return "OCDP Trip = xx A";
            }
        }

    }

    public double RequiredSizingAmps { get; set; }

    private bool _outdoor;

    public bool Outdoor
    {
        get { return _outdoor; }
        set
        {
            var oldValue = _outdoor;
            _outdoor = value;
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Outdoor), OldValue = oldValue, NewValue = _outdoor };
                Undo.AddUndoCommand(cmd);
            }
        }
    }

    private string _installationType = EdtSettings.DefaultCableInstallationType;

    public string InstallationType
    {
        get { return _installationType; }
        set
        {
            var oldValue = _installationType;
            _installationType = value;
            if (GlobalConfig.GettingRecords == false) {
                AutoSize();
            }
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(InstallationType), OldValue = oldValue, NewValue = _installationType };
                Undo.AddUndoCommand(cmd);
            }
        }
    }

    public string AmpacityTable { get; set; }
    public string InstallationDiagram { get; set; }


    private ICableUser _load;
    public ICableUser Load
    {
        get { return _load; }
        set { _load = value; }
    }





    public double HeatLoss { get; set; }



    #endregion



    public void CreateTag()
    {
        if (Source != null && Destination != null) {
            Tag = Source.Replace("-", "") + "-" + Destination.Replace("-", "");
        }
        OnPropertyUpdated();
    }
    public void AssignOwner(ICableUser load)
    {
        OwnerId = load.Id;
        OwnerType = load.GetType().ToString();
    }
    /// <summary>
    /// Gets the Source Eq Derating, Destination Eq FLA
    /// </summary>
    public void AssignTagging(ICableUser load)
    {
        if (load.FedFrom != null) {
            Source = load.FedFrom.Tag;
            Destination = load.Tag;
            CreateTag();
        }
    }
    public void CreateTypeList(ICableUser load)
    {
        TypeList.Clear();

        var cableVoltageClass = LibraryManager.GetCableVoltageClass(load.Voltage);

        foreach (var cableType in TypeManager.CableTypes) {
            if (cableVoltageClass == cableType.VoltageClass
                && this.UsageType == cableType.UsageType) {
                TypeList.Add(cableType);
            }
        }
    }
    public void CreateSizeList()
    {
        SizeList.Clear();
        if (EdtSettings.CableSizesUsedInProject == null) {
            return;
        }
        foreach (var cable in EdtSettings.CableSizesUsedInProject) {
            if (cable.Type == this.Type && cable.UsedInProject == true) {
                SizeList.Add(cable.Size);
            }
        }
    }
    public void SetCableParameters(ICableUser load)
    {
        _load = load;
        AssignTagging(load);
        AssignOwner(load);
        GetRequiredAmps(load);

        RequiredSizingAmps = GetRequiredSizingAmps();
        Spacing = CableManager.CableSizer.GetDefaultCableSpacing(this);
        AmpacityTable = CableManager.CableSizer.GetAmpacityTable(this);
        OnPropertyUpdated();
    }
    public double GetRequiredAmps(ICableUser load)
    {
        
        RequiredAmps = load.Fla;
        if (load.Type == LoadTypes.MOTOR.ToString() || load.Type == LoadTypes.TRANSFORMER.ToString()) {
            RequiredAmps *= 1.25;
        }


        if (load.GetType() == typeof(LoadModel)) {
            RequiredAmps = Math.Max(load.PdSizeTrip, RequiredAmps);
        }
        else {
            RequiredAmps = Math.Min(load.PdSizeTrip, RequiredAmps);
        }
        return RequiredAmps;
    }
    public double GetRequiredSizingAmps()
    {
        //Debug.WriteLine("GetRequiredSizingAmps");

        Derating = CableManager.CableSizer.GetDerating(this);
        RequiredSizingAmps = GetRequiredAmps(_load) / Derating;
        RequiredSizingAmps = Math.Round(RequiredSizingAmps, 1);
        return RequiredSizingAmps;

    }

    public string SelectCableType(double voltageClass, int conductorQty, double insulation, string usageType)
    {

        DataTable cableType = DataTables.CableTypes.Select($"VoltageClass >= {voltageClass}").CopyToDataTable();
        cableType = cableType.Select($"VoltageClass = MIN(VoltageClass) " +
                                     $"AND Conductors ={conductorQty}" +
                                     $"AND Insulation ={insulation}" +
                                     $"AND UsageType = '{usageType}'").CopyToDataTable();

        return cableType.Rows[0]["Type"].ToString();
        OnPropertyUpdated();

    }

    /// <summary>
    /// Recursive function that gets the cable qty and size from Ampacity Table based on cable type and required amps
    /// </summary>
    
    //Qty Size
    public void AutoSize()
    {
        Undo.Undoing=true; 
        //_calculating = true;
        RequiredSizingAmps = GetRequiredSizingAmps();
        AmpacityTable = CableManager.CableSizer.GetAmpacityTable(this);
        InstallationDiagram = "";

        string ampsColumn = "Amps75";

        if (InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
            GetCableQtySize_ForLadderTray(this, ampsColumn);
        }

        else if (InstallationType == GlobalConfig.CableInstallationType_DirectBuried
              || InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {

            CableQtySize_DirectBuriedOrRaceWayConduit(this, ampsColumn);
        }
        CalculateAmpacity(Load);
        OnPropertyUpdated();
        Undo.Undoing = false; 
        //_calculating = false;

    }
    //Qty Size
    private void GetCableQtySize_ForLadderTray(ICable cable, string ampsColumn)
    {

        //TODO - move this into above "CalculateCableQtyAndSize();" for both
        //TODO - Fix cable spacing default

        DataTable cableAmpacityTable = DataTables.CecCableAmpacities.Copy();
        //var ampacityTableFiltered = cableAmpacityTable.AsEnumerable().Where(x => x.Field<string>("AmpacityTable") == AmpacityTable);
        //cableAmpacityTable.Rows.Clear();
        //foreach (var cableAmpacityRow in ampacityTableFiltered) {
        //    cableAmpacityTable.Rows.Add(cableAmpacityRow.ItemArray);
        //}

        DataTable cablesWithHigherAmpsInProject = cableAmpacityTable.Copy();
                cablesWithHigherAmpsInProject.Rows.Clear();


        // 1 - filter cables larger than RequiredAmps first iteration
        SelectValidCables_SizeAmps(ampsColumn, cableAmpacityTable, cablesWithHigherAmpsInProject);
        // 3 - increase quantity until a valid cable is found
        cable.QtyParallel = 1;
        GetCableQty(cable.QtyParallel);


        // Helper - 3 Recursive method
        void GetCableQty(int cableQty)
        {
            if (cableQty < 20) {
                if (cablesWithHigherAmpsInProject.Rows.Count > 0) {
                    cable.Derating = CableManager.CableSizer.GetDerating(cable);
                    //select smallest of 
                    cablesWithHigherAmpsInProject = cablesWithHigherAmpsInProject.Select($"{ampsColumn} = MIN({ampsColumn})").CopyToDataTable();

                    cable.BaseAmps = double.Parse(cablesWithHigherAmpsInProject.Rows[0][ampsColumn].ToString());
                    cable.BaseAmps = Math.Round(cable.BaseAmps, 1);
                    cable.DeratedAmps = cable.BaseAmps * cable.Derating;
                    cable.DeratedAmps = Math.Round(cable.DeratedAmps, 1);
                    cable.QtyParallel = cableQty;
                    cable.CreateSizeList();
                    cable.Size = cablesWithHigherAmpsInProject.Rows[0]["Size"].ToString();
                }
                else {
                    cable.QtyParallel += 1;
                    cableAmpacityTable = DataTables.CecCableAmpacities.Copy();
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
                                                                                && x.Field<double>(ampsColumn) >= RequiredSizingAmps
                                                                                && x.Field<string>("AmpacityTable") == AmpacityTable); // ex: Table2 (from CEC)
                                                                                                                                       // remove cable if size is not in project
        foreach (var cableSizeInProject in EdtSettings.CableSizesUsedInProject) {
            string cableDetails = cableSizeInProject.Type.ToString() + ", " + cableSizeInProject.Size.ToString();
            //Debug.WriteLine(cableDetails);

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
    
    //Qty Size
    private void CableQtySize_DirectBuriedOrRaceWayConduit(ICable cable, string ampsColumn)
    {
        DataTable cableAmpacityTable = DataTables.CecCableAmpacities.Copy();
        //var ampacityTableFiltered = cableAmpacityTable.AsEnumerable().Where(x => x.Field<string>("AmpacityTable") == AmpacityTable);
        //cableAmpacityTable.Rows.Clear();
        //foreach (var cableAmpacityRow in ampacityTableFiltered) {
        //    cableAmpacityTable.Rows.Add(cableAmpacityRow.ItemArray);
        //}

        DataTable cablesWithHigherAmpsInProject = cableAmpacityTable.Copy();
        cablesWithHigherAmpsInProject.Rows.Clear();

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
                    cable.Derating = CableManager.CableSizer.GetDerating(cable);
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
                    cableAmpacityTable = DataTables.CecCableAmpacities.Copy();
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
            //CalculateAmpacityNew(Load);
        }
    }
    private void SelectValidCables_SizeAmpsQty(string ampsColumn, DataTable cableAmpacityTable, DataTable cablesWithHigherAmpsInProject, int qtyParallel)
    {
        var cablesWithHigherAmps = cableAmpacityTable.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                                                && x.Field<double>(ampsColumn) >= RequiredSizingAmps
                                                                                && x.Field<string>("AmpacityTable") == AmpacityTable
                                                                                && x.Field<long>("QtyParallel").ToString() == qtyParallel.ToString()
                                                                                
                                                                                );

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

    //Ampacity
    public string CalculateAmpacity(ICableUser load)
    {
        _calculating = true;
        SizeIsValid = true;
        Load = load;
        string ampsColumn = "Amps75";
        RequiredSizingAmps = GetRequiredSizingAmps();
        string output = "Default";

        if (InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
            CalculateAmpacity_LadderTray(this, ampsColumn);
            output = "CalculateAmpacity_LadderTray";
        }

        else if (InstallationType == GlobalConfig.CableInstallationType_DirectBuried
              || InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {

            CalculateAmpacity_DirectBuriedOrRaceWayConduit(this, ampsColumn);
            output = "CalculateAmpacity_DirectBuriedOrRaceWayConduit";
        }
        if (RequiredAmps > DeratedAmps) {
            SizeIsValid = false;
            SetCablInvalid(this);
        }
        OnPropertyUpdated();
        _calculating = false;
        return output;

    }
    private void CalculateAmpacity_LadderTray(ICable cable, string ampsColumn)
    {
        cable.Derating = CableManager.CableSizer.GetDerating(cable);

        DataTable cableAmps = DataTables.CecCableAmpacities.Copy(); //the created cable ampacity table

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
        catch {
            SizeIsValid = false;
            SetCablInvalid(this);
        }
    }
    private void CalculateAmpacity_DirectBuriedOrRaceWayConduit(ICable cable, string ampsColumn)
    {
        cable.Derating = CableManager.CableSizer.GetDerating(cable);


        DataTable cableAmps = DataTables.CecCableAmpacities.Copy(); //the created cable ampacity table

        //filter cables larger than RequiredAmps          
        var cables = cableAmps.AsEnumerable().Where(x => x.Field<string>("Code") == EdtSettings.Code
                                                      && x.Field<string>("AmpacityTable") == cable.AmpacityTable
                                                      && x.Field<string>("Size") == cable.Size
                                                      && x.Field<long>("QtyParallel").ToString() == cable.QtyParallel.ToString()
                                                      && x.Field<string>("Diagram") == cable.InstallationDiagram);
        cableAmps = null;
        try {
            cableAmps = cables.CopyToDataTable();
            //select smallest of the valid results
            var tableAmps = double.Parse(cableAmps.Rows[0][ampsColumn].ToString());
            cable.BaseAmps = double.Parse(cableAmps.Rows[0][ampsColumn].ToString()) * cable.QtyParallel;
            cable.BaseAmps = Math.Round(BaseAmps, 1);
            cable.DeratedAmps = cable.BaseAmps * cable.Derating;
            cable.DeratedAmps = Math.Round(cable.DeratedAmps, GlobalConfig.SigFigs);
            cable.InstallationDiagram = cableAmps.Rows[0]["Diagram"].ToString();

        }
        catch {
            SizeIsValid = false;
            SetCablInvalid(this);
        }
    }

    private void SetCablInvalid(ICable cable)
    {
        cable.SizeIsValid = false;
        //cable.Size = "n/a";
        //cable.BaseAmps = 0;
        //cable.DeratedAmps = 0;
        //cable.QtyParallel = 99;
        cable.InstallationDiagram = "n/a";
    }

    //Events
    public event EventHandler PropertyUpdated;
    public virtual async Task OnPropertyUpdated()
    {
        await Task.Run(() => {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        });
    }
}

