using EDTLibrary.AutocadInterop.Interop;
using EDTLibrary.Models.DistributionEquipment;

namespace EDTLibrary.Autocad.Interop;
public interface IPanelScheduleDrawer: IAcadDrawer
{
    string BlockSourceFolder { get; }

    void DrawPanelSchedule(IDteq dteq, double blockSpacing = 1.5);
}