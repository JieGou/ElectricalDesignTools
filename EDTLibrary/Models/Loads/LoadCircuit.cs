﻿using EdtLibrary.Commands;
using EdtLibrary.LibraryData.TypeModels;
using EdtLibrary.Managers;
using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Services;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EDTLibrary.Models.Loads;
[Serializable]
[AddINotifyPropertyChangedInterface]
public class LoadCircuit : ILoadCircuit
{

    

    public LoadCircuit()
    {
        ConvertToLoadCommand = new RelayCommand(ConvertToLoad);
    }

    public bool IsAreaLocked
    {
        get { return _isAreaLocked; }
        set
        {
            _isAreaLocked = value;
            OnPropertyUpdated();

        }
    }
    private bool _isAreaLocked;
    
    public bool IsCalculationLocked
    {
        get { return _isCalculationLocked; }
        set
        {
            _isCalculationLocked = value;
            OnPropertyUpdated();
        }
    }



    private bool _isCalculationLocked;
    public bool IsValid { get; set; } = true;
    public void Validate()
    {
        IsValid = true;
        OnPropertyUpdated();

        return;
    }


    public int ProtectionDeviceId { get; set; }
    public IProtectionDevice ProtectionDevice
    {
        get => _protectionDevice;
        set
        {
            _protectionDevice = value;
            ProtectionDeviceId = _protectionDevice.Id;
        }
    }
    private IProtectionDevice _protectionDevice;

    public ICommand ConvertToLoadCommand { get; }
    public void ConvertToLoad()
    {
        DpnCircuitManager.ConvertToLoad(this);
    }
    public bool IsSelected { get; set; } = false;

    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string Type { get; set; }
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            if (_description == "") {
                PdSizeTrip = 0;
            }
            //else if(_description != "SPARE") {

            //}
            else {

                PdSizeTrip = 15;
            }

