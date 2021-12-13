using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;

namespace WpfUI.ViewModels
{
    internal class NavigationBarViewModel : ViewModelBase
    {
       
        public ICommand? NavigateProjectSettingsCommand { get; }
        public ICommand? NavigateEquipmentCommand { get; }
        public ICommand? NavigateCablesCommand { get; }
        public ICommand? NavigateLibraryCommand { get; }

        public NavigationBarViewModel()
        {
            //NavigateEquipmentCommand = new NavigateCommand<EquipmentViewModel>
        }

    }

}
