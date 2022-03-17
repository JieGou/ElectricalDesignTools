using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.Validators
{
    public static class TagValidator
    {
        public static bool IsTagAvailable(string tag, ListManager listManger)
        {
            if (string.IsNullOrEmpty(tag)) {
                return true;
            }


            var areaTag = listManger.AreaList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var dteqTag = listManger.DteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var loadTag = listManger.LoadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());


            if (areaTag != null ||
                dteqTag != null ||
                loadTag != null) {
                return false;
            }

            return true;
        }
    }
}
