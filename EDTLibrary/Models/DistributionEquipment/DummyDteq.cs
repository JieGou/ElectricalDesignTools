using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Calculations;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.DistributionEquipment;
public class DummyDteq : IDteq
{

    public bool IsAreaLocked
    {
        get { return _isAreaLocked; }
        set
        {
            _isAreaLocked = value;
            OnPropertyUpdated();

        }
    }
    private bool _isAreaLocked;

    public bool IsCalculationLocked
    {
        get { return _isCalculationLocked; }
        set
        {
            _isCalculationLocked = value;
            OnPropertyUpdated();
        }
    }
    private bool _isCalculationLocked;
    bool IsCalculating { get; set; }

    public bool IsMainLugsOnly { get; set; }
    public int LineVoltageTypeId { get; set; }
    public VoltageType LineVoltageType { get; set; }
    public double LineVoltage { get; set; }
    public int LoadVoltageTypeId { get; set; }
    public VoltageType LoadVoltageType { get; set; }
    public double LoadVoltage { get; set; }
    public ObservableCollection<IPowerConsumer> AssignedLoads { get; set; } = new ObservableCollection<IPowerConsumer>();
    public double SCCA { get; set; }
    public double SCCR { get; set; }
    public double LoadCableDerating { get; set; }
    public int ProtectionDeviceId { get; set; }
    public IProtectionDevice ProtectionDevice { get; set; }
    public int VoltageTypeId { get; set; }
    public VoltageType VoltageType { get; set; }
    public double PowerFactor { get; set; }
    public double ConnectedKva { get; set; }
    public double DemandKva { get; set; }
    public double DemandKw { get; set; }
    public double DemandKvar { get; set; }
    public double RunningAmps { get; set; }
    public int SequenceNumber { get; set; }
    public string PanelSide { get; set; }
    public int CircuitNumber { get; set; }
    public double Voltage { get; set; }
    public double Size { get; set; }
    public string Unit { get; set; }
    public double Fla { get; set; }
    public string FedFromTag { get; set; }
    public int FedFromId { get; set; }
    public string FedFromType { get; set; }
    public IDteq FedFrom { get; set; }
    public double AmpacityFactor { get; set; }
    public CableModel PowerCable { get; set; }
    public ObservableCollection<IComponentEdt> AuxComponents { get; set; } = new ObservableCollection<IComponentEdt>();
    public ObservableCollection<IComponentEdt> CctComponents { get; set; } = new ObservableCollection<IComponentEdt>();
    public bool StandAloneStarterBool { get; set; }
    public int StandAloneStarterId { get; set; }
    public bool DisconnectBool { get; set; }
    public int DisconnectId { get; set; }
    public bool LcsBool { get; set; }
    public ILocalControlStation Lcs { get; set; }
    public IComponentEdt StandAloneStarter { get; set; }
    public IComponentEdt Disconnect { get; set; }
    public IComponentEdt SelectedComponent { get; set; }
    public bool IsValid { get; set; }
    public bool IsSelected { get; set; }
    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public int AreaId { get; set; }
    public IArea Area { get; set; }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public double HeatLoss { get; set; }
    public double PercentLoaded { get; set; }
    bool IDteq.IsCalculating { get; set; }

    public event EventHandler<CalculateLoadingEventArgs> LoadingCalculated;
    public event EventHandler PropertyUpdated;
    public event EventHandler AreaChanged;

    public bool AddNewLoad(IPowerConsumer load)
    {
        if (load == null) return false;

        //check if load is already assigned
        var iLoad = AssignedLoads.FirstOrDefault(l => l == load);

        if (iLoad == null) {
            AssignedLoads.Add(load);
            if (load.ProtectionDevice != null) {
                load.ProtectionDevice.IsSelected = false;
                load.CctComponents.Remove(load.ProtectionDevice);
            }
            return true;
        }
        return false;
    }

    public void CalculateCableAmps()
    {
        throw new NotImplementedException();
    }
    public virtual double CalculateSCCA()
    {
        if (Tag == GlobalConfig.UtilityTag) {
            return 0;
        }
        else if (FedFrom == null) {
            return 0;
        }
        return FedFrom.SCCA;
    }
    public void CalculateLoading(string propertyName = "")
    {
        if (DaManager.Importing) return;

        if (LineVoltageType == null || LoadVoltageType == null) return;


        Voltage = LineVoltage;
        var dis = this;
        //Sums values from Assinged loads
        ConnectedKva = (from x in AssignedLoads select x.ConnectedKva).Sum();
        ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);

        DemandKva = (from x in AssignedLoads select x.DemandKva).Sum();
        DemandKva = Math.Round(DemandKva, GlobalConfig.SigFigs);

        DemandKw = (from x in AssignedLoads select x.DemandKw).Sum();
        DemandKw = Math.Round(DemandKw, GlobalConfig.SigFigs);

        DemandKvar = (from x in AssignedLoads select x.DemandKvar).Sum();
        DemandKvar = Math.Round(DemandKvar, GlobalConfig.SigFigs);

        PowerFactor = DemandKw / DemandKva;
        PowerFactor = Math.Round(PowerFactor, 2);

        RunningAmps = DemandKva * 1000 / LineVoltageType.Voltage / Math.Sqrt(LineVoltageType.Phase);
        RunningAmps = Math.Round(RunningAmps, GlobalConfig.SigFigs);

        //Full Load / Max operating Amps
        if (Unit == Units.kVA.ToString()) {
            Fla = Size * 1000 / LineVoltageType.Voltage / Math.Sqrt(LineVoltageType.Phase);
            Fla = Math.Round(Fla, GlobalConfig.SigFigs);

        }
        else if (Unit == Units.A.ToString()) {
            Fla = Size;
        }
        if (Fla > 99999) Fla = 99999;

        PercentLoaded = RunningAmps / Fla * 100;
        PercentLoaded = Math.Round(PercentLoaded, GlobalConfig.SigFigs);

        IsCalculating = false;

    }

    public bool CanAdd(IPowerConsumer load)
    {
        return true;
    }

    public void Validate()
    {
        return;
    }

    public void Create()
    {
        throw new NotImplementedException();
    }

    public void CreatePowerCable()
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public Task OnAreaChanged()
    {
        throw new NotImplementedException();
    }

    public void OnAssignedLoadReCalculated(object source, CalculateLoadingEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void OnLoadingCalculated(string propertyName = "")
    {
        throw new NotImplementedException();
    }

    public Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        throw new NotImplementedException();
    }

    public void RemoveAssignedLoad(IPowerConsumer load)
    {
        if (load != null) {
            AssignedLoads.Remove(load);
            load.LoadingCalculated -= OnAssignedLoadReCalculated;
        }
    }

    public void SetLoadProtectionDevice(IPowerConsumer load)
    {
    }

    public void SizePowerCable()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAreaProperties()
    {
        throw new NotImplementedException();
    }


    public void ValidateCableSizes()
    {
        throw new NotImplementedException();
    }
}
