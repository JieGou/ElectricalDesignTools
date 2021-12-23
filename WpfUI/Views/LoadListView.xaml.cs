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
    /// Interaction logic for LoadListView.xaml
    /// </summary>
    public partial class LoadListView : UserControl
    {
        public LoadListView()
        {
            InitializeComponent();
        }

        private void txtTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtTag.Text == "") txtTag.Text = "";
        }
    }
}
