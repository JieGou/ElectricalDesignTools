using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace WpfUI._Authentication;
/// <summary>
/// Interaction logic for AuthenticationMainWindow.xaml
/// </summary>
public partial class UserInfoWindow : Window
{

    public bool _isLoggedIn;
    public UserInfoWindow()
    {
        InitializeComponent();
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        
    }
}
