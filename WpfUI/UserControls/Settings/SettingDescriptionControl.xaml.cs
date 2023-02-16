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

namespace WpfUI.UserControls.Settings;
/// <summary>
/// Interaction logic for SettingDescription.xaml
/// </summary>
public partial class SettingDescriptionControl : UserControl
{


    public string SettingName
    {
        get { return (string)GetValue(SettingNameProperty); }
        set { SetValue(SettingNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SettingName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SettingNameProperty =
        DependencyProperty.Register("SettingName", typeof(string), typeof(SettingDescriptionControl), new PropertyMetadata("Setting Name"));



    public string SettingDescription
    {
        get { return (string)GetValue(SettingDescriptionProperty); }
        set { SetValue(SettingDescriptionProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SettingDescription.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SettingDescriptionProperty =
        DependencyProperty.Register("SettingDescriptionText", typeof(string), typeof(SettingDescriptionControl), new PropertyMetadata("Setting Description"));



    public double DescriptionWidth
    {
        get { return (double)GetValue(DescriptionWidthProperty); }
        set { SetValue(DescriptionWidthProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DescriptionWidth.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DescriptionWidthProperty =
        DependencyProperty.Register("DescriptionWidth", typeof(double), typeof(SettingDescriptionControl), new PropertyMetadata(350));



    public SettingDescriptionControl()
    {
        InitializeComponent();
    }
}
