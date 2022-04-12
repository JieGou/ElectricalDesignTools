﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUI.Helpers
{
    public class ErrorHelper
    {
        public static void SqlErrorMessage(Exception ex)
        {
            if (ex.Data.Contains("UserMessage")) {
                MessageBox.Show($"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n" +
                   $"Error: \n\n{ex.Message}\n\n\n" +
                   $"Stack Trace: \n\n {ex}");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void EdtErrorMessage(Exception ex)
        {
            if (ex.Data.Contains("UserMessage")) {
                MessageBox.Show($"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n" + 
                    $"Error: \n\n{ex.Message}\n\n\n" +
                    $"Stack Trace: \n\n {ex}");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
