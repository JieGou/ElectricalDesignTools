using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfUI.Validators
{
    public static class TagValidator
    {
        public static bool IsTagAvailable(string tag, ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList)
        {
            if (tag == null) {
                return true;
            }
            var dteqTag = dteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var loadTag = loadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());

            if (dteqTag != null ||
                loadTag != null) {
                return false;
            }

            return true;
        }
    }
}
