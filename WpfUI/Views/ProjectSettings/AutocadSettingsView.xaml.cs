using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
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
using WpfUI.ViewModels.Menus;

namespace WpfUI.Views.Settings;
/// <summary>
/// Interaction logic for GeneralSettingsView.xaml
/// </summary>
public partial class AutocadSettingsView : UserControl
{
    private SettingsMenuViewModel Vm { get { return DataContext as SettingsMenuViewModel; } }

    public AutocadSettingsView()
    {
        InitializeComponent();
    }

   
}
