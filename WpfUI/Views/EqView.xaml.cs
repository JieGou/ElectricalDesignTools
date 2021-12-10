using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinFormCoreUI;
using WpfUI.ViewModels;

namespace WpfUI.Views {
    /// <summary>
    /// Interaction logic for EqView.xaml
    /// </summary>
    public partial class EqView : Window {
        private readonly EqViewModel _viewModel;

        public EqView() {
            InitializeComponent();
            DbManager.SetProjectDb(Settings.Default.ProjectDb);
            DbManager.SetLibraryDb(Settings.Default.LibraryDb);

            _viewModel = new EqViewModel();
            this.DataContext = _viewModel;
            
        }


        //BUTTONS

        //OC
        private void addDteqOC_Click(object sender, RoutedEventArgs e) {
        }
        private void addLoad_Click(object sender, RoutedEventArgs e) {
        }
        private void SDOC_Click(object sender, RoutedEventArgs e) {
        }
        private void SLOC_Click(object sender, RoutedEventArgs e) {

        }
        private void CLOC_Click(object sender, RoutedEventArgs e) {
        }

    }
}
