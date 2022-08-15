using EDTLibrary.Managers;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.ViewModels.Cable;
using WpfUI.ViewModels.Cables;

namespace WpfUI.ViewModels.Menus;

[AddINotifyPropertyChangedInterface]
public class CableMenuViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private CableListViewModel _cableListViewModel;
    private TraySizerViewModel _traySizerViewModel;

    #region Constructor
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

    public CableMenuViewModel(MainViewModel mainViewModel, ListManager listManager)
    {
        _mainViewModel = mainViewModel;
        _listManager = listManager;

        _cableListViewModel = new CableListViewModel(listManager);
        _traySizerViewModel = new TraySizerViewModel(listManager);


        //Navigation
        NavigateCableListCommand = new RelayCommand(NavigateCableList);
        NavigateTraySizerCommand = new RelayCommand(NavigateTraySizer);

    }

    #endregion


    private void NavigateCableList()
    {
        CurrentViewModel = _cableListViewModel;
        MainViewModel.CurrentViewModel = CurrentViewModel;

    }

    private void NavigateTraySizer()
    {
        CurrentViewModel = _traySizerViewModel;
        _mainViewModel.CurrentViewModel = CurrentViewModel;

    }

    #region Public Commands

    // Equipment Commands
    public ICommand NavigateCableListCommand { get; }
    public ICommand NavigateTraySizerCommand { get; }
    #endregion


}

