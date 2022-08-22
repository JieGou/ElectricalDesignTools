using EDTLibrary.DataAccess;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.ErrorManagement
{
    [AddINotifyPropertyChangedInterface]
    public class ErrorHelper
    {
        public static ObservableCollection<string> ErrorLog { get; set; } = new ObservableCollection<string>();

        public static void LogAndSaveToFile(string errorMessage, [CallerFilePath] string callerClass = "", [CallerMemberName] string callerMethod = "")
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                callerClass = Path.GetFileName(callerClass);
                errorMessage = $"{ErrorLog.Count} - C:{callerClass}, M:{callerMethod} - {errorMessage}";
                ErrorLog.Add(errorMessage);
                SaveLog();
            }));

        }
        public static void Log(string errorMessage, [CallerFilePath] string callerClass = "", [CallerMemberName] string callerMethod = "")
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                callerClass = Path.GetFileName(callerClass);
                errorMessage = $"{ErrorLog.Count} - C:{callerClass}, M:{callerMethod} - {errorMessage}";
                ErrorLog.Add(errorMessage);
            }));

        }
        public static void SaveLog()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += @"\Edt Error Log.txt";
            File.WriteAllLines(path, ErrorLog);
        }

        internal static void NotifyExeptionMessage(Exception ex)
        {

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                if (ex.Data.Contains("UserMessage")) {
                    MessageBox.Show($"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n" +
                        $"Error: \n\n{ex.Message}\n\n\n" +
                                    $"Stack Trace: \n\n {ex}", "Error");
                }
                else {
                    MessageBox.Show(ex.ToString());
                }
            }));

        }

        internal static void NotifyUserError(string message, string caption = "User Error")
        {
            if (DaManager.Importing == true) return;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                MessageBox.Show(message, caption);
            }));
        }
    }
}
