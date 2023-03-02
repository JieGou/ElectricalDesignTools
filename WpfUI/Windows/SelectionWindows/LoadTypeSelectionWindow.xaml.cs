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
using System.Windows.Shapes;
using WpfUI.ViewModels.Equipment;

namespace WpfUI.Windows.SelectionWindows;
/// <summary>
/// Interaction logic for FedFromSelectionWindow.xaml
/// </summary>
public partial class LoadTypeSelectionWindow : Window
{
    public LoadTypeSelectionWindow()
    {
        InitializeComponent();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        var dataContext = (EdtViewModelBase)this.DataContext;
        dataContext.CloseSelectionWindow();
    }

   
}
