using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Calculations;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
internal class DummyLoad : ILoad
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
    public bool IsValid { get; set; } = true;
    public void Validate()
    {

        IsValid = true;
        OnPropertyUpdated();

        return;
    }

    public double SCCA { get; set; }
    public double SCCR { get; set; }

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
    public ObservableCollection<IComponentEdt> AuxComponents { get; set; }
    public ObservableCollection<IComponentEdt> CctComponents { get; set; }
    public bool StandAloneStarterBool { get; set; }
    public int StandAloneStarterId { get; set; }
    public bool DisconnectBool { get; set; }
    public int DisconnectId { get; set; }
    public bool LcsBool { get; set; }
    public ILocalControlStation Lcs { get; set; }
    public IComponentEdt StandAloneStarter { get; set; }
    public IComponentEdt Disconnect { get; set; }
    public IComponentEdt SelectedComponent { get; set; }
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
    public double DemandFactor { get; set; }
    public double Efficiency { get; set; }

    public event EventHandler<CalculateLoadingEventArgs> LoadingCalculated;
    public event EventHandler PropertyUpdated;
    public event EventHandler AreaChanged;

    public void CalculateCableAmps()
    {
    }

    public void CalculateLoading(string propertyName = "")
    {
    }

    public void CreatePowerCable()
    {
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
    }

    public Task OnAreaChanged()
    {
        return null;
    }

    public void OnLoadingCalculated(string propertyName = "")
    {
    }

    public Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        return null;

    }

    public void SizePowerCable()
    {
    }

    public Task UpdateAreaProperties()
    {
        return null;
    }

    public void ValidateCableSizes()
    {

    }
}
