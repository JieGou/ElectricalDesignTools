using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModels.Home;

namespace WpfUI.UserControls.HomeControls;
/// <summary>
/// Interaction logic for PreviousProjectList.xaml
/// </summary>
public partial class PreviousProjectItem : UserControl
{
    private PreviousProject vm { get { return DataContext as PreviousProject; } }

    public PreviousProjectItem()
    {
        InitializeComponent();
    }

    private void StackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        vm.OpenProject();
    }
}
