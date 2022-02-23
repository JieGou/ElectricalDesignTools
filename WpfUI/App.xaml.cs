using EDTLibrary;
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

        }

        protected override void OnStartup(StartupEventArgs e) {

            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);

            MainWindow = new MainWindow() { 
                DataContext = new MainViewModel(startupService, listManager, "main") 
                //DataContext = new MainViewModel(_navigationStore) 
            };

            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
