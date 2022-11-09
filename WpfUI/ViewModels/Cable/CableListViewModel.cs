using EdtLibrary.Commands;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Raceways;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
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

        AddCablesToSelectedRacewayCommand = new RelayCommand(AddCablesToRaceway);

        RemoveCablesFromSelectedRacewayCommand = new RelayCommand(RemoveCablesFromSelectedRaceway);
        RemoveCablesFromAllRacewayCommand = new RelayCommand(RemoveCablesFromAllRaceway);
    }

    private bool _isRandomFill = false;
    public bool IsRandomFill { get => _isRandomFill;
        set 
        { 
            _isRandomFill = value;
            UpdateSelectedRacewayAsync();
        }
    }

    public bool IsBusy { get; set; }


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
        set
        {
            _selectedCable = value;
        }

    }

    public IList SelectedCables { get; internal set; }
    public IList SelectedLoads { get; internal set; }


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
            UpdateSelectedRacewayAsync();
        }
    }

    private async Task UpdateSelectedRacewayAsync()
    {
        TraySizerViewModel = null;
        IsBusy = true;


        Task.Run(() => {

            if (_selectedRaceway == null) return;
            _cablesInSelectedRaceway.Clear();
            foreach (var cable in ListManager.CableList) {
                foreach (var segment in cable.RacewaySegmentList) {
                    if (segment.RacewayId == _selectedRaceway.Id) {
                        _cablesInSelectedRaceway.Add(cable);
                    }
                }
            }
            _selectedRaceway.CableList = new ObservableCollection<ICable>(_cablesInSelectedRaceway);
            TraySizerViewModel = new TraySizerViewModel(ListManager, SelectedRaceway, _cablesInSelectedRaceway, IsRandomFill);

            Application.Current.Dispatcher.Invoke(() => IsBusy = false);
        });
      

    }

    private List<ICable> _cablesInSelectedRaceway = new List<ICable>();

    public TraySizerViewModel TraySizerViewModel { get; set; }




    public ICommand AddRacewayRouteSegmentCommand { get; }
    public void AddRacewayRouteSegment()
    {
        if (_selectedProjectRaceway == null || _selectedCable == null) return;
        RacewayManager.AddRacewayRouteSegment(_selectedProjectRaceway, _selectedCable, ListManager);
        UpdateSelectedRacewayAsync();
    }

    public ICommand RemoveRacewayRouteSegmentCommand { get; }
    public void RemoveRacewayRouteSegment()
    {
        if (_selectedRacewaySegment == null || _selectedCable == null) return;
        RacewayManager.RemoveRacewayRouteSegment(_selectedRacewaySegment, ListManager);
        UpdateSelectedRacewayAsync();
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

    public ICommand AddCablesToSelectedRacewayCommand { get; }
    public void AddCablesToRaceway()
    {
        AddCablesToRacewayAsync();
        UpdateSelectedRacewayAsync();
    }


    public async Task AddCablesToRacewayAsync()
    {
        if (_selectedProjectRaceway == null) return;
        try {
            foreach (var cable in SelectedCables) {
                RacewayManager.AddRacewayRouteSegment(_selectedProjectRaceway, (ICable)cable, ListManager);
            }
        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public ICommand RemoveCablesFromSelectedRacewayCommand { get; }
    public void RemoveCablesFromSelectedRaceway()
    {
        RemoveCablesFromSelectedRacewayAsync();
        UpdateSelectedRacewayAsync();
    }

    public async Task RemoveCablesFromSelectedRacewayAsync()
    {
        if (SelectedRaceway == null) return;
        try {

            ICable cable = new CableModel();

            var segmentsToDelete = new List<RacewayRouteSegment>();
            foreach (var cableObject in SelectedCables) {
                cable = (ICable)cableObject;
                foreach (var segment in ListManager.RacewaySegmentList) {
                    if (cable.Id == segment.CableId && segment.RacewayId == SelectedRaceway.Id) {
                        segmentsToDelete.Add(segment);
                    }
                }
            }

            foreach (var segment in segmentsToDelete) {
                RacewayManager.RemoveRacewayRouteSegment(segment, ListManager);
            }
            UpdateSelectedRacewayAsync();

        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

    public ICommand RemoveCablesFromAllRacewayCommand { get; }
    public void RemoveCablesFromAllRaceway()
    {
        RemoveCablesFromAllRacewayAsync();
        UpdateSelectedRacewayAsync();
    }

    public async Task RemoveCablesFromAllRacewayAsync()
    {

        try {
            ICable cable = new CableModel();

            var segmentsToDelete = new List<RacewayRouteSegment>();
            foreach (var cableObject in SelectedCables) {
                cable = (ICable)cableObject;
                foreach (var segment in ListManager.RacewaySegmentList) {
                    if (cable.Id == segment.CableId) {
                        segmentsToDelete.Add(segment);
                    }
                }
            }

            foreach (var segment in segmentsToDelete) {
                RacewayManager.RemoveRacewayRouteSegment(segment, ListManager);
            }

        }
        catch (Exception ex) {
            ErrorHelper.ShowErrorMessage(ex);
        }
    }

}

