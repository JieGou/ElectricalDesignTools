using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfUI.Stores;
using WpfUI.ViewModels;
using WpfUI.Views;

namespace WpfUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {

            NavigationStore navigationStore = new NavigationStore();
            navigationStore.CurrentViewModel = new EqViewModel(navigationStore);

            MainWindow = new MainWindow() { 
                DataContext = new MainViewModel(navigationStore) 
            };

            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
