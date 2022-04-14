using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfUI.Helpers;
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

        private void dgdAssignedLoads_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try {
                if (e.Key == Key.Enter ||
                                e.Key == Key.Tab) {
                    //TODO - set specific column by header or name
                    DataGridTextColumn col = (DataGridTextColumn)loadSize;
                    ILoad item = (LoadModel)dgdAssignedLoads.SelectedItem;
                    UpdateBindingTarget(dgdAssignedLoads, col, item);
                }
            }
            catch (Exception ex) {
                ex.Data.Add("UserMessage", "Cannot undo changes made to Distribution Equipment if they are made form the load Grid.");
                //ErrorHelper.EdtErrorMessage(ex);
            }
            
        }

        static void UpdateBindingTarget(DataGrid dg, DataGridTextColumn col, ILoad item)
        {
            DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromItem(item);
            BindingExpression be = null;
            if (col.GetCellContent(row).GetType() == typeof(TextBox)) {
                TextBox txt = (TextBox)col.GetCellContent(row);
                be = txt.GetBindingExpression(TextBox.TextProperty);
            }
            if (be != null) { be.UpdateSource(); }
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


        //Testing/Shortcuts
        private async void eqView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
#if DEBUG

                if (e.Key == Key.T) {
                    LoadTestEquipmentData();
                }
                if (e.Key == Key.D) {
                    MessageBox.Show("Deleting all Data");
                    DeleteEquipment();
                }
                if (e.Key == Key.B) {
                    MessageBox.Show("Clearing Equipment from Database");
                    DeleteEquipmentFromDatabase();
                }
#endif
                //Moved Ctrl-Z to main window
                //if (e.Key == Key.Z) {
                //    Undo.UndoCommand(eqVm.ListManager);
                //}
            }


        }

        //Testing
        private async Task LoadTestEquipmentData()
        {
            DteqToAddValidator dteqToAdd;
            LoadToAddValidator loadToAdd;
            ListManager listManager= eqVm.ListManager;

            MessageBoxResult result = MessageBox.Show("Dteq, Loads, Both", "Test Data", MessageBoxButton.YesNoCancel);
            switch (result) {
                case MessageBoxResult.Yes:
                    foreach (var dteq in TestData.TestDteqList) {
                        dteq.Area = listManager.AreaList[0];
                        dteqToAdd = new DteqToAddValidator(listManager, dteq);
                        eqVm.AddDteq(dteqToAdd);
                    }
                    break;
                case MessageBoxResult.No:
                    foreach (var load in TestData.TestLoadList) {
                        load.Area = listManager.AreaList[0];
                        loadToAdd = new LoadToAddValidator(listManager, load);
                        eqVm.AddLoad(loadToAdd);
                        load.CalculateLoading();
                    }
                    break;
                case MessageBoxResult.Cancel:
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
                    break;
            }
            //TestData.CreateTestDteqList();
            
        }
        //Testing
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

        private void btnGrdSplitAdjust_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dgdDteq.Height += 1;
        }

        private void btnGrdSplitAdjust_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dgdDteq.Height += 1;

        }

        private void btnGrdSplitAdjust_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dgdDteq.Height = 10;
            int count = 0;

            var maxDteqHeight = dteqButtonsStackPanel.ActualHeight - 10;
            dgdDteq.Height = maxDteqHeight + 40;

            var MaxLoadHeight = loadButtonsStackPanel.ActualHeight + 50;
            dgdAssignedLoads.Height = MaxLoadHeight+15;

            Undo.UndoCommand(eqVm.ListManager);
        }

        private void LoadGridContextMenu_SetFedFrom(object sender, MouseButtonEventArgs e)
        {
            ILoad load;
            foreach (var item in dgdAssignedLoads.SelectedItems) {
                load = (LoadModel)item;
                //dteq.Tag = "New Tag";
                load.FedFrom = eqVm.ListManager.IDteqList.FirstOrDefault(d => d.Tag == eqVm.LoadToAddValidator.FedFromTag);
            }
        }

        private void LoadGridContextMenu_Delete(object sender, MouseButtonEventArgs e)
        {
            ILoad load;
            while (dgdAssignedLoads.SelectedItems.Count>0) {
                //load = (LoadModel)dgdAssignedLoads.SelectedItems[0];
                //eqVm.DeleteLoad(load);
            }
        }

       
        private void FastEditEvent(object sender, RoutedEventArgs args)
        {
            var dataGridCell = (sender as UIElement)?.FindVisualParent<DataGridCell>();

            dataGridCell.FastEdit(args);
        }
    }
    public static class DataGridExtensions
    {
        public static void FastEdit(this DataGridCell dataGridCell, RoutedEventArgs args)
        {
            if (dataGridCell == null || dataGridCell.IsEditing || dataGridCell.IsReadOnly) {
                return;
            }

            var dataGrid = dataGridCell.FindVisualParent<DataGrid>();

            if (dataGrid == null) {
                return;
            }

            if (!dataGridCell.IsFocused) {
                dataGridCell.Focus();
            }

            dataGrid.Dispatcher.InvokeAsync(() =>
            {
                dataGrid.BeginEdit(args);
            });
        }
    }

    public static class UiElementExtensions
    {
        public static T FindVisualParent<T>(this UIElement element)
            where T : UIElement
        {
            var currentElement = element;

            while (currentElement != null) {
                if (currentElement is T correctlyTyped) {
                    return correctlyTyped;
                }

                currentElement = VisualTreeHelper.GetParent(currentElement) as UIElement;
            }

            return null;
        }
    }
}
