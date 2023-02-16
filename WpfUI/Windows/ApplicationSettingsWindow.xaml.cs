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
using System.Windows.Shapes;

namespace WpfUI.Windows;
/// <summary>
/// Interaction logic for ApplicationSettingsWindow.xaml
/// </summary>
public partial class ApplicationSettingsWindow : Window
{
    public ApplicationSettingsWindow()
    {
        InitializeComponent();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        AppSettings.Default.Save();
    }
}
