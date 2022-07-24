using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUI.Helpers;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Electrical;
using WpfUI.Views.Electrical.MjeqSubviews;
using WpfUI.Windows;
using WpfUI.Windows.SelectionWindows;

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for MjeqView.xaml
/// </summary>
public partial class _MjeqView : UserControl
{
    private MjeqViewModel mjeqVm { get { return DataContext as MjeqViewModel; } }


    private bool _isEditingLoadGrids;

    public _MjeqView()
    {
        InitializeComponent();
        if (AppSettings.Default.AddEquipmentPanelView == 0) {
            gridAdding.Visibility = Visibility.Visible;
        }
        else {
            gridAdding.Visibility = Visibility.Collapsed;
        }

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
            //explicit propertyChange for load size
            if (e.Key == Key.Enter || e.Key == Key.Tab) {
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
        try {
            if (e.Key == Key.Delete) {
                if (_isEditingLoadGrids == false) {
                    DeleteLoads_VM();
                }
            }
        }
        catch (Exception ex) {
            throw ex;
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

    DteqTabsView _dteqDetailsView = new DteqTabsView();
    LoadTabsView _loadDetailsView = new LoadTabsView();

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
                _loadDetailsView.DataContext = this.DataContext;
                LoadDetailsContent.Content = _loadDetailsView;
            }
        }
        if (mjeqVm == null) return;
        {
            mjeqVm.SelectedLoads = dgdAssignedLoads.SelectedItems;
        }
    }


    //Testing/Shortcuts
    DebugWindow debugWindow = null;
    TestWindow testWindow = null;

