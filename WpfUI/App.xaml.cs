using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Settings;
using Firebase.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVMEssentials.Services;
using MVVMEssentials.Stores;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfUI._Authentication;
using WpfUI._Authentication.Stores;
using WpfUI._Authentication.ViewModels;
using WpfUI.Helpers;
using WpfUI.Services;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Home;
using MainViewModel = WpfUI.ViewModels.MainViewModel;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        StartupService _startupService;
        private IHost _host;

        ObservableCollection<PreviousProject> PreviousProjects { get; set; } = new ObservableCollection<PreviousProject>();
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Njk5OTgxQDMyMzAyZTMyMmUzMGJCSWJsOE44RWhRcWhXSmFwdS96NlBvRlBqREtOcHZKU0xNMGtiemNCVWM9");


            _host = Host
           .CreateDefaultBuilder()
           .ConfigureServices((context, service) => {

               string firebaseApiKey = context.Configuration.GetValue<string>("FIREBASE_API_KEY");

               service.AddSingleton(new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey)));

               service.AddSingleton<NavigationStore>();

               service.AddSingleton<ModalNavigationStore>();

               service.AddSingleton<AuthenticationStore>();

               service.AddSingleton<MVVMEssentials.ViewModels.MainViewModel>();



               service.AddSingleton<NavigationService<RegisterViewModel>>(
                   (services) => new NavigationService<RegisterViewModel>(
                       services.GetRequiredService<NavigationStore>(),
                       () => new RegisterViewModel(
                           services.GetRequiredService<FirebaseAuthProvider>(),
                           services.GetRequiredService<NavigationService<LoginViewModel>>())));


               service.AddSingleton<NavigationService<LoginViewModel>>(
                   (services) => new NavigationService<LoginViewModel>(
                       services.GetRequiredService<NavigationStore>(),
                       () => new LoginViewModel(
                           services.GetRequiredService<AuthenticationStore>(),
                           services.GetRequiredService<NavigationService<RegisterViewModel>>(),
                           services.GetRequiredService<AuthenticationMainWindow>())));


               service.AddSingleton<AuthenticationMainWindow>((services) => new AuthenticationMainWindow() {
                   DataContext = services.GetRequiredService<MVVMEssentials.ViewModels.MainViewModel>()

               });




               service.AddSingleton<StartupService>();
               service.AddSingleton<TypeManager>();
               service.AddSingleton<EdtProjectSettings>();

               service.AddSingleton<MainViewModel>((services) => new MainViewModel(
                   services.GetRequiredService<AuthenticationStore>(),
                   services.GetRequiredService<StartupService>(),
                   services.GetRequiredService<TypeManager>(),
                   services.GetRequiredService<EdtProjectSettings>()));

           })
               .Build();

        }
        static void MainHandler(object sender, UnhandledExceptionEventArgs args)
        {
            string message = "An unhandled error has occured";
            if (args.IsTerminating) {
                message = "An unhandled error has occured and the application will now close";
            }

            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show($"{message} \n\n" +
                            
                            //$"Error: {e.Message}",

                            "Unhandled Error");
        }


        protected override async void OnStartup(StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MainHandler);

            var splashImage = "ContentFiles\\pd.png";
            SplashScreen splashScreen = new SplashScreen(splashImage);


            splashScreen.Show(false, true); // autoClose, topMost

            DaManager daManager = new DaManager();
            ListManager listManager = new ListManager();
            _startupService = new StartupService(listManager, PreviousProjects);
            TypeManager typeManager = new TypeManager();
            EdtProjectSettings edtSettings = new EdtProjectSettings();


            DeserializeRecentProjects();
            var _authenticationStore = _host.Services.GetService<AuthenticationStore>();

            _startupService.MainVm = new MainViewModel(_authenticationStore, _startupService, typeManager, edtSettings, "NewInstance");

            MainWindow = new MainWindow() {
                DataContext = _startupService.MainVm
            };
            _startupService.InitializeProject(AppSettings.Default.ProjectDb);

            MainWindow.Show();

            // TimeSpan Values:     days, hours, minutes, seconds, milliseconds.
            TimeSpan FadeTimeout = new TimeSpan(0, 0, 0, 0, 500);
            splashScreen.Close(TimeSpan.FromMilliseconds(150));


            ShowAuthenticationWindowAsync();


#if !DEBUG

            ShowAuthenticationWindow();

#endif

            base.OnStartup(e);

        }

        private async void ShowAuthenticationWindowAsync()
        {
            await Task.Run(() => {
                Thread.Sleep(500);
                INavigationService navigationService = _host.Services.GetRequiredService<NavigationService<LoginViewModel>>();
                navigationService.Navigate();
                var authWindow = _host.Services.GetRequiredService<AuthenticationMainWindow>();
                Application.Current.Dispatcher.Invoke(() => {
                    authWindow.ShowDialog();

                });
            });
        }


        private void DeserializeRecentProjects()
        {
            try {
                var previousProjectDtoList = new ObservableCollection<PreviousProjectDto>();
                string recentProjets = "RecentProjects.bin";

                //Deserialize RecentProject to DTO's
                if (File.Exists(recentProjets)) {
                    using (Stream stream = File.Open(recentProjets, FileMode.Open)) {

                        if (stream.Length <= 0) return;

                        BinaryFormatter bin = new BinaryFormatter();

                        previousProjectDtoList = (ObservableCollection<PreviousProjectDto>)bin.Deserialize(stream);

                        foreach (var project in previousProjectDtoList) {
                            PreviousProjects.Add(new PreviousProject(_startupService, project.Project));
                        }
                    }

                }
            }
            catch (IOException ex) {
               NotificationHandler.ShowErrorMessage(ex);
            }
        }
    }
}
