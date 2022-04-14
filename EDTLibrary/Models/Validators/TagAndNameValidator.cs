using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.Validators
{
    public static class TagAndNameValidator
    {
        public static bool IsTagAvailable(string tag, ListManager listManager)
        {
            if (string.IsNullOrEmpty(tag)) {
                return true;
            }

            var areaTag = listManager.AreaList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var dteqTag = listManager.DteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var loadTag = listManager.LoadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());

            if (areaTag != null ||
                dteqTag != null ||
                loadTag != null) {
                return false;
            }

            return true;
        }

        public static bool IsNameAvailable(string name, ListManager listManager)
        {
            if (string.IsNullOrEmpty(name)) {
                return true;
            }

            var areaTag = listManager.AreaList.FirstOrDefault(t => t.Tag.ToLower() == name.ToLower());
        
            if (areaTag != null) {
                return false;
            }

            return true;
        }
    }
}
