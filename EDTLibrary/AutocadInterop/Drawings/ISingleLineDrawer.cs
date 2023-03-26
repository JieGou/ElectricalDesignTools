using AutocadLibrary;
using EDTLibrary.AutocadInterop.Interop;
using EDTLibrary.Models.DistributionEquipment;

namespace EDTLibrary.Autocad.Interop;
public interface ISingleLineDrawer: IAcadDrawer
{
    string BlockSourceFolder { get; }
    void DrawSingleLine(IDteq mcc, double blockSpacing = 1.5);
}