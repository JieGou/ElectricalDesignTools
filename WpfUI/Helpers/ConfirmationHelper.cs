using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI.Helpers;
internal class ConfirmationHelper
{
    internal static bool Confirm(string message)
    {
        MessageBoxResult result = MessageBox.Show(message,
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes) {
            return true; 
        } 
        return false;  
    }
}
