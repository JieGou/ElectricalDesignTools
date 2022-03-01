using EDTLibrary.Models.Loads;

namespace EDTLibrary.Models.Cables
{
    public interface ICableSizer
    {
        string GetAmpacityTable(IPowerCable cable);
        string GetDefaultCableType(IPowerConsumer load);
    }
}