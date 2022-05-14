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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI.UserControls.HomeControls;
/// <summary>
/// Interaction logic for NewProjectControl.xaml
/// </summary>
public partial class NewProjectControl : UserControl
{
    public bool _isExpanded = true;

    public NewProjectControl()
    {
        InitializeComponent();
    }

   

    private void hideButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        Storyboard sb;
        if (_isExpanded) {
            sb = this.FindResource("gridin") as Storyboard;

        }
        else {
            sb = this.FindResource("gridout") as Storyboard;
        }
        sb.Begin(this);
        _isExpanded = !_isExpanded;
    }
}
