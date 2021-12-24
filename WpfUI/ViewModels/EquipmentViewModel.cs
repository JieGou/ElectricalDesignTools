using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinFormCoreUI;
using WpfUI.Commands;
using WpfUI.Services;
using WpfUI.Stores;
using WpfUI.ValidationRules;

namespace WpfUI.ViewModels {

    public class EquipmentViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        #region Properties
        public NavigationBarViewModel NavigationBarViewModel { get; }



        // DTEQ
        private ObservableCollection<DteqModel> _dteqList = new ObservableCollection<DteqModel>();
        public ObservableCollection<DteqModel> DteqList
        {
            get { return _dteqList;  }

            set
            {
                _dteqList = value;
            }
        }


        private DteqModel _selectedDteq;
        public DteqModel SelectedDteq
        {
            get { return _selectedDteq; }
            set
            {
                _selectedDteq = value;
                LoadListLoaded = false;

                if (_selectedDteq != null) {
                    AssignedLoads = new ObservableCollection<ILoadModel>(_selectedDteq.AssignedLoads);
                }

            }
        }

        public List<string> DteqTypes { get; set; } = new List<string>();



        private ObservableCollection<string> _fedFromList;
        public ObservableCollection<string> FedFromList
        {
            get
            {
                _fedFromList = new ObservableCollection<string>(DteqList.Select(dteq => dteq.Tag).ToList());
                _fedFromList.Insert(0, "UTILITY");
                return _fedFromList;
            }
            set { }
        }

        private string _dteqTagToAdd;
        public ObservableCollection<ILoadModel> AssignedLoads { get; set; } = new ObservableCollection<ILoadModel> { };

        public string DteqTagToAdd
        {
            get { return _dteqTagToAdd; }
            set
            {
                _dteqTagToAdd = value;

                ClearErrors(nameof(DteqTagToAdd));
                if (IsTagAvailable(value) == false) {
                    AddError(nameof(DteqTagToAdd), "Tag already exists");
                }
                else if (value == "") { // TODO - create method for invalid tags
                    AddError(nameof(DteqTagToAdd), "Tag cannot be empty");
                }
            }
        }
        public string Test { get; set; }

       

        // LOADS
        private ObservableCollection<LoadModel> _loadList = new ObservableCollection<LoadModel>();
        public ObservableCollection<LoadModel> LoadList { get; set; }

        public bool LoadListLoaded { get; set; }


        public ObservableCollection<ILoadModel> MasterLoadList { get; set; }
        #endregion

        private void Initialize()
        {
            foreach (var item in Enum.GetNames(typeof(EDTLibrary.DteqTypes))) {
                DteqTypes.Add(item.ToString());
            }

            //Instatiates the required properties
            //TODO = FigureOut MasterLoad List

            MasterLoadList = new ObservableCollection<ILoadModel>();
        }




        #region Public Commands

        // Equipment Commands
        public ICommand GetDteqCommand { get; }
        public ICommand CalculateDteqCommand { get; }
        public ICommand SaveDteqListCommand { get; }
        public ICommand DeleteDteqListCommand { get; }


        public ICommand AddDteqCommand { get; }


        // Load Commands
        public ICommand ShowLoadListCommand { get; }
        public ICommand SaveLoadListCommand { get; }

        #endregion


        #region Constructor
        public EquipmentViewModel()
        {
            
            // Create commands
            GetDteqCommand = new RelayCommand(GetDteq);
            CalculateDteqCommand = new RelayCommand(CalculateDteq);
            SaveDteqListCommand = new RelayCommand(SaveDteq);
            DeleteDteqListCommand = new RelayCommand(DeleteDteq);
            AddDteqCommand = new RelayCommand(AddDteq);


            ShowLoadListCommand = new RelayCommand(ShowLoadList);
            SaveLoadListCommand = new RelayCommand(SaveLoadList);

            Initialize();

        }

       









        /// <summary>
        /// Default Constructor
        /// </summary>
        public EquipmentViewModel(NavigationBarViewModel navigationBarViewModel, NavigationStore navigationStore)
        {
            NavigationBarViewModel = navigationBarViewModel;

            // Create commands
            AddDteqCommand = new RelayCommand(AddDteq);
            CalculateDteqCommand = new RelayCommand(CalculateDteq);

            Initialize();
        }


