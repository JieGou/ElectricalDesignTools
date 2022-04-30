using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;

namespace EDTLibrary.Models.DistributionEquipment;

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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(Impedance), OldValue = oldValue, NewValue = _impedance };
                Undo.UndoList.Add(cmd);
            }
            OnPropertyUpdated();
        }
    }

    

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
        get { return FindLargestMotor(this, new LoadModel { Tag="n/a", ConnectedKva=0}); }
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

