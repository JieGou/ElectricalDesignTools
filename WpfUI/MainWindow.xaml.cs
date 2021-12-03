using EDTLibrary;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ListManager.loadList.Add(new EDTLibrary.Models.LoadModel { Tag = "Test", PowerFactor = 0.8 });
            ObservableCollection<LoadModel> loadList = new ObservableCollection<LoadModel>();
            dgdTest.ItemsSource = loadList;
            dgdTest.ItemsSource = ListManager.loadList;
            //dgdTest.ItemsSource = ListManager.loadList;
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            //Browsable = false
            if (((PropertyDescriptor)e.PropertyDescriptor).IsBrowsable == false) {
                e.Cancel = true;
            }
            //Description == "x"
            if (((PropertyDescriptor)e.PropertyDescriptor).Description == "GroupName") {
                e.Cancel = true;
            }
            if ((e.PropertyName) == "Tye") {
                var cb = new DataGridComboBoxColumn();
                cb.ItemsSource = new List<string> { "test", "test2" };               
                e.Column = cb;
            }
            //Display name
            e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
            e.Column.MinWidth = 50;       
            
        }

        
    }    
}
