﻿using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;

namespace EDTLibrary.Models.DistributionEquipment;

[Serializable]
[AddINotifyPropertyChangedInterface]
public class XfrModel : DistributionEquipment
{
    public XfrModel()
    {
        Category = Categories.DTEQ.ToString();
        Voltage = LineVoltage;
    }

    private double _primaryFla;

    public double PrimaryFla
    {
        get { return Fla * LoadVoltage / LineVoltage; }
    }


    private double _impedance;
    public double Impedance
    {
        get { return _impedance; }
        set
        {
            var oldValue = _impedance;
            _impedance = value;
            if (_impedance <= 0) {
                _impedance = oldValue;
            }
            SCCR = CalculateSCCR();
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Impedance), OldValue = oldValue, NewValue = _impedance };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }


    public string PrimaryGrounding
    {
        get { return _primaryGrounding; }
        set
        {
            var oldValue = _primaryGrounding;
            _primaryGrounding = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(PrimaryGrounding), OldValue = oldValue, NewValue = _primaryGrounding };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }
    private string _primaryGrounding;

    public string SecondaryGrounding
    {
        get { return _secondaryGrounding; }
        set
        {
            var oldValue = _secondaryGrounding;
            _secondaryGrounding = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(SecondaryGrounding), OldValue = oldValue, NewValue = _secondaryGrounding };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }
    private string _secondaryGrounding;


    public string PrimaryWiring
    {
        get { return _primaryWiring; }
        set { 
            _primaryWiring = value; 
            OnPropertyUpdated();
        }
    }
    private string _primaryWiring;

    public TransformerWiringType PrimaryWiringType
    {
        get { return _primaryWiringType; }
        set 
        { 
            _primaryWiringType = value;

            if (!DaManager.GettingRecords) {
                PrimaryWiring = _primaryWiringType.WiringType;
            }
        }
    }
    private TransformerWiringType _primaryWiringType;


   
    public string SecondaryWiring
    {
        get { return _secondaryWiring; }
        set { _secondaryWiring = value; }
    }
    private string _secondaryWiring;

    public TransformerWiringType SecondaryWiringType
    {
        get { return _secondaryWiringType; }
        set 
        { 
            _secondaryWiringType = value;

            if (!DaManager.GettingRecords) {
                SecondaryWiring = _secondaryWiringType.WiringType;
                OnPropertyUpdated(); 
            }
        }
    }
    private TransformerWiringType _secondaryWiringType;



    private double _reactance;
    public double Reactance
    {
        get { return _reactance; }
        set 
        { 
            var oldImpX = _reactance;
            _reactance = value; 
            if (_reactance <= 0) {
                _reactance = oldImpX;
            }
        }
    }

    private double resistance;
    public double Resistance
    {
        get { return resistance; }
        set { resistance = value; }
    }



    public ILoad LargestMotor
    {
        
        get 
        {   
            { 
                return FindLargestMotor(this, new LoadModel { Tag = GlobalConfig.LargestMotor_StartLoad, ConnectedKva = 0 }); 
            }
        }
    }


    public override double CalculateSCCR()
    {
        double sccr = 0;
        //sccr = 1000 / (_impedance * 0.9) * Fla;
        sccr = Size / (1.732 * LoadVoltage * Impedance / 100);
        sccr = Math.Round(sccr, 2);
        return sccr;
    }

    public ILoad FindLargestMotor(IDteq dteq, IPowerConsumer largestMotor)
    {
        foreach (var assignedLoad in dteq.AssignedLoads) {
            if (assignedLoad.Type == LoadTypes.MOTOR.ToString()) {
                if (assignedLoad.ConnectedKva > largestMotor.ConnectedKva) {
                    largestMotor = assignedLoad;
                }
            }
            else if (assignedLoad.Category == Categories.DTEQ.ToString()) { 
                largestMotor = FindLargestMotor((IDteq)assignedLoad, largestMotor);
            }
        }

        return (ILoad)largestMotor;
    }

}

