using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.HelpMethods;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class StartupViewModel :ViewModelBase
    {
        #region Properties and Backing Fields

        private string _selectedProject;
        public string SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
            }
        }
        #endregion

        public StartupViewModel(NavigationStore navigationStore)
        {
            SetSelctedProject();

            OpenProjectCommand = new OpenProjectCommand(this, navigationStore);
            SelectProjectCommand = new RelayCommand(SelectProject);
        }
        
        public ICommand OpenProjectCommand { get; }
        public ICommand SelectProjectCommand { get; set; }


        public void SelectProject()
        {
            FileSystemHelper fs = new FileSystemHelper();
            string selectedProject = fs.SelectFile();

            Settings.Default.ProjectDb = selectedProject;
            Settings.Default.Save();
            SetSelctedProject();

        }

        public void SetSelctedProject()
        {
            _selectedProject = Settings.Default.ProjectDb;
            SelectedProject = Settings.Default.ProjectDb;
        }
    }
}
