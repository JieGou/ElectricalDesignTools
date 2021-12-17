using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ViewModels;
using WpfUI.Views;

namespace WpfUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private readonly ProjectFileStore _projectFileStore;
        private readonly NavigationStore _navigationStore;
        
        private readonly NavigationService<EquipmentViewModel> _equipmentNavigationService;

        private readonly NavigationBarViewModel _navigationBarViewModel;

        private readonly StartupViewModel _startupViewModel;
        private readonly ProjectSettingsViewModel _projectSettingsViewModel;
        private readonly EquipmentViewModel _equipmentViewModel;



        public App()
        {
            _projectFileStore = new ProjectFileStore();
            _navigationStore = new NavigationStore();
            
            _equipmentNavigationService = new NavigationService<EquipmentViewModel>(_navigationStore, () => _equipmentViewModel);

            _navigationBarViewModel = new NavigationBarViewModel(
                CreateStartupNavigationService(),
                CreateProjectSettingsNavigationService(),
                CreateEquipmentNavigationService());


            _startupViewModel = new StartupViewModel(_navigationBarViewModel, _projectFileStore, CreateProjectSettingsNavigationService());
            _projectSettingsViewModel = new ProjectSettingsViewModel(_navigationBarViewModel, _projectFileStore, CreateStartupNavigationService());
            _equipmentViewModel = new EquipmentViewModel(_navigationBarViewModel, _navigationStore);

        }

        protected override void OnStartup(StartupEventArgs e) {

            NavigationService<StartupViewModel> startupNavigationService = CreateStartupNavigationService();
            startupNavigationService.Navigate();

            MainWindow = new MainWindow() { 
                DataContext = new MainViewModel() 
                //DataContext = new MainViewModel(_navigationStore) 
            };

            MainWindow.Show();
            base.OnStartup(e);
        }

        private NavigationService<StartupViewModel> CreateStartupNavigationService()
        {
            return new NavigationService<StartupViewModel>(_navigationStore,()=> _startupViewModel);
        }

        private NavigationService<ProjectSettingsViewModel> CreateProjectSettingsNavigationService()
        {
            return new NavigationService<ProjectSettingsViewModel>(_navigationStore, () => _projectSettingsViewModel);
        }
        private NavigationService<EquipmentViewModel> CreateEquipmentNavigationService()
        {
            return new NavigationService<EquipmentViewModel>(_navigationStore, () => _equipmentViewModel);
        }
    }
}
