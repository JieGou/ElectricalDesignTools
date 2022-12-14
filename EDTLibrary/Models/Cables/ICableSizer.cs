using EDTLibrary.LibraryData.Cables;
using EDTLibrary.Models.Loads;

namespace EDTLibrary.Models.Cables
{
    public interface ICableSizer
    {
        bool IsUsingStandardSizingTable(ICable cable);
        string GetDefaultCableType(IPowerConsumer load);
        int GetDefaultCableTypeId(IPowerConsumer load);
        CableTypeModel GetDefaultCableTypeModel(IPowerConsumer load);
        double GetDefaultCableSpacing(ICable cable);
        string GetAmpacityTable(ICable cable, bool checkSizeCutoff = true);
        double SetDerating(ICable cable);
        void SetVoltageDrop(ICable cable);
    }
}