using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EDTLibrary.Managers;
public class DteqManager
{
    public static void SetPd(DistributionEquipment dteq)
    {

        //dteq.PdType = EdtSettings.DteqDefaultPdTypeLV;
        dteq.PdSizeFrame = DataTableSearcher.GetBreakerFrame(dteq);
        dteq.PdSizeTrip = DataTableSearcher.GetBreakerTrip(dteq);

    }

    public static void DeleteDteq(IDteq dteq, ListManager listManager)
    {
        try {

            IDteq dteqToDelete = DteqFactory.Recast(dteq);
            if (dteqToDelete != null) {
                //children first

                dteqToDelete.Tag = GlobalConfig.Deleted;
                listManager.UnregisterDteqFromLoadEvents(dteqToDelete);
                CableManager.DeletePowerCableAsync(dteqToDelete, listManager);
                DistributionManager.RetagLoadsOfDeleted(dteqToDelete);

                if (dteqToDelete.FedFrom != null) {
                    dteqToDelete.FedFrom.AssignedLoads.Remove(dteqToDelete);
                }
                listManager.DeleteDteq(dteqToDelete);
                DaManager.DeleteDteq(dteqToDelete);
                dteqToDelete.Delete();
            }
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message);
        }
        return;
    }
}
