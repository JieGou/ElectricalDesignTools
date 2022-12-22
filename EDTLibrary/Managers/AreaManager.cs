using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
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
            UndoManager.CanAdd = false;
            caller.AreaId = newArea.Id;
            caller.NemaRating = newArea.NemaRating;
            caller.AreaClassification = newArea.AreaClassification;

            if (caller is LoadModel) {
                var load = (LoadModel)caller;
                foreach (var component in load.CctComponents) {
                    if (component.SubType != CctComponentSubTypes.DefaultDrive.ToString()) {
                        component.Area = newArea;
                    }
                    if (load.LcsBool == true) {
                        load.Lcs.Area = newArea;
                    }
                }
            }

        }
    }
    public static bool IsAreaInUse(AreaModel area, ListManager listManager)
    {
        listManager.CreateEquipmentList();
        foreach (var item in listManager.EqList) {
            if (item.Area.Id == area.Id) {
                return true;
            }
        }
        return false;
    }


}
