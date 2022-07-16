using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.ProjectSettings;
using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;
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

            var splashImage = "ContentFiles\\pd.png";
            SplashScreen splashScreen = new SplashScreen(splashImage);
            splashScreen.Show(false, true); // autoClose, topMost

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

            // TimeSpan Values:     days, hours, minutes, seconds, milliseconds.
            TimeSpan FadeTimeout = new TimeSpan(0,0,0,0,500);
            splashScreen.Close(TimeSpan.FromMilliseconds(150));

            base.OnStartup(e);
           
        }
    }
}