            OnPropertyUpdated();
        }
    }
    bool _changingDescription;
    public int AreaId { get; set; }
    public IArea Area { get; set; }
    public int VoltageTypeId { get; set; } = 0;
    public VoltageType VoltageType
    {
        get { return _voltageType; }
        set
        {
            var oldValue = _voltageType;
            if (oldValue == null) oldValue = value;
            _voltageType = value;

            if (DaManager.GettingRecords) return;


            UndoManager.Lock(this, nameof(VoltageType));
            { 
                VoltageTypeId = _voltageType.Id;
                Voltage = _voltageType.Voltage;
            }
            
            UndoManager.AddUndoCommand(this, nameof(VoltageType), oldValue, _voltageType);
               
            if (FedFrom != null) {
                FedFrom.OnAssignedLoadReCalculated(this, new CalculateLoadingEventArgs { PropertyName = nameof(VoltageType) });
            }
            OnPropertyUpdated();
        }
    }

    //tried to get load circuit dPanel Poles display to update but it didn't work
    public static event EventHandler LoadCircuitVoltageChanged;
    private static void OnLoadCircuitVoltageChanged()
    {
        if (LoadCircuitVoltageChanged != null) {
            LoadCircuitVoltageChanged(nameof(FedFromManager), EventArgs.Empty);
        }
    }

    private VoltageType _voltageType;
    private double _pdSizeTrip;
    private string _description = "";

    public double Size { get; set; }
    public string Unit { get; set; }
    public double Fla { get; set; }
    public string FedFromTag { get; set; }
    public int FedFromId { get; set; }
    public string FedFromType { get; set; }
    public IDteq FedFrom { get; set; }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public double HeatLoss { get; set; }

    public double SCCA { get; set; }
    public double SCCR { get; set; }

    public double DemandFactor { get; set; }
    public double Efficiency { get; set; }
    public double PowerFactor { get; set; }
    public double ConnectedKva { get; set; }
    public double DemandKva { get; set; }
    public double DemandKw { get; set; }
    public double DemandKvar { get; set; }
    public double RunningAmps { get; set; }


    public double AmpacityFactor { get; set; }
    public string PdType { get; set; }
    public double PdSizeTrip
    {
        get => _pdSizeTrip;
        set
        {
            var oldValue = _pdSizeTrip;
            _pdSizeTrip = value;
            if (DaManager.GettingRecords == true) return;

            if (PdSizeTrip != 0) {
                if (Description == "") {
                    Description = "SPARE";
                }
                if (FedFrom.VoltageType.Voltage == 240 || FedFrom.VoltageType.Voltage== 208) {
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120);
                }
                else {
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == FedFrom.LoadVoltageType.Voltage);
                }
            }
            if (oldValue == 0) {
                //ConvertToLoad();
            }
            OnPropertyUpdated();
        }
    }

    public double PdSizeFrame { get; 
        set; }

    public string StarterType { get; set; }
    public string StarterSize { get; set; }

    public int SequenceNumber
    {
        get => _sequenceNumber;
        set
        {
            _sequenceNumber = value;
            OnPropertyUpdated();
        }
    }
    private int _sequenceNumber;

    public string PanelSide
    {
        get { return _panelSide; }
        set
        {
            _panelSide = value;
            OnPropertyUpdated();
        }
    }
    public string _panelSide;

    public double Voltage
    {
        get { return _voltage; }
        set
        {
            if (value == null) return;

            var oldValue = _voltage;
            _voltage = value;

            UndoManager.AddUndoCommand(this, nameof(Voltage), oldValue, _voltage);
            OnPropertyUpdated();

        }
    }
    private double _voltage;


    public int CircuitNumber
    {
        get { return _circuitNumber; }
        set { _circuitNumber = value;
            OnPropertyUpdated();
        }
    }
    private int _circuitNumber;

    public CableModel PowerCable { get; set; }
    public ObservableCollection<IComponentEdt> AuxComponents { get; set; }
    public ObservableCollection<IComponentEdt> CctComponents { get; set; }
    public bool StandAloneStarterBool { get; set; }
    public int StandAloneStarterId { get; set; }
    public bool DisconnectBool { get; set; }
    public int DisconnectId { get; set; }
    public bool LcsBool { get; set; }
    public ILocalControlStation Lcs { get; set; }
    public IComponentEdt StandAloneStarter { get; set; }
    public IComponentEdt Disconnect { get; set; }
    public IComponentEdt SelectedComponent { get; set; }


    public void CalculateCableAmps()
    {
        throw new NotImplementedException();
    }
    public void CalculateLoading(string propertyName = "")
    {
        throw new NotImplementedException();
    }
    public void CreatePowerCable()
    {
        throw new NotImplementedException();
    }
    public void SizePowerCable()
    {
        throw new NotImplementedException();
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        throw new NotImplementedException();
    }


    public event EventHandler<CalculateLoadingEventArgs> LoadingCalculated;
    public event EventHandler PropertyUpdated;
    public event EventHandler AreaChanged;
    public event EventHandler SpaceConverted;

    public async Task OnAreaChanged()
    {
        throw new NotImplementedException();
    }
    public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {

        try {
            if (DaManager.GettingRecords == true) return;

            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
            ErrorHelper.Log($"Tag: {Tag}, {callerMethod}");

            if (GlobalConfig.Testing == true) {
                ErrorHelper.Log($"Tag: {Tag}, {callerMethod}");
            }
        }
        catch (Exception ex) {

            EdtNotificationService.SendError(this, property, callerMethod, ex);
        }

    }
    public void OnLoadingCalculated(string propertyName = "")
    {
        if (LoadingCalculated != null) {
            var calcEventArgs = new CalculateLoadingEventArgs() { PropertyName = propertyName };
            LoadingCalculated(this, calcEventArgs);
        }
    }
    public Task UpdateAreaProperties()
    {
        throw new NotImplementedException();
    }
    public async Task OnSpaceConverted()
    {
        //if (SpaceConverted != null) {
        //    SpaceConverted(this, EventArgs.Empty);
        //}

        //await Task.Run(() => {

        //});
    }

    public void ValidateCableSizes()
    {
        throw new NotImplementedException();
    }
}
