using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Areas;
public class AreaFactory
{
    public static void CreateArea(ListManager listManager, object areaToAddObject)
    {
        AreaToAddValidator areaToAdd = (AreaToAddValidator)areaToAddObject;
        AreaModel newArea = new AreaModel();
        try {
            bool newAreaIsValid = areaToAdd.IsValid();
            var errors = areaToAdd._errorDict;
            if (newAreaIsValid) {

                newArea.Tag = areaToAdd.Tag;
                newArea.DisplayTag = areaToAdd.DisplayTag;
                newArea.Name = areaToAdd.Name;
                newArea.Description = areaToAdd.Description;
                newArea.AreaCategory = areaToAdd.AreaCategory;
                newArea.AreaClassification = areaToAdd.AreaClassification;
                newArea.MinTemp = double.Parse(areaToAdd.MinTemp);
                newArea.MaxTemp = double.Parse(areaToAdd.MaxTemp);
                newArea.NemaRating = areaToAdd.NemaRating;


                newArea.Id = DaManager.prjDb.InsertRecordGetId(newArea, GlobalConfig.AreaTable, NoSaveLists.AreaNoSaveList);

                listManager.AreaList.Add(newArea);
                
            }
        }
        catch (Exception ex) {
            ErrorHelper.SendExeptionMessage(ex);
        }
    }

}
