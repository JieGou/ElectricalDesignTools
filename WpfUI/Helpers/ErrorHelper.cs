using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI.Helpers
{
    public class ErrorHelper
    {
        public static void ErrorMessage(Exception ex)
        {
            if (ex.Data.Contains("UserMessage")) {
                MessageBox.Show($"Error: \n\n{ex.Message}\n\n\n" +
                $"Query: \n\n{ex.Data["UserMessage"]}\n\n\n" +
                $"Details: \n\n {ex}");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
