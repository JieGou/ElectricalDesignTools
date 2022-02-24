using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Stores
{
    public class DictionaryStore
    {
        public static Dictionary<string, IDteq> dteqDict { get; set; } = new Dictionary<string, IDteq>();

        public static void CreateDteqDict(ObservableCollection<IDteq> dteqOc) {
            dteqDict.Clear();

            //Todo - error check existing values with foreach
            foreach (var dteq in dteqOc) {
                if (dteqDict.ContainsKey(dteq.Tag) == false) {
                    dteqDict.Add(dteq.Tag, dteq);
                }
            }
            //dteqDict = dteqOc.ToList().Distinct().ToDictionary(x => x.Tag);
        }
    }
}
