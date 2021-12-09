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
        public ObservableCollection<DteqModel> DteqList { get; set; }
        public ObservableCollection<LoadModel> LoadList { get; set; }
    public MainWindow() {
            InitializeComponent();
            this.DataContext = this;

            UI.prjDb = new SQLiteConnector("C:\\Users\\pdeau\\Google Drive\\Work\\Visual Studio Projects\\_EDT Tables\\EDT SQLite DB Files\\EDTProjectTemplate1.1.db");
            //ListManager.loadList.Add(new EDTLibrary.Models.LoadModel { Tag = "Test", PowerFactor = 0.8 });

            ListManager.list = UI.prjDb.GetRecords<LoadModel>("Loads");
            ListManager.dteqList = UI.prjDb.GetRecords<DteqModel>("DistributionEquipment");

            DteqList = new ObservableCollection<DteqModel>(ListManager.dteqList);
            LoadList = new ObservableCollection<LoadModel>(ListManager.list);

            dgdDteqOC.ItemsSource = DteqList;
            dgdLoadsOC.ItemsSource = LoadList;

            dgdDteqLM.ItemsSource = ListManager.dteqList;            
            dgdLoadsLM.ItemsSource = ListManager.list;
        }
        private void TestEvent(object sender, EventArgs e) {
            MessageBox.Show("event Fired");
            ListManager.CreateEqDict();
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

        //BUTTONS

        //OC
        private void addDteqOC_Click(object sender, RoutedEventArgs e) {
            DteqList.Add(new DteqModel() { Tag = "MCC-99" });
        }
        private void addLoad_Click(object sender, RoutedEventArgs e) {
            LoadList.Add(new LoadModel() { Tag = "MTR-99" });
        }
        private void SDOC_Click(object sender, RoutedEventArgs e) {
            foreach (var item in DteqList) {
                MessageBox.Show($"{item.Tag} {item.FedFrom}");
            }
        }
        private void SLOC_Click(object sender, RoutedEventArgs e) {
            foreach (var item in LoadList) {
                MessageBox.Show($"{item.Tag} {item.FedFrom}");
            }
        }
        private void CLOC_Click(object sender, RoutedEventArgs e) {
            LoadList[1].Tag = "TEST";
        }


        //List
        private void addDteqLM_Click(object sender, RoutedEventArgs e) {
            ListManager.dteqList.Add(new DteqModel() { Tag = "MCC-99" });
        }
        private void addLM_Click(object sender, RoutedEventArgs e) {
            ListManager.list.Add(new LoadModel() { Tag = "MTR-99" });
        }
        private void SDLM_Click(object sender, RoutedEventArgs e) {
            dgdDteqLM.ItemsSource = null;
            dgdDteqLM.ItemsSource = ListManager.dteqList;
            foreach (var item in ListManager.dteqList) {
                MessageBox.Show($"{item.Tag} {item.FedFrom}");
            }
        }
        private void SLLM_Click(object sender, RoutedEventArgs e) {
            dgdLoadsLM.Items.Refresh();
            //dgdLoadsLM.ItemsSource = ListManager.loadList;
            foreach (var item in ListManager.list) {
                MessageBox.Show($"{item.Tag} {item.FedFrom}");
            }
        }

        private void CLLM_Click(object sender, RoutedEventArgs e) {
            ListManager.list[1].Tag = "TEST";
        }

        private void dgdDteqOC_Loaded(object sender, RoutedEventArgs e) {
            dgdDteqOC.CommitEdit();

        }

        private void dgdDteqOC_Unloaded(object sender, RoutedEventArgs e) {
            dgdDteqOC.CancelEdit();
        }
    }    
}
