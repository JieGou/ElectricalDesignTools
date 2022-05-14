using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.ProjectSettings;
using System.Windows;
using WpfUI.Services;
using WpfUI.ViewModels;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public App()
        {

        }

        protected override void OnStartup(StartupEventArgs e) {
            DaManager daManager = new DaManager();
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager);
            TypeManager typeManager = new TypeManager();
            EdtSettings edtSettings = new EdtSettings();

            MainWindow = new MainWindow() { 
                DataContext = new MainViewModel(startupService, listManager, typeManager, edtSettings, "NewInstance") 
                //DataContext = new MainViewModel(_navigationStore) 
            };

            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
