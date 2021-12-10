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
        public static Dictionary<string, DteqModel> dteqDict { get; set; }



        public static void CreateDteqDict(ObservableCollection<DteqModel> dteqOc) {
            dteqDict = dteqOc.ToList().ToDictionary(x => x.Tag);
        }
    }
}
