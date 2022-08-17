using EDTLibrary.Managers;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.ViewModels.Library;

namespace WpfUI.ViewModels.Menus
{
    [AddINotifyPropertyChangedInterface]
    public class LibraryMenuViewModel : ViewModelBase, INotifyDataErrorInfo
    {

        private DataTablesViewModel _dataTablesViewModel;


        private MainViewModel _mainViewModel;
        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set { _mainViewModel = value; }
        }

        private ListManager _listManager;
        public ListManager ListManager
        {
            get { return _listManager; }
            set { _listManager = value; }
        }

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; }
        }

        //CONSTRUCTOR
        public LibraryMenuViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            _dataTablesViewModel = new DataTablesViewModel();
            NavigateDataTablesCommand = new RelayCommand(NavigateDataTables);
        }

        private void NavigateDataTables()
        {
            CurrentViewModel = _dataTablesViewModel;
            _mainViewModel.CurrentViewModel = CurrentViewModel;

        }

        #region Public Commands

        // Equipment Commands
        public ICommand NavigateDataTablesCommand { get; }
        #endregion

    }

}