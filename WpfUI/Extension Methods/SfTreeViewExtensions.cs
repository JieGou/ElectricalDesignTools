using Syncfusion.UI.Xaml.TreeView.Engine;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.Extension_Methods;

internal static class SfTreeViewExtensions
{
    internal static void ExpandAllNodes(TreeViewNode node)
    {
        node.IsExpanded = true;
        foreach (var childNode in node.ChildNodes) {
            ExpandAllNodes(childNode);
        }
    }
}



