using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Stores
{
    public class ListStore
    {
        public ObservableCollection<DteqModel> DteqList { get; set; }
        public ObservableCollection<LoadModel> LoadList { get; set; }
        public ObservableCollection<PowerCableModel> CableList { get; set; }

    }
}
