using EDTLibrary.Models;
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
        public static Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();

        public static void CreateDteqDict(ObservableCollection<DteqModel> dteqOc) {
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
