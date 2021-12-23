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

        private DteqModel _selectedDteq;

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
        public DteqModel SelectedDteq { get => _selectedDteq; set => _selectedDteq = value; }

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
        //TODO = FigureOut MasterLoad List
        public ObservableCollection<DteqModel> DteqList { get; set; }
        
        public ObservableCollection<LoadModel> LoadList { get; set; }
        public ObservableCollection<ILoadModel> MasterLoadList { get; set; }
        #endregion

        private void Initialize()
        {
            foreach (var item in Enum.GetNames(typeof(EDTLibrary.DteqTypes))) {
                DteqTypes.Add(item.ToString());
            }
            //BuildFedFromList();

            //Instatiates the required properties
            MasterLoadList = new ObservableCollection<ILoadModel>();

            //TODO - move to service
            
            

            //ListManager.AssignLoadsToDteq(DteqList, LoadList);
        }

        #region Constructor
        public EquipmentViewModel()
        {
            
            // Create commands
            AddDteqCommand = new RelayCommand(AddDteq);
            CalculateDteqCommand = new RelayCommand(CalculateDteq);

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
        #endregion

        #region PrivateMembers

        private ObservableCollection<DteqModel> _dteqList = new ObservableCollection<DteqModel>();
        private ObservableCollection<LoadModel> _loadList = new ObservableCollection<LoadModel>();
        #endregion

        


        #region Public Commands


        //Equipment Commands
        public ICommand AddDteqCommand { get; }
        public ICommand CalculateDteqCommand { get; }
        public string Error { get; }
        #endregion



        #region Helper Methods

        private void AddDteq() { // TODO - methods for invalid tags
            if (IsTagAvailable(_dteqTagToAdd) && _dteqTagToAdd != "" && _dteqTagToAdd !=null) {
                DteqList.Add(new DteqModel() { Tag = _dteqTagToAdd });

                CreateMasterLoadList();
                DictionaryStore.CreateDteqDict(DteqList);

                BuildFedFromList();
            }
        }

        private void BuildFedFromList()
        {
            FedFromList = new ObservableCollection<string>(DteqList.Select(dteq => dteq.Tag).ToList());
            FedFromList.Insert(0, "UTILITY");
        }
            private void CalculateDteq()
        {

            ListManager.AssignLoadsToDteq(DteqList, LoadList);
            foreach (var dteq in DteqList) {
                dteq.CalculateLoading();
            }
        }

        private bool IsTagAvailable(string tag) {
            var val = MasterLoadList.FirstOrDefault(t => t.Tag.ToLower() == tag.ToLower());
            if (val!=null) {
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
