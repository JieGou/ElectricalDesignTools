using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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

        StartupService _startupService;
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


        protected override void OnStartup(StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MainHandler);

            var splashImage = "ContentFiles\\pd.png";
            SplashScreen splashScreen = new SplashScreen(splashImage);


            splashScreen.Show(false, true); // autoClose, topMost

            DaManager daManager = new DaManager();
            ListManager listManager = new ListManager();
            _startupService = new StartupService(listManager, PreviousProjects);
            TypeManager typeManager = new TypeManager();
            EdtSettings edtSettings = new EdtSettings();


            DeserializeRecentProjects();

            MainWindow = new MainWindow() {
                DataContext = new MainViewModel(_startupService, listManager, typeManager, edtSettings, "NewInstance")
                //DataContext = new MainViewModel(_navigationStore) 
            };
            MainWindow.Show();

            // TimeSpan Values:     days, hours, minutes, seconds, milliseconds.
            TimeSpan FadeTimeout = new TimeSpan(0, 0, 0, 0, 500);
            splashScreen.Close(TimeSpan.FromMilliseconds(150));

            base.OnStartup(e);

        }

        private void DeserializeRecentProjects()
        {
            try {
                var ppDtos = new ObservableCollection<PreviousProjectDto>();
                string recentProjets = "RecentProjects.bin";

                //Deserialize RecentProject to DTO's
                if (File.Exists(recentProjets)) {
                    using (Stream stream = File.Open(recentProjets, FileMode.Open)) {
                        BinaryFormatter bin = new BinaryFormatter();

                        ppDtos = (ObservableCollection<PreviousProjectDto>)bin.Deserialize(stream);

                        foreach (var project in ppDtos) {
                            PreviousProjects.Add(new PreviousProject(_startupService, project.Project));
                        }
                    }

                }
            }
            catch (IOException ex) {
               ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }
}
