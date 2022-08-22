using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EDTLibrary.A_Helpers
{
    public class ErrorHelper
    {
        public static List<string> ErrorLog { get; set; } = new List<string>();

        public static void Log(string errorMessage, [CallerFilePath] string callerClass = "", [CallerMemberName] string callerMethod = "")
        {
            callerClass = Path.GetFileName(callerClass);
            errorMessage = $"C:{callerClass}, M:{callerMethod} - {errorMessage}";
            ErrorLog.Add(errorMessage);
            SaveLog();
        }
        public static void LogNoSave(string errorMessage, [CallerFilePath] string callerClass = "", [CallerMemberName] string callerMethod = "")
        {
            callerClass = Path.GetFileName(callerClass);
            errorMessage = $"C:{callerClass}, M:{callerMethod} - {errorMessage}";
            ErrorLog.Add(errorMessage);
        }
        public static void SaveLog()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += @"\Edt Error Log.txt";
            File.WriteAllLines(path, ErrorLog);
        }
        public static void ShowErrorMessage(Exception ex)
        {
#if DEBUG
            if (ex.Data.Contains("UserMessage")) {
                MessageBox.Show($"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n" +
                    $"Error: \n\n{ex.Message}\n\n\n" +
                                $"Stack Trace: \n\n {ex}", "Error");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
#endif

#if !DEBUG

            if (ex.Data.Contains("UserMessage")) {
                 MessageBox.Show($"{ex.Message}\n\n\n" +
                                $"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n", "Error");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
#endif



        }

        internal static void Notify(string message, string caption = "Error")
        {
            MessageBox.Show(message, caption);
        }
    }
}
