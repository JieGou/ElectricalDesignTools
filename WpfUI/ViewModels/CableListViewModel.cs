using EDTLibrary;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class CableListViewModel : ViewModelBase
    {
        public ObservableCollection<CableModel> CableList { get; set; }


        public CableListViewModel()
        {
            //CableList = new ObservableCollection<CableModel>(ListManager.GetCableList());
        }


        public CableListViewModel(NavigationStore navigationStore)
        {
            CableList = new ObservableCollection<CableModel>(ListManager.GetCableList());
        }
    }
}
