using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUI.Helpers;
using WpfUI.SyncFusion.Renderers;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Electrical;
using WpfUI.Views.Electrical.LoadListSubViews;
using WpfUI.Windows;

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for MjeqView.xaml
/// </summary>
public partial class LoadListView : UserControl
{
    private LoadListViewModel vm { get { return DataContext as LoadListViewModel; } }


    private bool _isEditingLoadGrids;

    public LoadListView()
    {
        InitializeComponent();
        if (AppSettings.Default.AddEquipmentPanelView == 0) {
            gridAdding.Visibility = Visibility.Visible;
        }
        else {
            gridAdding.Visibility = Visibility.Collapsed;
        }

        dgdAssignedLoads.CellRenderers.Remove("ComboBox");
        dgdAssignedLoads.CellRenderers.Add("ComboBox", new ComboBoxRenderer());
        
        dgdAssignedLoads.CellRenderers["ComboBox"] = new CustomGridCellComboBoxRenderer();


        _ViewStateManager.ElectricalViewUpdate += OnElectricalViewUpdated;

    }

    private void dgdDteq_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) {
            dgdDteq.CancelEdit();
        }
    }

    private void dgdAssignedLoads_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        //try {
        //    //explicit propertyChange for load size
        //    if (e.Key == Key.Enter || e.Key == Key.Tab) {
        //        //TODO - set specific column by header or name
        //        DataGridTextColumn col = (DataGridTextColumn)loadSize;
        //        ILoad item = (LoadModel)dgdAssignedLoads.SelectedItem;
        //        UpdateBindingTarget(dgdAssignedLoads, col, item);
        //    }
        //}
        //catch (Exception ex) {
        //    //ErrorHelper.EdtErrorMessage(ex);
        //}


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

    //explicit update source for LoadModel??
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

    ListViewPropertyPane _loadTabsView = new ListViewPropertyPane();

    //Sets the datacontext for the details view panel on the right
    private void dgdDteq_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //if (dgdDteq.SelectedItem != null) {
        //    _loadTabsView.DataContext = this.DataContext;
        //    LoadDetailsContent.Content = _loadTabsView;
        //}
    }



    //Testing/Shortcuts
    DebugWindow debugWindow = null;
    TestWindow testWindow = null;

    private async void eqView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {

            if (e.Key == Key.V) {
                //vm.AddLoad(vm.SelectedLoad);
            }

#if DEBUG

            if (e.Key == Key.T) {
                LoadTestEquipmentData();
                e.Handled = true;
            }
           
            if (e.Key == Key.B) {
                MessageBox.Show("Clearing Equipment from Database");
                DeleteEquipmentFromDatabase();
                e.Handled = true;

            }
            if (e.Key == Key.D) {
                if (debugWindow == null || debugWindow.IsLoaded == false) {
                    debugWindow = new DebugWindow();
                    debugWindow.Show();
                }
                e.Handled = true;
            }

            if (e.Key == Key.S) {
                if (testWindow == null || testWindow.IsLoaded == false) {
                    testWindow = new TestWindow();
                    testWindow.DataContext = vm;
                    testWindow.Show();
                }
            }
#endif
        }
    }


    #region Context Menus

    //LOAD   
    //Sets the datacontext for the details view panel on the right
    private async void dgdAssignedLoads_SelectionChanged_1(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
    {

        if (vm == null) return;

        CopySelectedLoads(vm);


        async Task CopySelectedLoads(LoadListViewModel vm)
        {
            vm.SelectedLoads.Clear();
            foreach (var item in dgdAssignedLoads.SelectedItems) {
                vm.SelectedLoads.Add((IPowerConsumer)item);
            }
        }
    }

    private void dgdAssignedLoads_PreviewKeyDown_1(object sender, KeyEventArgs e)
    {
        dgdAssignedLoads.SearchHelper.SearchType = Syncfusion.UI.Xaml.Grid.SearchType.Contains;
        if (e.Key == Key.Escape) {
            dgdAssignedLoads.ClearFilters();
        }
    }


    private void LoadGridContextMenu_Delete(object sender, MouseButtonEventArgs e)
    {
        DeleteLoads_VM();
    }


   

   

    private async Task DeleteLoads_VM()
    {
        if (dgdAssignedLoads.SelectedItems == null) return;
        if (ConfirmationHelper.Confirm($"Delete {dgdAssignedLoads.SelectedItems.Count} loads? \n\nThis cannot be undone.")) {

            try {
                ILoad load;
                while (dgdAssignedLoads.SelectedItems.Count > 0) {
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        load = (LoadModel)dgdAssignedLoads.SelectedItems[0];
                        vm.DeleteSingleLoadAsync(load);
                        dgdAssignedLoads.SelectedItems.Remove(load);
                    }));
                }
            }
            catch (Exception ex) {
                ex.Data.Add("UserMessage", "Cannot delete Distribution Equipment from Load List");
                NotificationHandler.ShowErrorMessage(ex);
            }
        }
    }

    #endregion

    private void FastEditEvent(object sender, RoutedEventArgs args)
    {
        var dataGridCell = (sender as UIElement)?.FindVisualParent<DataGridCell>();

        dataGridCell.FastEdit(args);
    }

    private void FastEditEventSyncFusion(object sender, RoutedEventArgs args)
    {
        var dataGridCell = (sender as UIElement)?.FindVisualParent<Syncfusion.UI.Xaml.Grid.GridCell>();

        //dataGridCell.FastEdit(args);
    }


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

            vm.DteqCollectionView.Filter = (d) => {
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
            vm.DteqList.Clear();
            foreach (var dteq in vm.ListManager.IDteqList) {
                if (dteq.Tag.ToLower().Contains(txtDteqTagFilter.Text.ToLower())
                    && dteq.Description.ToLower().Contains(txtDteqDescriptionFilter.Text.ToLower())
                    && dteq.Area.Tag.ToLower().Contains(txtDteqAreaFilter.Text.ToLower())
                    && dteq.FedFrom.Tag.ToLower().Contains(txtDteqFedFromFilter.Text.ToLower())
                    ) {
                    vm.DteqList.Add(dteq);
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
            if (vm.LoadListLoaded == false && vm.SelectedDteq != null) {
                Filter(vm.SelectedDteq.AssignedLoads);
            }
            else if (vm.LoadListLoaded == true) {
                ObservableCollection<IPowerConsumer> list = new ObservableCollection<IPowerConsumer>();
                foreach (var item in vm.ListManager.LoadList) {
                    list.Add(item);
                }
                Filter(list);
            }

            void Filter(ObservableCollection<IPowerConsumer> listToFilter)
            {
                vm.AssignedLoads.Clear();
                foreach (var load in listToFilter) {
                    try {

                        //if (load.Description != null) {
                        //    if (load.Tag.ToLower().Contains(txtLoadTagFilter.Text.ToLower())
                        //    && load.Description.ToLower().Contains(txtLoadDescriptionFilter.Text.ToLower())
                        //    && load.Area.Tag.ToLower().Contains(txtLoadAreaFilter.Text.ToLower())
                        //    && load.FedFrom.Tag.ToLower().Contains(txtLoadFedFromFilter.Text.ToLower())
                        //    && load.Voltage.ToString().Contains(txtLoadVoltageFilter.Text.ToLower())
                        //    && load.Size.ToString().ToLower().Contains(txtLoadSizeFilter.Text.ToLower())
                        //    && load.Unit.ToString().ToLower().Contains(txtLoadUnitFilter.Text.ToLower())
                        //    && load.Type.ToString().ToLower().Contains(txtLoadTypeFilter.Text.ToLower())
                        //    ) {
                        //        MjeqVm.AssignedLoads.Add((IPowerConsumer)load);
                        //    }
                        //}
                        //else {
                        //    if (load.Tag.ToLower().Contains(txtLoadTagFilter.Text.ToLower())
                        //    && load.Area.Tag.ToLower().Contains(txtLoadAreaFilter.Text.ToLower())
                        //    && load.FedFrom.Tag.ToLower().Contains(txtLoadFedFromFilter.Text.ToLower())
                        //    && load.Voltage.ToString().Contains(txtLoadVoltageFilter.Text.ToLower())
                        //    && load.Size.ToString().ToLower().Contains(txtLoadSizeFilter.Text.ToLower())
                        //    && load.Unit.ToString().ToLower().Contains(txtLoadUnitFilter.Text.ToLower())
                        //    && load.Type.ToString().ToLower().Contains(txtLoadTypeFilter.Text.ToLower())
                        //    ) {
                        //        MjeqVm.AssignedLoads.Add((IPowerConsumer)load);
                        //    }
                        //}


                    }
                    catch { } //for any empty strings
                }
            }
        }
    }
    #endregion

    #region Add Eq Control Events and Grid edit Events

    private void txtDteqTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        //if (txtDteqTag.Text == "" || txtDteqTag.Text == GlobalConfig.EmptyTag) txtDteqTag.Text = "";
    }
    private void txtDteqTag_LostFocus(object sender, RoutedEventArgs e)
    {
        //if (txtDteqTag.Text == "") txtDteqTag.Text = GlobalConfig.EmptyTag;
    }


    private void txtLoadTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        //if (txtLoadTag.Text == "" || txtLoadTag.Text == GlobalConfig.EmptyTag) txtLoadTag.Text = "";
    }
    private void txtLoadTag_LostFocus(object sender, RoutedEventArgs e)
    {
        //if (txtLoadTag.Text == "") txtLoadTag.Text = GlobalConfig.EmptyTag;
    }


    private void btnAddDteq_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //if (txtDteqTag.Text == GlobalConfig.EmptyTag) {
        //    txtDteqTag.Text = "";
        //}
    }
    private void btnAddDteq_MouseLeave(object sender, MouseEventArgs e)
    {
        Task.Run(() => resetTag());
        resetTag();
        async Task resetTag()
        {
            //if (txtDteqTag.Text == "") {
            //    await Task.Delay(500);

            //    if (txtDteqTag.IsFocused == false)
            //        txtDteqTag.Text = GlobalConfig.EmptyTag;
            //}
        }
    }

    

    private void btnAddLoad_MouseLeave(object sender, MouseEventArgs e)
    {
        Task.Run(() => resetTag());
        resetTag();
        async Task resetTag()
        {
            //if (txtLoadTag.Text == "") {
            //    await Task.Delay(500);

            //    if (txtLoadTag.IsFocused == false)
            //        txtLoadTag.Text = GlobalConfig.EmptyTag;
            //}
        }
    }
    private void btnAddLoad_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //if (txtLoadTag.Text == GlobalConfig.EmptyTag) {
        //    txtLoadTag.Text = "";
        //}
    }

    private void eqView_Unloaded(object sender, RoutedEventArgs e)
    {
        _ViewStateManager.ElectricalViewUpdate -= OnElectricalViewUpdated;

    }
    private void eqView_MouseLeave(object sender, MouseEventArgs e)
    {
        var dataView = (ListCollectionView)CollectionViewSource.GetDefaultView(dgdDteq.ItemsSource);
        if (dataView == null) return;
        if (dataView.IsEditingItem) dataView.CommitEdit();
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
        ListManager listManager = vm.ListManager;

        MessageBoxResult result = MessageBox.Show($"Yes = Short Load List{Environment.NewLine}" +
                                                  $"No = Full Load List{Environment.NewLine}" +
                                                  $"Cancel = Add 10 Extra Motors to MCC-01", 
                                                  "Load Test Data", MessageBoxButton.YesNoCancel);

        DaManager.Importing = true;

        switch (result) {
            case MessageBoxResult.Yes:
                AddTestDteq(listManager);
                //await Task.Run(() => AddTestLoadsAsync(listManager, TestData.TestLoadList_Short));
                await AddTestLoadsAsync(listManager, TestData.TestLoadList_Short);           
                vm.GetLoadList();
                break;

            case MessageBoxResult.No:
                AddTestDteq(listManager);
                await AddTestLoadsAsync(listManager, TestData.TestLoadList_Full);
            

                break;

            case MessageBoxResult.Cancel:
                await AddExtraLoadsAsync(listManager);
                break;
        }

        DaManager.Importing = false;
        vm.ListManager.SaveAll();
        vm.ListManager.CreateEquipmentList();
        foreach (var item in vm.ListManager.EqList) {
            item.Validate();
        }
        foreach (var item in vm.ListManager.CableList) {
            item.Validate(item);
        }
        vm.GetLoadList();
    }

    private async Task AddExtraLoadsAsync(ListManager listManager)
    {
        try {

            int count = listManager.LoadList.Count;
            ILoad load = new LoadModel() {
                Tag = "MTR-",
                Type = LoadTypes.MOTOR.ToString(),
                FedFromTag = "MCC-01",
                Voltage = 460,
                VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 460),
                VoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 460).Id,
                Size = 15,
                Unit = Units.HP.ToString()
            };

            for (int i = 0; i < 250; i++) {
                await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                    load.Tag = "M-" + count.ToString();
                    load.Area = listManager.AreaList[3];

                    LoadToAddValidator loadToAdd = new LoadToAddValidator(listManager, load);
                    LoadManager.AddLoad(loadToAdd, listManager);
                    load.CalculateLoading();
                    count += 1;
                }));

            }

        }
        catch (Exception) {

            throw;
        }
        finally {
            DaManager.Importing = false;
        }
    }

    private async Task AddTestDteq(ListManager listManager)
    {
        foreach (var dteq in TestData.TestDteqList_Full) {
            dteq.Area = listManager.AreaList[0];
            DteqToAddValidator dteqToAdd = new DteqToAddValidator(listManager, dteq);
            DteqManager.AddDteq(dteqToAdd, listManager);
        }

    }

    private async Task AddTestLoadsAsync(ListManager listManager, ObservableCollection<ILoad> loadList)
    {
        try {
            foreach (var load in loadList) {
                load.Area = listManager.AreaList[0];
                LoadToAddValidator loadToAdd = new LoadToAddValidator(listManager, load);
                await Task.Run(() => LoadManager.AddLoad(loadToAdd, listManager));
                //LoadManager.AddLoad(loadToAdd, listManager);

            }
        }
        catch (Exception ex) {
            NotificationHandler.ShowErrorMessage(ex);
        }
    }

    private void DeleteEquipment()
    {

        while (vm.ListManager.IDteqList.Count > 0) {
            IDteq dteq = vm.ListManager.IDteqList[0];
            vm.DeleteDteq(dteq);
        }

        while (vm.ListManager.LoadList.Count > 0) {
            vm.DeleteLoad(vm.ListManager.LoadList[0]);
        }

        DaManager.prjDb.DeleteAllRecords(GlobalConfig.ComponentTable);

    }

    public void DeleteEquipmentFromDatabase()
    {
        //Delete records
        DaManager.DeleteAllModelRecords();

        vm.DbGetAll();
    }

    #endregion

    #region View Modifiers
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
            vm.LoadGridTop = new System.Windows.GridLength(127, GridUnitType.Pixel);
        }
        else {
            AppSettings.Default.AddEquipmentPanelView = 2;
            AppSettings.Default.Save();
            gridAdding.Visibility = Visibility.Collapsed;
            vm.LoadGridTop = new System.Windows.GridLength(0, GridUnitType.Pixel);

        }

    }
    #endregion

    #region View Update Events
    public void OnElectricalViewUpdated(object source, EventArgs e)
    {
        string tag = "";
        if (vm == null) return;

        if (vm.SelectedEquipment != null) {
            tag = ((IEquipment)vm.SelectedEquipment).Tag;
        }
        
        vm.ListManager.CreateEquipmentList();
        if (tag != "") {
            vm.SelectedEquipment = vm.ListManager.EqList.FirstOrDefault(e => e.Tag == tag);
        }
        
    }
    #endregion
    private void eqView_Loaded(object sender, RoutedEventArgs e)
    {
       
    }

    private void LoadGridPaste(object sender, Syncfusion.UI.Xaml.Grid.GridCopyPasteEventArgs e)
    {
        //e.Handled = true;
    }

    private void dgdAssignedLoads_PasteGridCellContent(object sender, Syncfusion.UI.Xaml.Grid.GridCopyPasteCellEventArgs e)
    {
        if (e.Column.MappingName == "Id") e.Handled = true;
        if (e.Column.MappingName == "Tag") e.Handled = true;
        if (e.Column.MappingName == "IsValid") e.Handled = true;

        if (e.Column.MappingName == "ProtectionDevice.TripAmps") e.Handled = true;
        if (e.Column.MappingName == "ProtectionDevice.FrameAmps") e.Handled = true;
        if (e.Column.MappingName == "ProtectionDevice.StarterSize") e.Handled = true;


        //Components
        if (e.Column.MappingName == "Disconnect.Tag") e.Handled = true;
        if (e.Column.MappingName == "StandAloneStarter.Tag") e.Handled = true;
        if (e.Column.MappingName == "Lcs.Tag") e.Handled = true;
    }
}



