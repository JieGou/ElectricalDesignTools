using EdtLibrary.Commands;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Raceways;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfUI.Stores;

namespace WpfUI.ViewModels.Cables;

[AddINotifyPropertyChangedInterface]
public class CableListViewModel : ViewModelBase
{
    public CableListViewModel(ListManager listManager)
    {
        _listManager = listManager;
        AddRacewayRouteSegmentCommand = new RelayCommand(AddRacewayRouteSegment);
    }
    

    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }
    private ListManager _listManager;

    private ICable _selectedCable;

    public ICable SelectedCable
    {
        get { return _selectedCable; }
        set { _selectedCable = value; }
    }

    private RacewayRouteSegment _selectedCableRaceway;

    public RacewayRouteSegment SelectedCableRaceway
    {
        get { return _selectedCableRaceway; }
        set { _selectedCableRaceway = value; }
    }

    private RacewayModel _selectedProjectRaceway;

    public RacewayModel SelectedProjectRaceway
    {
        get { return _selectedProjectRaceway; }
        set { _selectedProjectRaceway = value; }
    }

    public ICommand AddRacewayRouteSegmentCommand { get; }
    public void AddRacewayRouteSegment()
    {
        var racewayToadd = ListManager.RacewayList[1];
        RacewayManager.AddRacewayRouteSegment(racewayToadd, _selectedCable, ListManager);
    }
}