        #endregion

        #region Error Validation

        // INotifyDataErrorInfo
        public bool HasErrors => _errorDict.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public readonly Dictionary<string, List<string>> _errorDict  = new Dictionary<string, List<string>>();

        private void ClearErrors(string propertyName) {
            _errorDict.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        public void AddError(string propertyName, string errorMessage) {
            if (!_errorDict.ContainsKey(propertyName)) { // check if error Key exists
                _errorDict.Add(propertyName, new List<string>()); // create if not
            }
            _errorDict[propertyName].Add(errorMessage); //add error message to list of error messages
            OnErrorsChanged(propertyName);
        }

        public IEnumerable GetErrors(string? propertyName) {
            return _errorDict.GetValueOrDefault(propertyName, null);
        }

        private void OnErrorsChanged(string? propertyName) {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        public string Error { get; }

        #endregion


       



        #region Command Methods

        // Dteq
        private void GetDteq() {
            DteqList = new ObservableCollection<DteqModel>(DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.dteqListTable));
        }
        private void CalculateDteq()
        {

            ListManager.AssignLoadsToDteq(DteqList, LoadList);
            foreach (var dteq in DteqList) {
                dteq.CalculateLoading();
            }
        }
        private void SaveDteq()
        {
            //if (DteqList.Count !=0) {
            //    DbManager.prjDb.DeleteAllRecords(GlobalConfig.dteqListTable);
            //    foreach (var dteq in DteqList) {
            //        DbManager.prjDb.InsertRecord<DteqModel>(dteq, GlobalConfig.dteqListTable, SaveLists.DteqSaveList);
            //    }
            //}

            foreach (var dteq in DteqList) {
                DbManager.prjDb.UpsertRecord<DteqModel>(dteq, GlobalConfig.dteqListTable, SaveLists.DteqSaveList);
            }
            CalculateDteq();
        }
        private void DeleteDteq()
        {
            if (_selectedDteq !=null) {
                DbManager.prjDb.DeleteRecord(GlobalConfig.dteqListTable, _selectedDteq.Id);
                DteqList.Remove(_selectedDteq);
            }
        }
        private void AddDteq()
        {
            // TODO - methods for invalid tags
            if (IsTagAvailable(_dteqTagToAdd) && _dteqTagToAdd != "" && _dteqTagToAdd != " " && _dteqTagToAdd != null) {
                DteqList.Add(new DteqModel() { Tag = _dteqTagToAdd });

                CreateMasterLoadList();
                DictionaryStore.CreateDteqDict(DteqList);

                BuildFedFromList();
            }
        }


        // Loads
        private void ShowLoadList()
        {
            LoadListLoaded = true;
            //LoadList = new ObservableCollection<LoadModel>(DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.loadListTable));

            AssignedLoads.Clear();
            foreach (var load in LoadList) {
                AssignedLoads.Add(load);
            }
        }
        private void SaveLoadList()
        {
            if (LoadList.Count != 0 && LoadListLoaded==true) {
                DbManager.prjDb.DeleteAllRecords(GlobalConfig.loadListTable);
                foreach (var load in LoadList) {
                    DbManager.prjDb.InsertRecord<LoadModel>(load, GlobalConfig.loadListTable, SaveLists.LoadSaveList);
                }
            }
            CalculateDteq();
        }

        #endregion

        #region Helper Methods
        private void BuildFedFromList()
        {
            _fedFromList = new ObservableCollection<string>(DteqList.Select(dteq => dteq.Tag).ToList());
            _fedFromList.Insert(0, "UTILITY");
        }

        private bool IsTagAvailable(string tag) {
            if (tag == null) {
                return false;
            }
            var dteqTag = DteqList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            var loadTag = LoadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());

            if (dteqTag != null || 
                loadTag != null) {
                return false;
            }
            
            return true;
        }

        private void CreateMasterLoadList() {
            MasterLoadList.Clear();
            foreach (var dteq in DteqList) {
                MasterLoadList.Add(dteq);
            }
            foreach (var load in LoadList) {
                MasterLoadList.Add(load);
            }
        }

        #endregion

    }

}
