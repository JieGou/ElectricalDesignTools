using EDTLibrary.Models;
using EDTLibrary.UndoSystem;
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
using WpfUI.ViewModels.Debug;

namespace WpfUI.Windows;
/// <summary>
/// Interaction logic for DebugWindow.xaml
/// </summary>
public partial class DebugWindow : Window
{
    DebugViewModel dataContextVm;
    public DebugWindow()
    {
        InitializeComponent();
        
        this.DataContext = new DebugViewModel(new UndoManager());
        //    lstDebug.Items.Clear();
        //    foreach (var item in UndoManager.UndoList) {
        //        lstDebug.Items.Add(item.ToString());
        //    }
    }

    private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //lstDebug.Items.Clear();
        //foreach (var item in UndoManager.UndoList) {
        //    lstDebug.Items.Add(item.ToString());
        //}
    }

}
