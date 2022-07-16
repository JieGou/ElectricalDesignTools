using System.Windows.Controls;
using WpfUI.ViewModels;
using WpfUI.Views.Electrical.MjeqSubviews;

namespace WpfUI.Views;

/// <summary>
/// Interaction logic for EqView.xaml
/// </summary>
public partial class ElectricalMenuView : UserControl
{
    private ElectricalMenuViewModel elecVm { get { return DataContext as ElectricalMenuViewModel; } }

    public ElectricalMenuView()
    {
        InitializeComponent();
       
    }

}
