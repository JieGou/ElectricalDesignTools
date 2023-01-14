using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfUI.PopupWindows;
/// <summary>
/// Interaction logic for NotificationPopup.xaml
/// </summary>
public partial class NotificationPopup : Window
{
    public static MainWindow MainWindow { get; set; }

    public NotificationPopup()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SetWindowScreen(this, GetWindowScreen(App.Current.MainWindow));

    }
    public void SetWindowScreen(Window window, Screen screen)
    {
        if (screen != null) {
            if (!window.IsLoaded) {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            var workingArea = screen.WorkingArea;
            window.Left = workingArea.Left + workingArea.Width - this.Width - 10;
            window.Top = workingArea.Top + workingArea.Height - this.Height - 10;
        }
    }
    public Screen GetWindowScreen(Window window)
    {
        return Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
    }
    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.Close();
    }

}
