using EDTLibrary.Models.Loads;

namespace EDTLibrary.Models.Cables
{
    public interface ICableSizer
    {
        string GetDefaultCableType(IPowerConsumer load);
        double GetDefaultCableSpacing(ICable cable);
        string GetAmpacityTable(ICable cable);
        double GetDerating(ICable cable);
        double GetVoltageDrop(ICable cable);
    }
}