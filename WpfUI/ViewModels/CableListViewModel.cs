using EDTLibrary;
using EDTLibrary.Models.Cables;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class CableListViewModel : ViewModelBase
    {
        private ListManager _listManager;

        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }

        public CableListViewModel(ListManager listManager)
        {
            _listManager = listManager;
        }

    }
}
