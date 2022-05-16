using EDTLibrary.DataAccess;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables;
internal class CableManager
{
    public static async Task DeletePowerCableAsync(IPowerConsumer powerCableUser, ListManager listManager)
    {
        //TODO - Move to Cable Manager

        if (powerCableUser.PowerCable != null) {
            int cableId = powerCableUser.PowerCable.Id;
            DaManager.prjDb.DeleteRecord(GlobalConfig.PowerCableTable, cableId); //await
            listManager.CableList.Remove(powerCableUser.PowerCable);
        }
        return;
    }
}
