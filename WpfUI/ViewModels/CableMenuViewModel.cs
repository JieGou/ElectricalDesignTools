using EDTLibrary;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.ViewModels.Cable;
using WpfUI.ViewModels.Cables;

namespace WpfUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class CableMenuViewModel : ViewModelBase, INotifyDataErrorInfo
{

    #region Constructor
    private readonly MainViewModel _mainViewModel;
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


    CableListViewModel _cableListViewModel;
    TraySizerViewModel _traySizerViewModel;

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
        _mainViewModel.CurrentViewModel = CurrentViewModel;
       
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

