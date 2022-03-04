using EDTLibrary.Models.Loads;

namespace EDTLibrary.Models.Cables
{
    public interface ICableSizer
    {
        string GetDefaultCableType(IPowerConsumer load);
        double GetDefaultCableSpacing(IPowerCable cable);
        string GetAmpacityTable(IPowerCable cable);
        double GetDerating(IPowerCable cable);

    }
}