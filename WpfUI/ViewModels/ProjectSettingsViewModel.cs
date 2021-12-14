﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.HelpMethods;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class ProjectSettingsViewModel : ViewModelBase
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

        #region Commands
        public ICommand NavigateStartupCommand { get; }
        public ICommand SelectProjectCommand { get; set; }
        #endregion

        public ProjectSettingsViewModel(NavigationStore navigationStore)
        {
            _selectedProject = Settings.Default.ProjectDb;
            SelectedProject = Settings.Default.ProjectDb;

            NavigateStartupCommand = new NavigateCommand<StartupViewModel>(navigationStore, () => new StartupViewModel(navigationStore));

            // Create commands
            this.SelectProjectCommand = new RelayCommand(SelectProject);
        }


        #region Helper Methods
        public void SelectProject()
        {
            FileSystemHelper fs = new FileSystemHelper();
            string selectedProject = fs.SelectFile();
            
            Settings.Default.ProjectDb = selectedProject;
            Settings.Default.Save();
            SelectedProject = Settings.Default.ProjectDb;
        }
        #endregion
    }
}
