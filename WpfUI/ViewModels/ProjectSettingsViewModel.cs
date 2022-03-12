using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;

namespace WpfUI.ViewModels
{
    public class ProjectSettingsViewModel : ViewModelBase
    {

        #region Properties and Backing Fields

        public ObservableCollection<SettingModel> StringSettings { get; set; }
        public SettingModel SelectedStringSetting { get; set; }

        public ObservableCollection<CableTypeModel> CableTypes { get; set; }
        public ObservableCollection<CableSizeModel> SelectedCableSizes { get; set; } = new ObservableCollection<CableSizeModel>();
        private CableTypeModel _selectedCableType;



        public CableTypeModel SelectedCableType
        {
            get { return _selectedCableType; }
            set { 
                
               _selectedCableType = value;
                var selectedCableSizes = EdtSettings.CableSizesUsedInProject.Where(c => c.Type == _selectedCableType.Type );
                SelectedCableSizes.Clear();
                foreach (var cable in selectedCableSizes) {
                    SelectedCableSizes.Add(cable);
                }
            }
        }

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
            // Create commands

            ReloadSettingsCommand = new RelayCommand(LoadSettings);

            SaveStringSettingCommand = new RelayCommand(SaveStringSetting);
            SaveTableSettingCommand = new RelayCommand(SaveTableSetting);

            CableTypes = TypeManager.CableTypes;
        }

        #region Helper Methods
        public void LoadSettings()
        {
            SettingManager.LoadProjectSettings();
            StringSettings = new ObservableCollection<SettingModel>(SettingManager.StringSettingList);
            TableSettings = new ObservableCollection<SettingModel>(SettingManager.TableSettingList);

            //StringSettings = new ObservableCollection<SettingModel>(SettingManager.GetStringSettings());
            //TableSettings = new ObservableCollection<SettingModel>(SettingManager.GetTableSettings());
            //SettingManager.CreateCableAmpsUsedInProject();
            CableTypes = TypeManager.CableTypes;
        }
        private void SaveStringSetting()
        {
            SettingManager.SaveStringSetting(SelectedStringSetting);
        }
        private void SaveTableSetting()
        {
            ArrayList tables = DbManager.prjDb.GetListOfTablesNamesInDb();

            //only saves to Db if Db table exists

            if (SelectedTableSetting != null) {
                if (tables.Contains(SelectedTableSetting.Name)) {
                    SettingManager.SaveTableSetting(SelectedTableSetting);
                    //SettingManager.CreateCableAmpsUsedInProject();
                }
            }
            

            foreach (CableSizeModel cable in SelectedCableSizes) {
                DbManager.prjDb.UpsertRecord(cable, "CableSizesUsedInProject", new List<string>());
            }
            EdtSettings.CableSizesUsedInProject = DbManager.prjDb.GetRecords<CableSizeModel>("CableSizesUsedInProject");
        }
        #endregion
    }
}
