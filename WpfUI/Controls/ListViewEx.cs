using System.Windows;
using System.Windows.Controls;

namespace WpfUI.Controls;
public partial class ListViewEx : ListView
{
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new ListViewItemEx();
    }
}