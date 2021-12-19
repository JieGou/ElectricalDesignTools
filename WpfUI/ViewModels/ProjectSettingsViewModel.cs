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

        #region Properties and Backing Fields

        public string TestString { get; set; }
        public ObservableCollection<SettingModel> StringSettings { get; set; }
        public SettingModel SelectedStringSetting { get; set; }


        public ObservableCollection<SettingModel> TableSettings { get; set; }
        public SettingModel SelectedTableSetting { get; set; }


        private readonly ProjectFileStore _projectStore;

        //public string? ProjectName => _projectStore.SelectedProject?.Name;
        //public string? ProjectPath => _projectStore.SelectedProject?.Path;

        public NavigationBarViewModel NavigationBarViewModel { get; }
        #endregion


        #region Commands
        public ICommand TestCommand { get; }


        public ICommand NavigateStartupCommand { get; }
        public ICommand OpenProjectCommand { get; }

        public ICommand SelectProjectCommand { get; }
        public ICommand ReloadSettingsCommand { get; }

        public ICommand SaveStringSettingCommand { get; }
        public ICommand SaveTableSettingCommand { get; }

        #endregion


        public ProjectSettingsViewModel()
        {
            LoadSettings();
            // Create commands

            SelectProjectCommand = new RelayCommand(SelectProject);

            ReloadSettingsCommand = new RelayCommand(LoadSettings);

            SaveStringSettingCommand = new RelayCommand(SaveStringSetting);
            SaveTableSettingCommand = new RelayCommand(SaveTableSetting);

            TestCommand = new RelayCommand(Test);

        }

        private void Test()
        {
            SelectedTableSetting.TableValue.Rows[0][2] = false;
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
            //SettingManager.GetSettings();
            //StringSettings = new ObservableCollection<SettingModel>(SettingManager.StringSettingList);
            //TableSettings = new ObservableCollection<SettingModel>(SettingManager.TableSettingList);

            StringSettings = new ObservableCollection<SettingModel>(SettingManager.GetStringSettings());
            TableSettings = new ObservableCollection<SettingModel>(SettingManager.GetTableSettings());
        }
        private void SaveStringSetting()
        {
            SettingManager.SaveStringSetting(SelectedStringSetting);
        }
        private void SaveTableSetting()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in SelectedTableSetting.TableValue.Rows) {
                foreach (var item in row.ItemArray) {
                   sb.Append($"{item.ToString()} ");
                }            
                sb.AppendLine("\n");
            }
            TestString = sb.ToString();
            SettingManager.SaveTableSetting(SelectedTableSetting);
        }



        #endregion
    }
}
