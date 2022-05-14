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
using WpfUI.UserControls.HomeControls;

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for StartupView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public bool _isExpanded = true;

        public HomeView()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard sb;
            if (_isExpanded) {
                sb = Application.Current.TryFindResource("gridin") as Storyboard;

            }
            else {
                sb = Application.Current.TryFindResource("gridout") as Storyboard;
            }
            //NewProjectControl npc = newProjectUc;
            //sb.Begin(npc);
            //_isExpanded = !_isExpanded;
        }
    }
}
