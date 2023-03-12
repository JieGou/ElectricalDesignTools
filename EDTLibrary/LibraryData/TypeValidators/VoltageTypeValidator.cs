using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.LibraryData.TypeValidators;
public class VoltageTypeValidator : TypeValidatorBase
{
    public string VoltageString
    {
        get { return _voltageString; }
        set
        {
            _voltageString = value;
        }
    }
    private string _voltageString;

    public double Voltage
    {
        get { return _voltage; }
        set
        {
            _voltage = value;
            _isValid = true;
            ClearErrors(nameof(Voltage));
            if (_voltage <= 0)
            {
                AddError(nameof(Voltage), "Invalid value.");
                _isValid = false;
            }

        }
    }
    private double _voltage;

    public double Phase
    {
        get { return _phase; }
        set
        {
            _phase = value;
        }
    }
    private double _phase;



    public double Frequency
    {
        get { return _frequency; }
        set { _frequency = value; }
    }
    private double _frequency;


    public int Poles
    {
        get { return _poles; }
        set
        {
            _poles = value;
        }
    }
    private int _poles;


    private bool _isValid { get; set; }

    public override bool IsValid()
    {

        string temp;
        string fake = "fake";




        if (_isValid && HasErrors == false)
        {
            return true;
        }
        return false;
    }
}
