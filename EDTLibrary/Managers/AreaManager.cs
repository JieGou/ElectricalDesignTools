using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Areas;
public class AreaManager
{
    public static void UpdateArea(IEquipment caller, IArea newArea, IArea oldArea)
    {
        if (DaManager.GettingRecords == false && newArea != oldArea) {
            if (oldArea != null) {
                oldArea.PropertyChanged -= caller.OnAreaPropertiesChanged;
            }
            newArea.PropertyChanged += caller.OnAreaPropertiesChanged;
            caller.Area = newArea;
            caller.AreaId = newArea.Id;
            caller.NemaRating = newArea.NemaRating;
            caller.AreaClassification = newArea.AreaClassification;
        }
    }
    public static bool IsAreaInUse(AreaModel area, ListManager listManager)
    {
        listManager.CreateEquipmentList();
        foreach (var item in listManager.EqList) {
            if (item.Area.Id== area.Id) {
                return true;
            }
        }
        return false;
    }
}
