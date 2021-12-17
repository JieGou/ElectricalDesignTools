using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;

namespace WpfUI.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
       
        public ICommand? NavigateStartupCommand { get; }
        public ICommand? NavigateProjectSettingsCommand { get; }
        public ICommand? NavigateEquipmentCommand { get; }
        public ICommand? NavigateCablesCommand { get; }
        public ICommand? NavigateLibraryCommand { get; }

        public NavigationBarViewModel()
        {
            NavigateProjectSettingsCommand = new RelayCommand(NavigateProjectSettings);
        }

        private void NavigateProjectSettings()
        {

        }

        public NavigationBarViewModel(NavigationService<StartupViewModel> startupNavigationService ,
            NavigationService<ProjectSettingsViewModel> projectSettingsNavigationService,
            NavigationService<EquipmentViewModel> eqNavigationService
            )
        {
            NavigateStartupCommand = new NavigateCommand<StartupViewModel>(startupNavigationService);
            NavigateProjectSettingsCommand = new NavigateCommand<ProjectSettingsViewModel>(projectSettingsNavigationService);
            NavigateEquipmentCommand = new NavigateCommand<EquipmentViewModel>(eqNavigationService);
        }

        
    }

}
