using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Linq;

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

    

    private string _subType;
    public double PrimaryFla
    {
        get { return Math.Round(Fla * LoadVoltage / LineVoltage,0); }
    }
    private double _primaryFla;


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
            SCCA = CalculateSCCA();
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(Impedance), OldValue = oldValue, NewValue = _impedance };
                UndoManager.AddUndoCommand(cmd);
            }
            if (DaManager.GettingRecords) return;
            OnPropertyUpdated();
        }
    }
    private double _impedance;


    //Primary
    public string PrimaryWiring
    {
        get { return _primaryWiring; }
        set
        {
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

    public string PrimaryGrounding
    {
        get { return _primaryGrounding; }
        set
        {
            var oldValue = _primaryGrounding;
            _primaryGrounding = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(PrimaryGrounding), OldValue = oldValue, NewValue = _primaryGrounding };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }
    private string _primaryGrounding;

  
   //Secondary
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

    public string SecondaryGrounding
    {
        get { return _secondaryGrounding; }
        set
        {
            var oldValue = _secondaryGrounding;
            _secondaryGrounding = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(SecondaryGrounding), OldValue = oldValue, NewValue = _secondaryGrounding };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }
    private string _secondaryGrounding;




    private double _reactance;
    public double Reactance
    {
        get { return _reactance; }
        set 
        { 
            var oldValue = _reactance;
            _reactance = value; 
            if (_reactance <= 0) {
                _reactance = oldValue;
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
                return FindLargestMotor(this, new DummyLoad { Tag = GlobalConfig.LargestMotor_StartLoad, ConnectedKva = 0 }); 
            }
        }
    }

   
    public override double CalculateSCCA()
    {
        double scca = 0;
        scca = Size / (1.732 * LoadVoltage * Impedance / 100);
        scca = Math.Round(scca, 2);

        if (ProtectionDevice != null) {
            ProtectionDevice.SCCA = scca;
        }

        foreach (var load in AssignedLoads) {

            load.SCCA = scca;
            if (load.ProtectionDevice != null) {
                load.ProtectionDevice.SCCA = scca;
            }

            foreach (var comp in load.CctComponents) {
                if (comp!= null) {
                    comp.SCCA = scca;
                }
            }
        }

        return scca;
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


    public override bool CanAdd(IPowerConsumer load)
    {
        if (AssignedLoads.Count > 0 || load is LoadModel) {
            return false;
        }
        return true;
    }
    public override bool AddNewLoad(IPowerConsumer load)
    {

        if (load == null) return false;

        //check if load is already assigned
        var iLoad = AssignedLoads.FirstOrDefault(l => l == load);

        if (iLoad == null) {
            AssignedLoads.Add(load);
            if (load.ProtectionDevice != null) {
                load.ProtectionDevice.IsSelected = false;
                load.CctComponents.Remove(load.ProtectionDevice);
            }
            return true;
        }
        return false;
    }

}

