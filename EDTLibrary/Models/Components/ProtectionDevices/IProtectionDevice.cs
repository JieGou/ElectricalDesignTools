namespace EDTLibrary.Models.Components.ProtectionDevices;

public interface IProtectionDevice : IComponentEdt
{
    bool IsStandAlone { get; set; }
}