    private async void eqView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {

#if DEBUG

            if (e.Key == Key.T) {
                LoadTestEquipmentData();
                e.Handled = true;
            }
            if (e.Key == Key.D) {
                MessageBox.Show("Deleting all Data");
                DeleteEquipment();
                e.Handled = true;

            }
            if (e.Key == Key.B) {
                MessageBox.Show("Clearing Equipment from Database");
                DeleteEquipmentFromDatabase();
                e.Handled = true;

            }
            if (e.Key == Key.F) {
                if (debugWindow == null || debugWindow.IsLoaded == false) {
                    debugWindow = new DebugWindow();
                    debugWindow.DataContext = mjeqVm.DebugViewModel;
                    debugWindow.Show();
                }
                e.Handled = true;
            }
            if (e.Key == Key.S) {
                if (testWindow == null || testWindow.IsLoaded == false) {
                    testWindow = new TestWindow();
                    testWindow.DataContext = mjeqVm;
                    testWindow.Show();
                }
            }

            //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
            //    if (e.Key == Key.Z) {
            //        Undo.UndoCommand(elecVm.ListManager);
            //    }
            //}
#endif
        }
    }

    //Testing







    #region Context Menus

    //LOAD
    private void AddEquipmentPanelViewToggle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //dgdDteq.Height = 10;
        //int count = 0;

        var maxDteqHeight = dteqButtonsStackPanel.ActualHeight - 25;
        dgdDteq.Height = maxDteqHeight + 40;
        AppSettings.Default.DteqGridHeight = dgdDteq.Height;
        AppSettings.Default.Save();

        //var MaxLoadHeight = loadButtonsStackPanel.ActualHeight + 50;
        //dgdAssignedLoads.Height = MaxLoadHeight + 15;


        if (gridAdding.Visibility == Visibility.Collapsed) {
            gridAdding.Visibility = Visibility.Visible;
            AppSettings.Default.AddEquipmentPanelView = 0;
            AppSettings.Default.Save();
            mjeqVm.LoadGridTop = new System.Windows.GridLength(127, GridUnitType.Pixel);
        }
        else {
            AppSettings.Default.AddEquipmentPanelView = 2;
            AppSettings.Default.Save();
            gridAdding.Visibility = Visibility.Collapsed;
            mjeqVm.LoadGridTop = new System.Windows.GridLength(0, GridUnitType.Pixel);

        }

    }

    private void LoadGridContextMenu_SetFedFrom(object sender, MouseButtonEventArgs e)
    {
        FedFromSelectionWindow fedFromSelectionWindow = new FedFromSelectionWindow();
        fedFromSelectionWindow.DataContext = mjeqVm;
        mjeqVm.SelectionWindow = fedFromSelectionWindow;
        fedFromSelectionWindow.ShowDialog();


        //IPowerConsumer load;
        //foreach (var item in dgdAssignedLoads.SelectedItems) {
        //    load = (IPowerConsumer)item;
        //    //dteq.Tag = "New Tag";
        //    load.FedFrom = mjeqVm.ListManager.IDteqList.FirstOrDefault(d => d.Tag == mjeqVm.LoadToAddValidator.FedFromTag);
        //}
    }

    private void LoadGridContextMenu_Delete(object sender, MouseButtonEventArgs e)
    {
        DeleteLoads_VM();


    }

    private async Task DeleteLoads_VM()
    {
        if (ConfirmationHelper.Confirm($"Delete {dgdAssignedLoads.SelectedItems.Count} loads? \n\nThis cannot be undone.")) {

            try {
                ILoad load;
                while (dgdAssignedLoads.SelectedItems.Count > 0) {
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        load = (LoadModel)dgdAssignedLoads.SelectedItems[0];
                        mjeqVm.DeleteLoadAsync(load);
                        dgdAssignedLoads.SelectedItems.Remove(load);
                    }));
                }
            }
            catch (Exception ex) {
                ex.Data.Add("UserMessage", "Cannot delete Distribution Equipment from Load List");
                ErrorHelper.ShowErrorMessage(ex);
            }
        }
    }

    private void FastEditEvent(object sender, RoutedEventArgs args)
    {
        var dataGridCell = (sender as UIElement)?.FindVisualParent<DataGridCell>();

        dataGridCell.FastEdit(args);
    }

    #endregion


    #region Filters
    //CollectionView in XAML
    private void cvsIdteq_Filter(object sender, FilterEventArgs e)
    {
        Task t = e.Item as Task;

        IDteq dteq = e.Item as IDteq;
        if (dteq != null)
        // If filter is turned on, filter completed items.
        {
            if (dteq.Tag == null
                       || dteq.Description == null
                       || dteq.Area == null
                       || dteq.FedFrom == null
                       ) {
                e.Accepted = true;
            }
            else if (dteq.Tag.ToLower().Contains(txtDteqTagFilter.Text.ToLower())
                    && dteq.Description.ToLower().Contains(txtDteqDescriptionFilter.Text.ToLower())
                    && dteq.Area.Tag.ToLower().Contains(txtDteqAreaFilter.Text.ToLower())
                    && dteq.FedFrom.Tag.ToLower().Contains(txtDteqFedFromFilter.Text.ToLower())
                    ) {
                e.Accepted = true;

            }
            else {
                e.Accepted = false;
            }
        }
    }

    //CollectionView in ViewModel
    private void DteqGridFilter()
    {
        try {

            mjeqVm.DteqCollectionView.Filter = (d) => {
                IDteq dteq = (IDteq)d;
                if (dteq != null)
                // If filter is turned on, filter completed items.
                {
                    if (dteq.Tag == null
                               || dteq.Description == null
                               || dteq.Area == null
                               || dteq.FedFrom == null
                               ) {
                        return true;
                    }
                    else if (dteq.Tag.ToLower().Contains(txtDteqTagFilter.Text.ToLower())
                            && dteq.Description.ToLower().Contains(txtDteqDescriptionFilter.Text.ToLower())
                            && dteq.Area.Tag.ToLower().Contains(txtDteqAreaFilter.Text.ToLower())
                            && dteq.FedFrom.Tag.ToLower().Contains(txtDteqFedFromFilter.Text.ToLower())
                            ) {
                        return true;

                    }
                    else {
                        return false;
                    }
                }
                return false;
            };
        }
        catch (Exception ex) {

        }

    }

    //Textbox change events
    private void DteqGridFilter(object sender, KeyEventArgs e)
    {
        TextBox textBox = (TextBox)sender;


        if (e.Key == Key.Enter /*|| e.Key == Key.Tab*/) {
            //elecVm.DteqFilter = true;
            //ApplyFilter();
            //CollectionViewSource.GetDefaultView(dgdDteq.ItemsSource).Refresh(); //XAML cvs
            DteqGridFilter();

        }

        if (e.Key == Key.Escape) {

            textBox = (TextBox)sender;
            textBox.Text = "";
            if (txtDteqTagFilter.Text == ""
                    && txtDteqAreaFilter.Text == ""
                    && txtDteqDescriptionFilter.Text == ""
                    && txtDteqFedFromFilter.Text == "") {

                //elecVm.DteqFilter = false;
            }
            //ApplyFilter();
            //CollectionViewSource.GetDefaultView(dgdDteq.ItemsSource).Refresh();  //XMAL cvs
            DteqGridFilter();

        }

        void ApplyFilter()
        {
            mjeqVm.DteqList.Clear();
            foreach (var dteq in mjeqVm.ListManager.IDteqList) {
                if (dteq.Tag.ToLower().Contains(txtDteqTagFilter.Text.ToLower())
                    && dteq.Description.ToLower().Contains(txtDteqDescriptionFilter.Text.ToLower())
                    && dteq.Area.Tag.ToLower().Contains(txtDteqAreaFilter.Text.ToLower())
                    && dteq.FedFrom.Tag.ToLower().Contains(txtDteqFedFromFilter.Text.ToLower())
                    ) {
                    mjeqVm.DteqList.Add(dteq);
                }
            }
        }
    }
    private void LoadGridFilter(object sender, KeyEventArgs e)
    {
        TextBox textBox = (TextBox)sender;


        if (e.Key == Key.Enter || e.Key == Key.Escape /*|| e.Key == Key.Tab*/) {
            if (e.Key == Key.Escape) {
                textBox = (TextBox)sender;
                textBox.Text = "";
            }
            ApplyFilter();
        }



        void ApplyFilter()
        {
            if (mjeqVm.LoadListLoaded == false && mjeqVm.SelectedDteq != null) {
                Filter(mjeqVm.SelectedDteq.AssignedLoads);
            }
            else if (mjeqVm.LoadListLoaded == true) {
                ObservableCollection<IPowerConsumer> list = new ObservableCollection<IPowerConsumer>();
                foreach (var item in mjeqVm.ListManager.LoadList) {
                    list.Add(item);
                }
                Filter(list);
            }

            void Filter(ObservableCollection<IPowerConsumer> listToFilter)
            {
                mjeqVm.AssignedLoads.Clear();
                foreach (var load in listToFilter) {
                    try {

                        if (load.Description != null) {
                            if (load.Tag.ToLower().Contains(txtLoadTagFilter.Text.ToLower())
                            && load.Description.ToLower().Contains(txtLoadDescriptionFilter.Text.ToLower())
                            && load.Area.Tag.ToLower().Contains(txtLoadAreaFilter.Text.ToLower())
                            && load.FedFrom.Tag.ToLower().Contains(txtLoadFedFromFilter.Text.ToLower())
                            && load.Voltage.ToString().Contains(txtLoadVoltageFilter.Text.ToLower())
                            && load.Size.ToString().ToLower().Contains(txtLoadSizeFilter.Text.ToLower())
                            && load.Unit.ToString().ToLower().Contains(txtLoadUnitFilter.Text.ToLower())
                            && load.Type.ToString().ToLower().Contains(txtLoadTypeFilter.Text.ToLower())
                            ) {
                                mjeqVm.AssignedLoads.Add((IPowerConsumer)load);
                            }
                        }
                        else {
                            if (load.Tag.ToLower().Contains(txtLoadTagFilter.Text.ToLower())
                            && load.Area.Tag.ToLower().Contains(txtLoadAreaFilter.Text.ToLower())
                            && load.FedFrom.Tag.ToLower().Contains(txtLoadFedFromFilter.Text.ToLower())
                            && load.Voltage.ToString().Contains(txtLoadVoltageFilter.Text.ToLower())
                            && load.Size.ToString().ToLower().Contains(txtLoadSizeFilter.Text.ToLower())
                            && load.Unit.ToString().ToLower().Contains(txtLoadUnitFilter.Text.ToLower())
                            && load.Type.ToString().ToLower().Contains(txtLoadTypeFilter.Text.ToLower())
                            ) {
                                mjeqVm.AssignedLoads.Add((IPowerConsumer)load);
                            }
                        }


                    }
                    catch { } //for any empty strings
                }
            }
        }
    }
    #endregion

    #region Editing and Add Eq Control Events

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


    private void btnAddDteq_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (txtDteqTag.Text == GlobalConfig.EmptyTag) {
            txtDteqTag.Text = "";
        }
    }
    private void btnAddDteq_MouseLeave(object sender, MouseEventArgs e)
    {
        Task.Run(() => resetTag());
        resetTag();
        async Task resetTag()
        {
            if (txtDteqTag.Text == "") {
                await Task.Delay(500);

                if (txtDteqTag.IsFocused == false)
                    txtDteqTag.Text = GlobalConfig.EmptyTag;
            }
        }
    }

    private void eqView_Unloaded(object sender, RoutedEventArgs e)
    {
    }
    private void eqView_MouseLeave(object sender, MouseEventArgs e)
    {
        var dataView = (ListCollectionView)CollectionViewSource.GetDefaultView(dgdDteq.ItemsSource);
        if (dataView.IsEditingItem)
            dataView.CommitEdit();
    }
    private void btnAddLoad_MouseLeave(object sender, MouseEventArgs e)
    {
        Task.Run(() => resetTag());
        resetTag();
        async Task resetTag()
        {
            if (txtLoadTag.Text == "") {
                await Task.Delay(500);

                if (txtLoadTag.IsFocused == false)
                    txtLoadTag.Text = GlobalConfig.EmptyTag;
            }
        }
    }
    private void btnAddLoad_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (txtLoadTag.Text == GlobalConfig.EmptyTag) {
            txtLoadTag.Text = "";
        }
    }
    private void dgdDteq_MouseLeave(object sender, MouseEventArgs e)
    {
        var dataGrid = (DataGrid)sender;
        dataGrid.CancelEdit();
    }
    private void dgdAssignedLoads_MouseLeave(object sender, MouseEventArgs e)
    {
        var dataGrid = (DataGrid)sender;
        dataGrid.CancelEdit();
    }
    private void dgdAssignedLoads_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        _isEditingLoadGrids = true;
    }
    private void dgdAssignedLoads_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        _isEditingLoadGrids = false;
    }
    #endregion

    #region Testing
    //Testing
    private async Task LoadTestEquipmentData()
    {
        ListManager listManager = mjeqVm.ListManager;

        MessageBoxResult result = MessageBox.Show("Dteq, Loads, Both", "Test Data", MessageBoxButton.YesNoCancel);
        string start = "";
        GlobalConfig.Importing = true;
        switch (result) {
            case MessageBoxResult.Yes:
                start = DateTime.Now.ToString();
                AddTestDteq(listManager, start);
                break;

            case MessageBoxResult.No:
                AddTestDteq(listManager, start);
                start = DateTime.Now.ToString();
                await Task.Run(() => AddTestLoadsAsync(listManager));
                mjeqVm.GetLoadList();
                Debug.Print($"start: {start} end: {DateTime.Now.ToString()}");
                break;

            case MessageBoxResult.Cancel:
                start = DateTime.Now.ToString();
                AddTestDteq(listManager, start);
                foreach (var load in TestData.TestLoadList) {
                    load.Area = listManager.AreaList[0];
                    LoadToAddValidator loadToAdd = new LoadToAddValidator(listManager, load);
                    mjeqVm.AddLoad(loadToAdd);
                    load.CalculateLoading();
                }
                Debug.Print($"start: {start} end: {DateTime.Now.ToString()}");
                break;
        }
        GlobalConfig.Importing = false;
        mjeqVm.DbSaveAll();

        Debug.Print($"Final start: {start} Final end: {DateTime.Now.ToString()}");
    }

    private void AddTestDteq(ListManager listManager, string start)
    {
        foreach (var dteq in TestData.TestDteqList) {
            dteq.Area = listManager.AreaList[0];
            DteqToAddValidator dteqToAdd = new DteqToAddValidator(listManager, dteq);
            mjeqVm.AddDteq(dteqToAdd);
            //Debug.Print($"start: {start} end: {DateTime.Now.ToString()}");
        }
    }

    private async Task AddTestLoadsAsync(ListManager listManager)
    {
        try {

            //List<Task<LoadModel>> tasks = new List<Task<LoadModel>>();

            foreach (var load in TestData.TestLoadList) {
                load.Area = listManager.AreaList[0];
                LoadToAddValidator loadToAdd = new LoadToAddValidator(listManager, load);
                await Task.Run(() => LoadManager.AddLoad(loadToAdd, listManager));

                //tasks.Add  (Task.Run(() => LoadManager.AddLoad(loadToAdd, listManager)));  
                //load.CalculateLoadingAsync();
            }
            //var results = await Task.WhenAll(tasks);
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);

        }

    }

    private void DeleteEquipment()
    {
        while (mjeqVm.ListManager.IDteqList.Count > 0) {
            IDteq dteq = mjeqVm.ListManager.IDteqList[0];
            mjeqVm.DeleteDteq(dteq);
        }

        while (mjeqVm.ListManager.LoadList.Count > 0) {
            mjeqVm.DeleteLoad(mjeqVm.ListManager.LoadList[0]);
        }

        DaManager.prjDb.DeleteAllRecords(GlobalConfig.ComponentTable);
    }

    private void DeleteEquipmentFromDatabase()
    {
        //Delete records
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.DteqTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.XfrTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.SwgTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.MccTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.LoadTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.CableTable);

        mjeqVm.DbGetAll();
    }
}
    #endregion



