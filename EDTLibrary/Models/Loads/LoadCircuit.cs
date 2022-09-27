﻿using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
[AddINotifyPropertyChangedInterface]
public class LoadCircuit : ILoad
{
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
            OnPropertyUpdated();
        }
    }
    public int AreaId { get; set; }
    public IArea Area { get; set; }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public double HeatLoss { get; set; }


    public double LoadFactor { get; set; }
    public double Efficiency { get; set; }
    public string StarterType { get; set; }
    public double StarterSize { get; set; }

    public double PowerFactor { get; set; }
    public double ConnectedKva { get; set; }
    public double DemandKva { get; set; }
    public double DemandKw { get; set; }
    public double DemandKvar { get; set; }
    public double RunningAmps { get; set; }
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


    public int VoltageTypeId { get; set; } = 0;
    public VoltageType VoltageType
    {
        get { return _voltageType; }
        set
        {
            var oldValue = _voltageType;
            _voltageType = value;

            UndoManager.Lock(this, nameof(VoltageType));
            VoltageTypeId = _voltageType.Id;
            Voltage = _voltageType.Voltage;
            UndoManager.AddUndoCommand(this, nameof(VoltageType), oldValue, _voltageType);

            OnPropertyUpdated();
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
    public double AmpacityFactor { get; set; }
    public string PdType { get; set; }
    public double PdSizeTrip
    {
        get => _pdSizeTrip;
        set
        {

            
            if (DaManager.GettingRecords==true) return;
            _pdSizeTrip = value;
            if (PdSizeTrip != 0) {
                Description = "SPARE";
            }
            OnPropertyUpdated();
        }
    }
    public double PdSizeFrame { get; set; }
    public CableModel PowerCable { get; set; }
    public ObservableCollection<IComponentEdt> AuxComponents { get; set; }
    public ObservableCollection<IComponentEdt> CctComponents { get; set; }
    public bool DriveBool { get; set; }
    public int DriveId { get; set; }
    public bool DisconnectBool { get; set; }
    public int DisconnectId { get; set; }
    public bool LcsBool { get; set; }
    public ILocalControlStation Lcs { get; set; }
    public IComponentEdt Drive { get; set; }
    public IComponentEdt Disconnect { get; set; }

    public event EventHandler LoadingCalculated;
    public event EventHandler PropertyUpdated;
    public event EventHandler AreaChanged;

    public void CalculateCableAmps()
    {
        throw new NotImplementedException();
    }

    public void CalculateLoading()
    {
        throw new NotImplementedException();
    }

    public void CreatePowerCable()
    {
        throw new NotImplementedException();
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public Task OnAreaChanged()
    {
        throw new NotImplementedException();
    }

    public void OnLoadingCalculated()
    {
        throw new NotImplementedException();
    }
    public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {

        if (DaManager.GettingRecords == true) return;

        await Task.Run(() => {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        });
        ErrorHelper.Log($"Tag: {Tag}, {callerMethod}");

        if (GlobalConfig.Testing == true) {
            ErrorHelper.Log($"Tag: {Tag}, {callerMethod}");
        }

    }
    public void SizePowerCable()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAreaProperties()
    {
        throw new NotImplementedException();
    }
}