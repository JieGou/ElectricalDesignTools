using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.Validators
{
    public static class TagAndNameValidator
    {
        public static bool IsTagAvailable(string tagToCheck, ListManager listManager)
        {
            if (string.IsNullOrEmpty(tagToCheck)) return true;
            if (listManager == null) return true; //for test data
            if (DaManager.GettingRecords == true) return true;                                                
            if (tagToCheck == GlobalConfig.Utility) return true;
            if (tagToCheck == GlobalConfig.Deleted) return true;
            if (tagToCheck == GlobalConfig.LargestMotor_StartLoad) return true;

            listManager.CreateEquipmentList();
            var tag = listManager.EqList.FirstOrDefault(eq => eq.Tag.ToLower() == tagToCheck.ToLower());

            if (tag != null) return false;

            return true;
        }

        public static bool IsNameAvailable(string name, ListManager listManager)
        {
            if (string.IsNullOrEmpty(name)) {
                return true;
            }

            var areaNAme = listManager.AreaList.FirstOrDefault(t => t.Name.ToLower() == name.ToLower());

            if (areaNAme != null) {
                return false;
            }

            return true;
        }
    }
}
