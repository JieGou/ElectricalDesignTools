using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUI.ViewModels;
using WpfUI.Views.SubViews;

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for EqView.xaml
    /// </summary>
    public partial class EquipmentView : UserControl
    {
        private EquipmentViewModel eqVm { get { return DataContext as EquipmentViewModel; } }

        DteqDetailView _dteqDetailsView = new DteqDetailView();
        LoadDetailView _loaDetailView = new LoadDetailView();
        public EquipmentView()
        {
            InitializeComponent();
        }

        private void txtDteqTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtDteqTag.Text == "" || txtDteqTag.Text == GlobalConfig.EmptyTag) txtDteqTag.Text = "";
        }

        private void txtDteqTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDteqTag.Text == "") txtDteqTag.Text = GlobalConfig.EmptyTag;
        }


        private void txtLoadTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtLoadTag.Text == "" || txtLoadTag.Text == GlobalConfig.EmptyTag) txtLoadTag.Text = "";
        }

        private void txtLoadTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLoadTag.Text == "") txtLoadTag.Text = GlobalConfig.EmptyTag;
        }

        private void dgdDteq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) {
                dgdDteq.CancelEdit();
            }
        }

      

        private void dgdDteq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DteqDetailsContent.Content = null;
            if (dgdDteq.SelectedItem != null) {
                if (dgdDteq.SelectedItem.GetType().IsSubclassOf(typeof(DistributionEquipment))) {
                    _dteqDetailsView.DataContext = this.DataContext;
                    DteqDetailsContent.Content = _dteqDetailsView;
                }
            }
        }

        private void dgdAssignedLoads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgdAssignedLoads.SelectedItem != null) {
                if (dgdAssignedLoads.SelectedItem.GetType() == typeof(LoadModel)) {
                    _loaDetailView.DataContext = this.DataContext;
                    LoadDetailsContent.Content = _loaDetailView;
                }
            }
        }

        private void eqView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                if (e.Key == Key.T) {
                    MessageBox.Show("kD");
                }
            }
        }

        private async void eqView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
#if DEBUG
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                if (e.Key == Key.T) {
                    MessageBox.Show("Loading Test Data");
                    await LoadTestEquipmentData();
                }
                if (e.Key == Key.D) {
                    MessageBox.Show("Deleting all Data");
                    DeleteEquipment();
                }
                if (e.Key == Key.B) {
                    MessageBox.Show("Clearing Equipment from Database");
                    DeleteEquipmentFromDatabase();
                }
            }


#endif
        }

        private async Task LoadTestEquipmentData()
        {
            DteqToAddValidator dteqToAdd;
            LoadToAddValidator loadToAdd;
            ListManager listManager= eqVm.ListManager;

            //TestData.CreateTestDteqList();
            foreach (var dteq in TestData.TestDteqList) {
                dteq.Area = listManager.AreaList[0];
                dteqToAdd = new DteqToAddValidator(listManager, dteq);
                eqVm.AddDteq(dteqToAdd);
            }
            foreach (var load in TestData.TestLoadList) {
                load.Area = listManager.AreaList[0];
                loadToAdd = new LoadToAddValidator(listManager, load);
                eqVm.AddLoad(loadToAdd);
                load.CalculateLoading();
            }
        }

        private void DeleteEquipment()
        {
            while (eqVm.ListManager.IDteqList.Count > 0) {
                IDteq dteq = eqVm.ListManager.IDteqList[0];
                eqVm.DeleteDteq(dteq);
            }

            while (eqVm.ListManager.LoadList.Count > 0) {
                eqVm.DeleteLoad(eqVm.ListManager.LoadList[0]);
            }
        }

        private void DeleteEquipmentFromDatabase()
        {
            //Delete records
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.DteqTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.XfrTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.SwgTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.MccTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.LoadTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.PowerCableTable);

            eqVm.DbGetAll();
           
        }
    }
}
