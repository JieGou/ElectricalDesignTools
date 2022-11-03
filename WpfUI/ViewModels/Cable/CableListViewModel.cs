using EdtLibrary.Commands;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
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
using WpfUI.Helpers;
using WpfUI.Stores;
using WpfUI.ViewModels.Cable;

namespace WpfUI.ViewModels.Cables;

[AddINotifyPropertyChangedInterface]
public class CableListViewModel : ViewModelBase
{
    public CableListViewModel(ListManager listManager)
    {
        _listManager = listManager;
        RacewayToAddValidator = new RacewayToAddValidator(listManager);
        AddRacewayRouteSegmentCommand = new RelayCommand(AddRacewayRouteSegment);
        RemoveRacewayRouteSegmentCommand = new RelayCommand(RemoveRacewayRouteSegment);

        AddRacewayCommand = new RelayCommand(AddRaceway);
        DeleteRacewayCommand = new RelayCommand(DeleteRaceway);

    }


    public ListManager ListManager
    {
        get { return _listManager; }
        set { _listManager = value; }
    }
    private ListManager _listManager;

    public RacewayToAddValidator RacewayToAddValidator { get; set; }

    private ICable _selectedCable;
    public ICable SelectedCable
    {
        get { return _selectedCable; }
        set { _selectedCable = value; }
    }

    private RacewayRouteSegment _selectedRacewaySegment;
    public RacewayRouteSegment SelectedRacewaySegment
    {
        get { return _selectedRacewaySegment; }
        set 
        { 
            _selectedRacewaySegment = value;
            if (SelectedRacewaySegment != null) {
                SelectedRaceway = _selectedRacewaySegment.RacewayModel;
            }

        }
    }

    private RacewayModel _selectedProjectRaceway;
    public RacewayModel SelectedProjectRaceway
    {
        get { return _selectedProjectRaceway; }
        set 
        { 
            _selectedProjectRaceway = value;
            if (_selectedProjectRaceway != null) {
                SelectedRaceway = _selectedProjectRaceway;
            }

        }
    }

    private RacewayModel _selectedRaceway;
    public RacewayModel SelectedRaceway
    {
        get { return _selectedRaceway; }
        set 
        { 
            _selectedRaceway = value;

            _cablesInSelectedRaceway.Clear();
            foreach (var cable in ListManager.CableList) {
                foreach (var segment in cable.RacewaySegmentList) {
                    if (segment.RacewayId == _selectedRaceway.Id) {
                        _cablesInSelectedRaceway.Add(cable);
                    }
                }
            }
            TraySizerViewModel = new TraySizerViewModel(ListManager, _selectedRaceway, _cablesInSelectedRaceway);
        }
    }

    private List<CableModel> _cablesInSelectedRaceway = new List<CableModel>();
    public TraySizerViewModel TraySizerViewModel { get; set; }




    public ICommand AddRacewayRouteSegmentCommand { get; }
    public void AddRacewayRouteSegment()
    {
        if (_selectedProjectRaceway == null || _selectedCable == null) return;
        RacewayManager.AddRacewayRouteSegment(_selectedProjectRaceway, _selectedCable, ListManager);
    }

    public ICommand RemoveRacewayRouteSegmentCommand { get; }
    public void RemoveRacewayRouteSegment()
    {
        if (_selectedRacewaySegment == null || _selectedCable == null) return;
        RacewayManager.RemoveRacewayRouteSegment(_selectedRacewaySegment, _selectedCable, ListManager);
    }

    public ICommand AddRacewayCommand { get; }
    public void AddRaceway(object racewayToAddObject)
    {
        AddRacewayAsync(racewayToAddObject);
    }

    public async Task AddRacewayAsync(object racewayToAddObject)
    {
        try {
            RacewayModel newRaceway = await RacewayManager.AddRaceway(racewayToAddObject, _listManager);
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public ICommand DeleteRacewayCommand { get; }
    public void DeleteRaceway(object racewayToAddObject)
    {
        DeleteRacewayAsync(racewayToAddObject);
    }

    public async Task DeleteRacewayAsync(object racewayToAddObject)
    {
        try {
            int deletedRacewayId = await RacewayManager.DeleteRaceway(racewayToAddObject, _listManager);
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }
}

