using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        public BaseViewModel CurrentViewModel { get;  }

        public MainViewModel() {

            //Initialze Db Connections First
            DbManager.SetProjectDb(Settings.Default.ProjectDb);
            DbManager.SetLibraryDb(Settings.Default.LibraryDb);

            CurrentViewModel = new EqViewModel();

        }
    }
}
 