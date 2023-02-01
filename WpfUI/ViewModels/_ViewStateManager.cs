using EDTLibrary.DataAccess;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDTLibrary.DistributionControl;
using WpfUI.Views.Electrical;

namespace WpfUI.ViewModels;
public class _ViewStateManager
{
    public static void OnFedFromUpdated(object source, EventArgs e)
    {
        OnElectricalViewUpdated();
    }

    public static void OnLoadDeleted(object source, EventArgs e)
    {
        OnElectricalViewUpdated();
    }

    public static event EventHandler ElectricalViewUpdate;
    public static void OnElectricalViewUpdated()
    {
        if (ElectricalViewUpdate != null) {
            ElectricalViewUpdate(nameof(FedFromManager), EventArgs.Empty);
        }
    }
}
