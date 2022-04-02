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
        PdType = ProjectSettings.EdtSettings.DteqDefaultPdTypeLV;
    }

    private double _impedance;
    public double Impedance
    {
        get { return _impedance; }
        set
        {
            var oldImpZ = _impedance;
            _impedance = value;
            if (_impedance <= 0) {
                _impedance = oldImpZ;
            }
            SCCR = CalculateSCCR();
            //_sccr = CalculateSCCR();
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

    public override double SCCR { get; set; }

    public ILoad LargestMotor
    {
        get { return FindLargestMotor(this, new LoadModel { ConnectedKva=0}); }
    }


    private double CalculateSCCR()
    {
        SCCR = 100 / (_impedance * 0.9) * Fla;
        SCCR = Math.Round(SCCR, 2);
        //_sccr = SCCR;
        return SCCR;
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

