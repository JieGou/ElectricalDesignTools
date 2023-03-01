using AutocadLibrary;
using EDTLibrary.AutocadInterop.Interop;
using EDTLibrary.Models.DistributionEquipment;

namespace EDTLibrary.Autocad.Interop;
public interface ISingleLineDrawer: IAcadDrawer
{
    string BlockSourceFolder { get; }
    void DrawMccSingleLine(IDteq mcc, double blockSpacing = 1.5);
}