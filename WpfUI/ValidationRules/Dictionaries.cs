using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ValidationRules
{
    public class Dictionaries
    {
        public static Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();



        public static void CreateDteqDict(ObservableCollection<DteqModel> dteqOc) {
            dteqDict.Clear();
            dteqDict = dteqOc.ToList().Distinct().ToDictionary(x => x.Tag);
        }
    }
}
