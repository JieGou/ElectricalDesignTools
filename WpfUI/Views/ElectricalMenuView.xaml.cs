using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModels.Menus;
using WpfUI.Views.Electrical.MjeqSubviews;
using WpfUI.Windows;

namespace WpfUI.Views;

/// <summary>
/// Interaction logic for EqView.xaml
/// </summary>
public partial class ElectricalMenuView : UserControl
{
    private ElectricalMenuViewModel vm { get { return DataContext as ElectricalMenuViewModel; } }

    public ElectricalMenuView()
    {
        InitializeComponent();
       
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        //FlushWindowMEssageQueue();
        //var loadingWindow = LoadingWindow.CreateAsync(eqView);
        //vm.MainViewModel.CurrentViewModel = new MjeqViewModel(vm.ListManager);
        //FlushWindowMEssageQueue();
        //if (loadingWindow != null) {
        //    loadingWindow.Dispatcher.InvokeShutdown();

        //}
    }

    public static void FlushWindowMEssageQueue()
    {
        Application.Current.Dispatcher.Invoke(() => dummySub, DispatcherPriority.Background, null);
    }

    public static void dummySub()
    {

    }
}
