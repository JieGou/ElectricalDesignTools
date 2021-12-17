using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.HelpMethods;
using WpfUI.Models;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class ProjectSettingsViewModel : ViewModelBase
    {

        private string _selectedStringSetting;
        public string SelectedStringSetting 
        {
            get { return _selectedStringSetting;  }
            set 
            { 
                _selectedStringSetting = value;
                SelectedStringValue = SettingManager.GetStringSetting(_selectedStringSetting);
            }
        }
        public string SelectedStringValue { get; set; }
           


        public ObservableCollection<string> StringSettings { get; set; }
        public ObservableCollection<string> TableSettings { get; set; }

        public ObservableCollection<SettingModel> StringSettingsOC { get; set; }

        private SettingModel _selectedStringSettingOC;
        public SettingModel SelectedStringSettingOC { get; set; }
                   

        public string SelectedTableSetting { get; set; }


        #region Properties and Backing Fields
        private readonly ProjectFileStore _projectStore;

        //public string? ProjectName => _projectStore.SelectedProject?.Name;
        //public string? ProjectPath => _projectStore.SelectedProject?.Path;

        public NavigationBarViewModel NavigationBarViewModel { get; }
        #endregion


        #region Commands
        public ICommand NavigateStartupCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand SelectProjectCommand { get; }
        #endregion


        public ProjectSettingsViewModel()
        {
            LoadSettings();
            LoadSettingsOG();
            // Create commands
            SelectProjectCommand = new RelayCommand(SelectProject);

        }


        public ProjectSettingsViewModel(NavigationBarViewModel navigationBarViewModel, ProjectFileStore projectFileStore, NavigationService<StartupViewModel> startupNavigationService)
        {
            NavigationBarViewModel = navigationBarViewModel;
            _projectStore = projectFileStore;

            NavigateStartupCommand = new NavigateCommand<StartupViewModel>(startupNavigationService);

            // Create commands
            SelectProjectCommand = new RelayCommand(SelectProject);
        }

        #region Helper Methods
        public void SelectProject()
        {
            DataBaseService.SelectProject();  
            //TODO = move SetSelectedPrject to DbService
            //SetSelctedProject();          
        }

        public void LoadSettings()
        {
            SettingManager.GetSettings();
            StringSettingsOC = new ObservableCollection<SettingModel>(SettingManager.SettingList);
        }


        public void LoadSettingsOG()
        {
            Type prjStringSettings = typeof(EDTLibrary.ProjectSettings.Settings);

            StringSettings = new ObservableCollection<string>();
            TableSettings = new ObservableCollection<string>();
            StringSettings.Clear();
            TableSettings.Clear();

            foreach (var prop in prjStringSettings.GetProperties()) {
                if (prop.PropertyType.Name == "String") {
                    StringSettings.Add(prop.Name);
                }
                else if (prop.PropertyType.Name == "DataTable") {
                    TableSettings.Add(prop.Name);
                }
            }
        }
        #endregion
    }
}
