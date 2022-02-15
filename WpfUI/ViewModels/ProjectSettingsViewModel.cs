using EDTLibrary.DataAccess;
using EDTLibrary.ProjectSettings;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfUI.Commands;
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
            //LoadSettings();
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
            DataBaseService.SelectProject("C:\\");  
            //TODO = move SetSelectedPrject to DbService
            //SetSelctedProject();          
        }

        public void LoadSettings()
        {
            SettingManager.LoadProjectSettings();
            StringSettings = new ObservableCollection<SettingModel>(SettingManager.StringSettingList);
            TableSettings = new ObservableCollection<SettingModel>(SettingManager.TableSettingList);

            //StringSettings = new ObservableCollection<SettingModel>(SettingManager.GetStringSettings());
            //TableSettings = new ObservableCollection<SettingModel>(SettingManager.GetTableSettings());
            //SettingManager.CreateCableAmpsUsedInProject();
        }
        private void SaveStringSetting()
        {
            SettingManager.SaveStringSetting(SelectedStringSetting);
        }
        private void SaveTableSetting()
        {
            ArrayList tables = DbManager.prjDb.GetListOfTablesNamesInDb();

            //only saves to Db if Db table exists
            if (tables.Contains(SelectedTableSetting.Name)) {
                SettingManager.SaveTableSetting(SelectedTableSetting);
                SettingManager.CreateCableAmpsUsedInProject();
            }

        }
        #endregion
    }
}
