using Dia2Lib;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.UI.Xaml.TreeView.Engine;
using Syncfusion.Windows.Controls.Input.Resources;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfUI.Extension_Methods;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Electrical;
using WpfUI.Views.Electrical.MjeqSubviews;
using TreeViewItem = Syncfusion.UI.Xaml.TreeView.TreeViewItem;

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for SinlgeLineView.xaml
/// </summary>
/// 
public partial class SinlgeLineView : UserControl
{
    private SingleLineViewModel vm { get { return DataContext as SingleLineViewModel; } }


    public SinlgeLineView()
    {
        InitializeComponent();

        _propertyPaneWidth = PropertyPaneColumn.Width.Value;
        if (vm!=null) {
            vm.DteqCollectionView = new ListCollectionView(vm.ViewableDteqList);
        }
        _ViewStateManager.ElectricalViewUpdate += OnElectricalViewUpdated;


    }

    //Sets the datacontext for the details view panel on the right

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (vm != null) {
            if (vm.SelectedDteq == null && vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = (IDteq)vm.ViewableDteqList[0];
            }
        }
    }


    //Sets the datacontext for the details view panel on the right

    private void LoadGraphicView_EquipmentSelected(object sender, RoutedEventArgs e)
    {

        SingleLineViewModel slVm = (SingleLineViewModel)DataContext;
        slVm.SelectedLoadEquipment = (IEquipment)e.OriginalSource;
        slVm.IsSelectedLoadCable = false;

        slVm.ClearSelections();
        //ClearSelections(slVm.ListManager);

        slVm.SelectedLoadEquipment.IsSelected = true;

    }

    private void LoadGraphicView_EquipmentCableSelected(object sender, RoutedEventArgs e)
    {
        SingleLineViewModel slVm = (SingleLineViewModel)DataContext;
        slVm.SelectedLoadCable = (IEquipment)e.OriginalSource;
        slVm.IsSelectedLoadCable = true;

        slVm.ClearSelections();
        //ClearSelections(slVm.ListManager);

        

        if (slVm.SelectedLoadCable.GetType() == typeof(LoadModel)) {
            var load = (LoadModel)slVm.SelectedLoadCable;
            if (load.PowerCable != null) {
                load.PowerCable.IsSelected = true; 
            }
        }
        else if (slVm.SelectedLoadCable.GetType() == typeof(ComponentModel)) {

            var comp = (ComponentModel)slVm.SelectedLoadCable;
            if (comp.PowerCable != null) {
                comp.PowerCable.IsSelected = true;

            }
        }

        else if (slVm.SelectedLoadCable is DistributionEquipment) {

            var dteq = DteqFactory.Recast(slVm.SelectedLoadCable);
            if (dteq.PowerCable != null) {
                dteq.PowerCable.IsSelected = true;

            }
        }

    }

    //private static void ClearSelections(ListManager listManager)
    //{
    //    listManager.CreateEquipmentList();
    //    foreach (var item in listManager.EqList) {
    //        item.IsSelected = false;
    //    }

    //    foreach (var item in listManager.CableList) {
    //        item.IsSelected = false;
    //    }

    //}




    #region Data Members For Drag selection

    /// <summary>
    /// Set to 'true' when the left mouse-button is down.
    /// </summary>
    private bool isLeftMouseButtonDownOnTarget = false;

    /// <summary>
    /// Set to 'true' when dragging the 'selection rectangle'.
    /// Dragging of the selection rectangle only starts when the left mouse-button is held down and the mouse-cursor
    /// is moved more than a threshold distance.
    /// </summary>
    private bool isDraggingSelectionRect = false;

    /// <summary>
    /// Records the location of the mouse (relative to the window) when the left-mouse button has pressed down.
    /// </summary>
    private Point origMouseDownPoint;

    /// <summary>
    /// The threshold distance the mouse-cursor must move before drag-selection begins.
    /// </summary>
    private static readonly double DragThreshold = 5;

    /// <summary>
    /// Set to 'true' when the left mouse-button is held down on a rectangle.
    /// </summary>
    private bool isLeftMouseDownOnRectangle = false;

    /// <summary>
    /// Set to 'true' when the left mouse-button and control are held down on a rectangle.
    /// </summary>
    private bool isLeftMouseAndControlDownOnRectangle = false;

    /// <summary>
    /// Set to 'true' when dragging a rectangle.
    /// </summary>
    private bool isDraggingRectangle = false;

    #endregion Data Members

    #region Drag Selection
    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DragEvent_MouseDown(e, grdSingleLine);
    }
    
    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        DragEvent_MouseUp(e, grdSingleLine);
    }

    private void DragEvent_MouseDown(MouseButtonEventArgs e, UIElement uIElement)
    {
        if (e.ChangedButton == MouseButton.Left) {

            //
            //  Clear selection immediately when starting drag selection.
            //

            isLeftMouseButtonDownOnTarget = true;
            origMouseDownPoint = e.GetPosition(uIElement);

            uIElement.CaptureMouse();

            e.Handled = true;
        }
    }
    private void DragEvent_MouseMove(MouseEventArgs e, UIElement uIElement)
    {
        txtMousePos.Text = $"Pos - X: {Math.Round(e.GetPosition(grdSingleLine).X,2)}, Y: {e.GetPosition(grdSingleLine).Y}";
        if (isDraggingSelectionRect) {
            //
            // Drag selection is in progress.
            //
            Point curMouseDownPoint = e.GetPosition(uIElement);
            UpdateDragSelectionRect(origMouseDownPoint, curMouseDownPoint);

            e.Handled = true;
        }
        else if (isLeftMouseButtonDownOnTarget) {
            //
            // The user is left-dragging the mouse,
            // but don't initiate drag selection until
            // they have dragged past the threshold value.
            //
            Point curMouseDownPoint = e.GetPosition(uIElement);
            var dragDelta = curMouseDownPoint - origMouseDownPoint;
            double dragDistance = Math.Abs(dragDelta.Length);
            if (dragDistance > DragThreshold) {
                //
                // When the mouse has been dragged more than the threshold value commence drag selection.
                //
                isDraggingSelectionRect = true;
                InitDragSelectionRect(origMouseDownPoint, curMouseDownPoint);
            }

            e.Handled = true;
        }
    }
    private void DragEvent_MouseUp(MouseButtonEventArgs e, UIElement uIElement)
    {
        if (e.ChangedButton == MouseButton.Left) {
            bool wasDragSelectionApplied = false;

            if (isDraggingSelectionRect) {
                //
                // Drag selection has ended, apply the 'selection rectangle'.
                //

                isDraggingSelectionRect = false;
                ApplyDragSelectionRect();

                e.Handled = true;
                wasDragSelectionApplied = true;
            }

            if (isLeftMouseButtonDownOnTarget) {
                isLeftMouseButtonDownOnTarget = false;
                uIElement.ReleaseMouseCapture();

                e.Handled = true;
            }

            if (!wasDragSelectionApplied) {
                //
                // A click and release in empty space clears the selection.
                //

               // listViewLoads.SelectedItems.Clear();
            }
        }
    }

    private void InitDragSelectionRect(Point pt1, Point pt2)
    {
        UpdateDragSelectionRect(pt1, pt2);

        dragSelectionCanvas.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Update the position and size of the rectangle used for drag selection.
    /// </summary>
    private void UpdateDragSelectionRect(Point pt1, Point pt2)
    {
        double x, y, width, height;

        //
        // Determine x,y,width and height of the rect inverting the points if necessary.
        // 

        if (pt2.X < pt1.X) {
            x = pt2.X;
            width = pt1.X - pt2.X;
        }
        else {
            x = pt1.X;
            width = pt2.X - pt1.X;
        }

        if (pt2.Y < pt1.Y) {
            y = pt2.Y;
            height = pt1.Y - pt2.Y;
        }
        else {
            y = pt1.Y;
            height = pt2.Y - pt1.Y;
        }

        //
        // Update the coordinates of the rectangle used for drag selection.
        //
        Canvas.SetLeft(dragSelectionBorder, x);
        Canvas.SetTop(dragSelectionBorder, y);
        dragSelectionBorder.Width = width;
        dragSelectionBorder.Height = height;
    }

    /// <summary>
    /// Select all nodes that are in the drag selection rectangle.
    /// </summary>
    private void ApplyDragSelectionRect()
    {
        dragSelectionCanvas.Visibility = Visibility.Collapsed;

        double x = Canvas.GetLeft(dragSelectionBorder);
        double y = Canvas.GetTop(dragSelectionBorder);
        double width = dragSelectionBorder.Width;
        double height = dragSelectionBorder.Height;
        Rect dragRect = new Rect(x, y, width, height);

        //
        // Inflate the drag selection-rectangle by 1/10 of its size to 
        // make sure the intended item is selected.
        //
        dragRect.Inflate(width / 10, height / 10);

        //
        // Clear the current selection.
        //
        
        listViewLoads.SelectedItems.Clear();



        //
        // Find and select all the list box items.
        //

        //foreach (RectangleViewModel rectangleViewModel in this.ViewModel.Rectangles) {
        //    Rect itemRect = new Rect(rectangleViewModel.X, rectangleViewModel.Y, rectangleViewModel.Width, rectangleViewModel.Height);
        //    if (dragRect.Contains(itemRect)) {
        //        listBox.SelectedItems.Add(rectangleViewModel);
        //    }
        //}
        
        ScrollViewer scrollViewer = GetScrollViewer(listViewLoads);
        var uiElement = listViewLoads.ItemContainerGenerator.ContainerFromItem(listViewLoads);

        foreach (var item in listViewLoads.Items) {
            var container = listViewLoads.ItemContainerGenerator.ContainerFromItem(item); 
            FrameworkElement element = container as FrameworkElement;

            if (element != null) {
                
                var transform = element.TransformToVisual(scrollViewer);
                var positionInScrollViewer = transform.Transform(new Point(0, 0));
                Rect itemRect = new Rect(positionInScrollViewer.X, 145, 100, 90);
                if (dragRect.Contains(itemRect)) {
                    listViewLoads.SelectedItems.Add(item);
                }
            }
        }


    }

    #endregion

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (vm == null) return;
        vm.SelectedLoads = new ObservableCollection<IPowerConsumer>(listViewLoads.SelectedItems.Cast<IPowerConsumer>().ToList());
    }

    public static ScrollViewer GetScrollViewer(DependencyObject depObj)
    {
        var obj = depObj as ScrollViewer;
        if (obj != null) return obj;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
            var child = VisualTreeHelper.GetChild(depObj, i);

            var result = GetScrollViewer(child);
            if (result != null) return result;
        }
        return null;
    }

    private void EqList_SelectionChanged(object sender, Syncfusion.UI.Xaml.TreeView.ItemSelectionChangedEventArgs e)
    {
        
    }

    private void listViewLoads_LayoutUpdated(object sender, EventArgs e)
    {
        ScrollViewer sv = listViewLoads.GetChildOfType<ScrollViewer>();
        if (sv != null) {
            listViewLoadsBorderWidth.Width = sv.ScrollableWidth + sv.ViewportWidth;
        }
    }

    private void ExternalScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        ScrollViewer sv = listViewLoads.GetChildOfType<ScrollViewer>();
        sv.ScrollToHorizontalOffset(e.HorizontalOffset);
    }

    double _propertyPaneWidth = 0;
    private void GridSplitter_TogglePropertyPane(object sender, RoutedEventArgs e)
    {
        var width = PropertyPaneColumn.Width.Value == 0 ? _propertyPaneWidth : 0;
        PropertyPaneColumn.Width = new GridLength(width);
    }

    private void SaveGridWidth(object sender, MouseButtonEventArgs e)
    {
        _propertyPaneWidth = PropertyPaneColumn.Width.Value;    
    }

    private void sfTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        vm.SelectedDteq = (DistributionEquipment)sfTreeView.SelectedItem;
    }

    private void singleLine_ScrollViewer_H_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var sv = (ScrollViewer)sender;
        ExternalScrollViewer_Horizontal.ScrollToHorizontalOffset(ExternalScrollViewer_Horizontal.HorizontalOffset - e.Delta);
    }

    private void sfTreeView_SourceUpdated(object sender, DataTransferEventArgs e)
    {
        var treeView = (SfTreeView)sender;
        
    }

    public void OnElectricalViewUpdated(object source, EventArgs e)
    {
        sfTreeView.ExpandAll();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        _ViewStateManager.ElectricalViewUpdate -= OnElectricalViewUpdated;

    }

    private void listViewLoads_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {

            if (e.Key == Key.V) {
                vm.AddLoad(vm.SelectedLoad);
            }
        }
    }
    private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        DragEvent_MouseMove(e, grdSingleLine);
        string senderType = sender.GetType().ToString();
        //drag drop
        if (e.LeftButton == MouseButtonState.Pressed) {

            var listView = sender as ListView;

            if (listView == null  || listView.SelectedItem == null) return;
            var draggedItem = listView.SelectedItem;

            if (draggedItem != null) {
                DragDrop.DoDragDrop(listView, new DataObject(DataFormats.Serializable, draggedItem), DragDropEffects.Link);
            }
        }
    }
    private void sfTreeView_PreviewDrop(object sender, DragEventArgs e)
    {
        
        
    }

    
}

