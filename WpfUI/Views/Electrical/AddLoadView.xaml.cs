using EDTLibrary;
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

namespace WpfUI.Views.EquipmentSubViews;
/// <summary>
/// Interaction logic for AddLoadView.xaml
/// </summary>
public partial class AddLoadView : UserControl
{
    public AddLoadView()
    {
        InitializeComponent();
    }

    private void txtLoadTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (txtLoadTag.Text == "" || txtLoadTag.Text == GlobalConfig.EmptyTag) txtLoadTag.Text = "";
    }

    private void txtLoadTag_LostFocus(object sender, RoutedEventArgs e)
    {
        if (txtLoadTag.Text == "") txtLoadTag.Text = GlobalConfig.EmptyTag;
    }


    private void Control_KeyEnterSubmit(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) {
            
        }
    }

}

