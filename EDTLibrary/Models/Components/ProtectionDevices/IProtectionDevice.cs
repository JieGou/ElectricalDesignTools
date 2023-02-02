namespace EDTLibrary.Models.Components.ProtectionDevices;

public interface IProtectionDevice : IComponentEdt
{
    bool IsStandAlone { get; set; }

    public string StarterSize
    {
        get;
        set;
    }

    double SCCA { get; set; }
    double SCCR { get; set; }

    double AIC { get; set; }
}