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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EDTLibrary;
using WinFormCoreUI;
using EDTLibrary.DataAccess;

namespace WpfUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window {
        
        public MainWindow() {
                InitializeComponent();
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
                if ((e.PropertyName) == "Type") {
                    var cb = new DataGridComboBoxColumn();
                    cb.ItemsSource = new List<string> { "ADD LOAD TYPES" };               
                    //e.Column = cb;
                }
                //Display name
                e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
                e.Column.MinWidth = 50;

                e.Column.HeaderStyle = new Style(typeof(DataGridColumnHeader));
                e.Column.HeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

                e.Column.CellStyle = new Style(typeof(DataGridCell));
                e.Column.CellStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));


                if (e.PropertyName == "FedFrom") {

                    var cb = new DataGridComboBoxColumn();
                    cb.ItemsSource = ListManager.dteqList;
                    cb.SelectedValuePath = "Tag";
                    cb.DisplayMemberPath = "Tag";
                    cb.SelectedValueBinding = new Binding("FedFrom"); //allows binding to the property

                    cb.CellStyle  = new Style(typeof(DataGridCell));
                    cb.CellStyle.Setters.Add(new Setter(BackgroundProperty, Brushes.Transparent));
                    cb.CellStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.Black));
                    cb.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
                    e.Column = cb;
                }
            }
        }    
}
