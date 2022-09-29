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
public partial class AddDteqView : UserControl
{
    public AddDteqView()
    {
        InitializeComponent();
    }

    private void txtDteqTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (txtDteqTag.Text == "" || txtDteqTag.Text == GlobalConfig.EmptyTag) txtDteqTag.Text = "";
    }
    private void txtDteqTag_LostFocus(object sender, RoutedEventArgs e)
    {
        if (txtDteqTag.Text == "") txtDteqTag.Text = GlobalConfig.EmptyTag;
    }

    private void btnAddDteq_MouseLeave(object sender, MouseEventArgs e)
    {
        
        Task.Run(() => resetTag());
        resetTag();
        async Task resetTag()
        {
            if (txtDteqTag.Text == "") {
                await Task.Delay(500);

                if (txtDteqTag.IsFocused == false)
                    txtDteqTag.Text = GlobalConfig.EmptyTag;
            }
        }
    }

    private void btnAddDteq_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (txtDteqTag.Text == GlobalConfig.EmptyTag) {
            txtDteqTag.Text = "";
        }
    }
}
