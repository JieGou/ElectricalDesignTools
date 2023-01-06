using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Raceways;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EDTLibrary.Models.Cables;

public interface ICable
{
    string AmpacityTable { get; set; }
    ICommand AutoSizeCommand { get; }
    double BaseAmps { get; set; }
    string Category { get; set; }
    int ConductorQty { get; set; }
    double DeratedAmps { get; set; }
    string DeratedAmpsToolTip { get; }
    double Derating { get; set; }
    double Derating5A { get; set; }
    double Derating5C { get; set; }
    string DeratingToolTip { get; }
    string Destination { get; set; }
    IEquipment DestinationModel { get; set; }
    double HeatLoss { get; set; }
    int Id { get; set; }
    string InstallationDiagram { get; set; }
    string InstallationType { get; set; }
    double InsulationPercentage { get; set; }
    bool Is1C { get; set; }
    bool IsOutdoor { get; set; }
    bool IsValidSize { get; set; }
    string InvalidCableMessage { get; set; }
    double Length { get; set; }
    ICableUser Load { get; set; }
    int LoadId { get; set; }
    string LoadType { get; set; }
    double MaxVoltageDropPercentage { get; set; }
    int OwnerId { get; set; }
    string OwnerType { get; set; }
    int QtyParallel { get; set; }
    double RequiredAmps { get; set; }
    string RequiredAmpsToolTip { get; }
    double RequiredSizingAmps { get; set; }
    string Size { get; set; }
    List<string> SizeList { get; set; }
    string SizeTag { get; set; }
    string Source { get; set; }
    int SourceId { get; set; }
    string SourceType { get; set; }
    IEquipment SourceModel { get; set; }
    double Spacing { get; set; }
    string Tag { get; set; }
    string Type { get; set; }
    int TypeId { get; set; }
    ObservableCollection<RacewayRouteSegment> RacewaySegmentList { get; set; }
    List<CableTypeModel> TypeList { get; set; }
    CableTypeModel TypeModel { get; set; }

    string UsageType { get; set; }
    double VoltageRating { get; set; }
    double VoltageDrop { get; set; }
    double VoltageDropPercentage { get; set; }

    double Diameter { get; set; }
    string BondWireSize { get; set; }
    double WeightLbs1kFeet { get; set; }
    double WeightKgKm { get; set; }

    event EventHandler PropertyUpdated;

    void AssignOwner(ICableUser load);
    void AutoSizeAll();
    Task AutoSizeAllAsync();
    string CalculateAmpacity(ICableUser load);
    void CreateSizeList();
    void CreateTag();
    void CreateTypeList(ICableUser load);
    double GetRequiredAmps(ICableUser load);
    double GetRequiredSizingAmps();
    Task OnPropertyUpdated();
    string SelectCableType(double voltageClass, int conductorQty, double insulation, string usageType);
    void SetCableInvalid(ICable cable);
    void SetSizingParameters(ICableUser load);
    void SetSourceAndDestinationTags(ICableUser load);
    void SetTypeProperties();
    void ValidateCableSize(ICable cable);
}