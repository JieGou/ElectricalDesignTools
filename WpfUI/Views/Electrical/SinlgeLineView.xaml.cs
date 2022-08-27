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

namespace WpfUI.Views.Electrical;
/// <summary>
/// Interaction logic for SinlgeLineView.xaml
/// </summary>
public partial class SinlgeLineView : UserControl
{
    private SingleLineViewModel vm { get { return DataContext as SingleLineViewModel; } }

    public SinlgeLineView()
    {
        InitializeComponent();

        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (vm != null) {
            if (vm.ViewableDteqList.Count > 0) {
                vm.SelectedDteq = vm.ViewableDteqList[0];
            }
        }
    }
}
