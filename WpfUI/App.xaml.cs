using EDTLibrary;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.ProjectSettings;
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

        public App()
        {

        }

        protected override void OnStartup(StartupEventArgs e) {

            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);
            TypeManager typeManager = new TypeManager();
            EdtSettings edtSettings = new EdtSettings();

            MainWindow = new MainWindow() { 
                DataContext = new MainViewModel(startupService, listManager, typeManager, edtSettings, "dev") 
                //DataContext = new MainViewModel(_navigationStore) 
            };

            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
