using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfUI.ViewModels;

namespace WpfUI.Commands
{
    internal class SelectProjectCommand : CommandBase
    {
        private readonly ProjectSettingsViewModel _viewModel;
        public override void Execute(object? parameter)
        {
            SelectProject();
            //navigate to ProjectSettingsView
        }



        public static void SelectProject()
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "EDT files (*.db)|*.db|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    Settings.Default.ProjectDb = filePath;
                    Settings.Default.Save();                    
                }
            }

        }
    }
}
