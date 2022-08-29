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

namespace WpfUI.Windows;
/// <summary>
/// Interaction logic for DebugWindow.xaml
/// </summary>
public partial class DebugWindow : Window
{
    public DebugWindow()
    {
        InitializeComponent();
      
    }

    private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
       
    }

    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        EDTLibrary.ErrorManagement.ErrorHelper.SaveLog();
    }

    private void btnClearUndo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        UndoManager.ClearUndoList();
    }
}
