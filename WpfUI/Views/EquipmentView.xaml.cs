using EDTLibrary.Models.DistributionEquipment;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void txtDteqTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtDteqTag.Text == "" || txtDteqTag.Text == " ") txtDteqTag.Text = "";
        }

        private void txtDteqTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDteqTag.Text == "") txtDteqTag.Text = " ";
        }


        private void txtLoadTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtLoadTag.Text == "" || txtLoadTag.Text == " ") txtLoadTag.Text = "";
        }

        private void txtLoadTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLoadTag.Text == "") txtLoadTag.Text = " ";
        }

        private void dgdDteq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) {
                dgdDteq.CancelEdit();
            }
        }

        private void dgdDteq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgdDteq.SelectedItem != null) {
                if (dgdDteq.SelectedItem.GetType() == typeof(DteqModel)) {
                    TestDteqView dteqDetailsView = new TestDteqView();
                    dteqDetailsView.DataContext = this.DataContext;
                    DteqDetailsContent.Content = dteqDetailsView;
                }
            }
        }
    }
}
