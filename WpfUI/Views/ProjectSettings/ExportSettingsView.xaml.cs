using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Settings;

namespace WpfUI.Views.Settings;
/// <summary>
/// Interaction logic for GeneralSettingsView.xaml
/// </summary>
public partial class ExportSettingsView : UserControl
{
    private ExportSettingsViewModel vm { get { return DataContext as ExportSettingsViewModel; } }

    public ExportSettingsView()
    {
        InitializeComponent();
    }


    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        vm.SaveMapping();

    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        vm.SaveMapping();

    }
}
