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
using WpfUI.Controls;

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


    public Visibility AreaVisibility
    {
        get { return (Visibility)GetValue(AreaVisibilityProperty); }
        set { SetValue(AreaVisibilityProperty, value); }
    }

    public static readonly DependencyProperty AreaVisibilityProperty =
        DependencyProperty.Register("AreaVisibility", typeof(Visibility), typeof(AddLoadView), new PropertyMetadata(Visibility.Visible));

    public Visibility FedFromVisibility
    {
        get { return (Visibility)GetValue(FedFromVisibilityProperty); }
        set { SetValue(FedFromVisibilityProperty, value); }
    }

    public static readonly DependencyProperty FedFromVisibilityProperty =
        DependencyProperty.Register("FedFromVisibility", typeof(Visibility), typeof(AddLoadView), new PropertyMetadata(Visibility.Collapsed));


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

    private void btnAddDteq_MouseLeave(object sender, MouseEventArgs e)
    {
        Task.Run(() => resetTag());
        resetTag();
        async Task resetTag()
        {
            if (txtLoadTag.Text == "") {
                await Task.Delay(500);

                if (txtLoadTag.IsFocused == false)
                    txtLoadTag.Text = GlobalConfig.EmptyTag;
            }
        }
    }

    private void btnAddDteq_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (txtLoadTag.Text == GlobalConfig.EmptyTag) {
            txtLoadTag.Text = "";
        }
    }
}

