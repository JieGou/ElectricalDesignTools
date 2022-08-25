
using EdtLibrary.Commands;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
        Load = load;
        AssignOwner(load);
        SetTagging(load);
        DaManager.GettingRecords = true;
        Type = CableManager.CableSizer.GetDefaultCableType(load);
        DaManager.GettingRecords = false;

        UsageType = CableUsageTypes.Power.ToString();
        QtyParallel = 1;
        Spacing = 100;
        Length = CableManager.GetLength(load.Category);

        AutoSizeCableCommand = new RelayCommand(AutoSize);

    }
    public ICommand AutoSizeCableCommand { get; }

    #region Properties
    [Browsable(false)]
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public string Tag { get; set; }
    private string _sizeTag;

    public string SizeTag
    {
        get {
            string voltage = TypeModel.VoltageClass.ToString() +"V";
            if (TypeModel.VoltageClass>=1000) {
                voltage = (TypeModel.VoltageClass / 1000).ToString() + "kV";
            }
            return $"{TypeModel.ConductorQty}C-#{Size}-{TypeModel.SubType}-{voltage}"; }
        set { _sizeTag = value; }
    }

    public string Category { get; set; }

    private string _source;
    public string Source
    {
        get { return _source; }

        set { _source = value; }
    }
    public string Destination { get; set; }
    public int LoadId { get; set; }
    public string LoadType { get; set; }
    public ICableUser Load { get; set; }

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


            Is1C = _type.Contains("1C") ? true : false;

            var cmd = new UndoCommandDetail { Item = this, PropName = nameof(TypeModel), OldValue = oldValue, NewValue = _typeModel };
            UndoManager.AddUndoCommand(cmd);
            
            if (DaManager.GettingRecords == false) {
                if (UsageType==CableUsageTypes.Power.ToString()) {
                    SetTypeProperties();
                    AutoSizeAsync();
                }
                if (CableManager.IsUpdatingPowerCables == false) {

                    //CableManager.AddAndUpdateLoadPowerComponentCablesAsync(Load as IPowerConsumer, ScenarioManager.ListManager);
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
          
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(QtyParallel), OldValue = oldValue, NewValue = _qtyParallel };
                UndoManager.AddUndoCommand(cmd);
            }
            if (DaManager.GettingRecords == false && _calculating == false) {
                _calculatingQty = true;
                CalculateAmpacity(this.Load);
                _calculatingQty = false;

            }
            OnPropertyUpdated();
        }
    }
    private bool _calculatingQty;
    private bool _calculating;
    private string _size;

    public string Size
    {
        get { return _size; }
        set
        {
            var oldValue = _size;
            _size = value;

            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Size), OldValue = oldValue, NewValue = _size };
                UndoManager.AddUndoCommand(cmd);
            }

            UndoManager.AddUndoCommand(this, nameof(Size), oldValue, _size);
            OnPropertyUpdated();
        }
    }
    public bool IsValidSize { get; set; }
    public bool Is1C { get; set; }

    public double BaseAmps { get; set; }

    private double _spacing;

    public double Spacing
    {
        get { return _spacing; }
        set
        {
            var oldValue = _spacing;
            _spacing = value;

            if (DaManager.GettingRecords == false) {
                _calculating = true;
                Derating = CableManager.CableSizer.SetDerating(this);
                CalculateAmpacity(Load);
                _calculating = false;
            }

            UndoManager.AddUndoCommand(this, nameof(Spacing), oldValue, _spacing);
            OnPropertyUpdated();
        }
    }

    public double _length;
    public double Length 
    { 
        get { return _length; }
        set
        {
            var oldValue = _length;
            _length = value;
            CableManager.CableSizer.SetVoltageDrop(this);
            UndoManager.AddUndoCommand(this, nameof(Length), oldValue, _length);
        }
    }

    public double VoltageDrop { get; set; }
    public double VoltageDropPercentage { get; set; }
    public double MaxVoltageDropPercentage { get; set; }

    public double Derating { get; set; }

    public double Derating5A { get; set; }
    public double Derating5C { get; set; }
    public string DeratingToolTip
    {
        get
        {
            return $" {Derating5A} x {Derating5C} = {Derating} \n" +
                $"Table 5A x Table 5C";
        }
    }


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
            if (Load != null) {
                return $"OCDP Trip = {Load.PdSizeTrip} A";

            }
            else {
                return "OCDP Trip = xx A";
            }
        }

    }

    public double RequiredSizingAmps { get; set; }

    private bool _outdoor;

    public bool IsOutdoor
    {
        get { return _outdoor; }
        set
        {
            var oldValue = _outdoor;
            _outdoor = value;

            if (DaManager.GettingRecords == false) {
                _calculating = true;
                AutoSize();
                _calculating = false;
            }

            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(IsOutdoor), OldValue = oldValue, NewValue = _outdoor };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }

    }

    private string _installationType = EdtSettings.CableInstallationType;

    public string InstallationType
    {
        get { return _installationType; }
        set
        {
            if (value == null || value == "") return;

            var oldValue = _installationType;
            _installationType = value;

            if (DaManager.GettingRecords == false) {
                _calculating = true;
                if (UsageType == CableUsageTypes.Power.ToString()) {
                    AutoSize();

                }
                _calculating = false;
            }

            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(InstallationType), OldValue = oldValue, NewValue = _installationType };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();

        }
    }

    public string AmpacityTable { get; set; }
    public string InstallationDiagram { get; set; }

    





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
    public void SetTagging(ICableUser load)
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

        var cableVoltageClass = DataTableManager.GetCableVoltageClass(load.Voltage);

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
    public void SetTypeProperties()
    {
        if (UsageType==CableUsageTypes.Power.ToString()) {
            VoltageClass = TypeManager.PowerCableTypes.FirstOrDefault(c => c.Type == Type).VoltageClass;
            ConductorQty = TypeManager.PowerCableTypes.FirstOrDefault(c => c.Type == Type).ConductorQty;
            AmpacityTable = CableManager.CableSizer.GetAmpacityTable(this);
        }

    }
    public void SetCableParameters(ICableUser load)
    {
        Load = load;
        SetTagging(load);
        AssignOwner(load);

        VoltageClass = TypeManager.PowerCableTypes.FirstOrDefault(c => c.Type == Type).VoltageClass;
        ConductorQty = TypeManager.PowerCableTypes.FirstOrDefault(c => c.Type == Type).ConductorQty;

        CableManager.CableSizer.SetDerating(this);
        GetRequiredAmps(load);

        RequiredSizingAmps = GetRequiredSizingAmps();
        Spacing = CableManager.CableSizer.GetDefaultCableSpacing(this);
        AmpacityTable = CableManager.CableSizer.GetAmpacityTable(this);
        OnPropertyUpdated();
    }
    public double GetRequiredAmps(ICableUser load)
    {
        if (load == null) return 99999;

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
        //ValidateCableSize(this);
        return RequiredAmps;
    }
    public double GetRequiredSizingAmps()
    {
        //Debug.WriteLine("GetRequiredSizingAmps");

        Derating = CableManager.CableSizer.SetDerating(this);
        RequiredSizingAmps = GetRequiredAmps(Load) / Derating;
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

    string ampsColumn = "Amps75";

    public async Task AutoSizeAsync()
    {
        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            AutoSize();
        }));
    }
    public void AutoSize()
    {
        UndoManager.CanAdd = false;
        //_calculating = true;
        SetTypeProperties();
        RequiredSizingAmps = GetRequiredSizingAmps();
        Spacing = CableManager.CableSizer.GetDefaultCableSpacing(this);
        AmpacityTable = CableManager.CableSizer.GetAmpacityTable(this);
        InstallationDiagram = "";


        if (InstallationType == GlobalConfig.CableInstallationType_LadderTray) {
            GetCableQtySize_ForLadderTray(this, ampsColumn);
        }

        else if (InstallationType == GlobalConfig.CableInstallationType_DirectBuried
              || InstallationType == GlobalConfig.CableInstallationType_RacewayConduit) {

            CableQtySize_DirectBuriedOrRaceWayConduit(this, ampsColumn);
        }
        CalculateAmpacity(Load);
        OnPropertyUpdated();
        UndoManager.CanAdd = true;
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
            if (cableQty < maxCableQtyTray) {
                if (cablesWithHigherAmpsInProject.Rows.Count > 0) {
                    cable.Derating = CableManager.CableSizer.SetDerating(cable);
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
    int maxCableQtyTray = 25;
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
            if (cableQty < maxCableQtyRaceWay) {
                if (cablesWithHigherAmpsInProject.Rows.Count > 0) {
                    cable.Derating = CableManager.CableSizer.SetDerating(cable);
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
    int maxCableQtyRaceWay = 25;
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
        IsValidSize = true;
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
        ValidateCableSize(this);
        CableManager.CableSizer.SetVoltageDrop(this);
        OnPropertyUpdated();
        _calculating = false;
        return output;

    }
    private void CalculateAmpacity_LadderTray(ICable cable, string ampsColumn)
    {
        cable.Derating = CableManager.CableSizer.SetDerating(cable);

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
            SetCableInvalid(this);
        }
    }
    private void CalculateAmpacity_DirectBuriedOrRaceWayConduit(ICable cable, string ampsColumn)
    {
        cable.Derating = CableManager.CableSizer.SetDerating(cable);


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
            SetCableInvalid(this);
        }
    }

    public void ValidateCableSize(ICable cable)
    {
        CableManager.CableSizer.SetDerating(this);
        GetRequiredAmps(Load);
        if (cable.RequiredAmps > cable.DeratedAmps) {
            cable.SetCableInvalid(this);
        }
        else {
            cable.IsValidSize = true;
        }
    }
    public void SetCableInvalid(ICable cable)
    {
        cable.IsValidSize = false;
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

