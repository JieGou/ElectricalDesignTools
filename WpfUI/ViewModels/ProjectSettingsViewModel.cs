using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class ProjectSettingsViewModel : ViewModelBase
    {
        public ProjectSettingsViewModel(NavigationStore navigationStore) {

            // Create commands
            this.SelectProjectCommand = new RelayCommand(SelectProject);
        }

        #region Properties
        public string CurrentProject { get; set; }
        #endregion

        #region Commands
        public ICommand SelectProjectCommand { get; set; }
        #endregion

        #region Helper Methods
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
        #endregion
    }
}
