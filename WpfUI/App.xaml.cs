using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Windows;
using WpfUI.Helpers;
using WpfUI.Services;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Home;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        ObservableCollection<PreviousProject> PreviousProjects { get; set; } = new ObservableCollection<PreviousProject>();
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njk5OTgxQDMyMzAyZTMyMmUzMGJCSWJsOE44RWhRcWhXSmFwdS96NlBvRlBqREtOcHZKU0xNMGtiemNCVWM9");
        }
        static void MainHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show($"Error: { e.Message}\n\n" +
                        $"Application Closed? - {args.IsTerminating.ToString()}", 
                        "Fatal Error");
        }

        protected override void OnStartup(StartupEventArgs e) {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MainHandler);

            var splashImage = "ContentFiles\\pd.png";
            SplashScreen splashScreen = new SplashScreen(splashImage);

            splashScreen.Show(false, true); // autoClose, topMost

            DaManager daManager = new DaManager();
            ListManager listManager = new ListManager();
            StartupService startupService = new StartupService(listManager, PreviousProjects);
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
