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

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for LocationsView.xaml
    /// </summary>
    public partial class LocationsView : UserControl
    {
        public LocationsView()
        {
            InitializeComponent();
        }

        private void txtTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtLocationTag.Text == "" || txtLocationTag.Text == " ") txtLocationTag.Text = "";
        }

        private void txtTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLocationTag.Text == "") txtLocationTag.Text = " ";
        }
    }
}
