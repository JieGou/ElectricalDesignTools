using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.UI.Xaml.TreeView.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WpfUI.Behaviors;
public class DteqTreeViewDagDropBehavior : TargetedTriggerAction<SfTreeView>
{
    private SfTreeView treeView;
    private DateTime droppingTime;

    protected override void Invoke(object parameter)
    {
        treeView = this.AssociatedObject as SfTreeView;
        treeView.Drop += OnTreeViewDrop;
    }

    protected override void OnDetaching()
    {
        if (treeView != null) {
            treeView.Drop -= OnTreeViewDrop;
            treeView = null;
        }
        base.OnDetaching();
    }

    
    private void OnTreeViewDrop(object sender, DragEventArgs e)
    {
        TreeViewItem treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);

        if (treeViewItem != null) {

            var item = treeViewItem.DataContext;
            if (item is TreeViewNode) {

                var dteqDropTarget = ((TreeViewNode)item).Content;
                if (dteqDropTarget == null) {
                    return;
                }
                var dataType = e.Data.GetData(DataFormats.Serializable);
                if (dataType == null) {
                    return;
                }
                if (dataType is IPowerConsumer) {
                    ((IPowerConsumer)dataType).FedFrom = ((DistributionEquipment)dteqDropTarget);
                }
            }

        }

    }

    private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        // Search the VisualTree for specified type
        while (current != null) {
            if (current is T) {
                return (T)current;
            }
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }
}