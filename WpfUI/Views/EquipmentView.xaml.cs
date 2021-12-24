using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WinFormCoreUI;
using WpfUI.ViewModels;

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for EqView.xaml
    /// </summary>
    public partial class EquipmentView : UserControl
    {
        public EquipmentView()
        {
            InitializeComponent();
        }

        private void txtTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtTag.Text == "" || txtTag.Text == " ") txtTag.Text = "";
        }

        private void txtTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTag.Text == "") txtTag.Text = " ";
        }
    }
}
