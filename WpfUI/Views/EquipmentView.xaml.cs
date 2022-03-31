using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUI.Views.SubViews;

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for EqView.xaml
    /// </summary>
    public partial class EquipmentView : UserControl
    {
        DteqDetailView _dteqDetailsView = new DteqDetailView();
        LoadDetailView _loaDetailView = new LoadDetailView();
        public EquipmentView()
        {
            InitializeComponent();
        }

        private void txtDteqTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtDteqTag.Text == "" || txtDteqTag.Text == GlobalConfig.EmptyTag) txtDteqTag.Text = "";
        }

        private void txtDteqTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDteqTag.Text == "") txtDteqTag.Text = GlobalConfig.EmptyTag;
        }


        private void txtLoadTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtLoadTag.Text == "" || txtLoadTag.Text == GlobalConfig.EmptyTag) txtLoadTag.Text = "";
        }

        private void txtLoadTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtLoadTag.Text == "") txtLoadTag.Text = GlobalConfig.EmptyTag;
        }

        private void dgdDteq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) {
                dgdDteq.CancelEdit();
            }
        }

      

        private void dgdDteq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DteqDetailsContent.Content = null;
            if (dgdDteq.SelectedItem != null) {
                if (dgdDteq.SelectedItem.GetType().IsSubclassOf(typeof(DistributionEquipment))) {
                    _dteqDetailsView.DataContext = this.DataContext;
                    DteqDetailsContent.Content = _dteqDetailsView;
                }
            }
        }

        private void dgdAssignedLoads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgdAssignedLoads.SelectedItem != null) {
                if (dgdAssignedLoads.SelectedItem.GetType() == typeof(LoadModel)) {
                    _loaDetailView.DataContext = this.DataContext;
                    LoadDetailsContent.Content = _loaDetailView;
                }
            }
        }
    }
}
