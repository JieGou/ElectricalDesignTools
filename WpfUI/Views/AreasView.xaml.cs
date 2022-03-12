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
    /// Interaction logic for AreasView.xaml
    /// </summary>
    public partial class AreasView : UserControl
    {
        public AreasView()
        {
            InitializeComponent();
        }
        private void txtTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtAreaTag.Text == "" || txtAreaTag.Text == " ") txtAreaTag.Text = "";
        }

        private void txtTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAreaTag.Text == "") txtAreaTag.Text = " ";
        }
    }
}
