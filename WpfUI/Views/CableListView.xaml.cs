using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WpfUI.ViewModels;

namespace WpfUI.Views;

/// <summary>
/// Interaction logic for CableListView.xaml
/// </summary>
public partial class CableListView : UserControl
{
    private CableListViewModel cblVm { get { return DataContext as CableListViewModel; } }

    public CableListView()
    {
        InitializeComponent();
    }

    private void Expander_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //Expander expander = (Expander)sender;
        //if (expander.IsExpanded) {
        //    expander.IsExpanded = false;
        //}
        //else if (expander.IsExpanded == false) {
        //    expander.IsExpanded = true;
        //}
    }
}



