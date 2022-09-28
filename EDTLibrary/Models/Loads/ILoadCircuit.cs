using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
public interface ILoadCircuit : ILoad
{
    double AmpacityFactor { get; set; }
    IArea Area { get; set; }
    string AreaClassification { get; set; }
    int AreaId { get; set; }
    ObservableCollection<IComponentEdt> AuxComponents { get; set; }
    string Category { get; set; }
    ObservableCollection<IComponentEdt> CctComponents { get; set; }
    double ConnectedKva { get; set; }
    double DemandKva { get; set; }
    double DemandKvar { get; set; }
    double DemandKw { get; set; }
    string Description { get; set; }
    IComponentEdt Disconnect { get; set; }
    bool DisconnectBool { get; set; }
    int DisconnectId { get; set; }
    IComponentEdt Drive { get; set; }
    bool DriveBool { get; set; }
    int DriveId { get; set; }
    double Efficiency { get; set; }
    IDteq FedFrom { get; set; }
    int FedFromId { get; set; }
    string FedFromTag { get; set; }
    string FedFromType { get; set; }
    double Fla { get; set; }
    double HeatLoss { get; set; }
    int Id { get; set; }
    ILocalControlStation Lcs { get; set; }
    bool LcsBool { get; set; }
    double LoadFactor { get; set; }
    string NemaRating { get; set; }
    string PanelSide { get; set; }
    double PdSizeFrame { get; set; }
    double PdSizeTrip { get; set; }
    string PdType { get; set; }
    CableModel PowerCable { get; set; }
    double PowerFactor { get; set; }
    double RunningAmps { get; set; }
    int SequenceNumber { get; set; }
    double Size { get; set; }
    double StarterSize { get; set; }
    string StarterType { get; set; }
    string Tag { get; set; }
    string Type { get; set; }
    string Unit { get; set; }
    double Voltage { get; set; }
    VoltageType VoltageType { get; set; }
    int VoltageTypeId { get; set; }

    event EventHandler AreaChanged;
    event EventHandler SpaceConverted;
    event EventHandler LoadingCalculated;
    event EventHandler PropertyUpdated;

    void CalculateCableAmps();
    void CalculateLoading();
    void CreatePowerCable();
    void MatchOwnerArea(object source, EventArgs e);
    Task OnAreaChanged();
    Task OnSpaceConverted();
    void OnLoadingCalculated();
    Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "");
    void SizePowerCable();
    Task UpdateAreaProperties();
}