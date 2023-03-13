using EDTLibrary.LibraryData;
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
            if (value <= 0)
            {
                AddError(nameof(Voltage), "Invalid value.");
                _isValid = false;
            }
            BuildVoltageString();
            ValidateValues();
        }
    }
    
    
    private double _voltage;

    public double Phase
    {
        get { return _phase; }
        set
        {
            _phase = value;
            _isValid = true;

            ClearErrors(nameof(Phase));
            if (value == null || value == 0) {
                AddError(nameof(Phase), "Invalid value.");
                _isValid = false;
            }
            BuildVoltageString();
            ValidateValues();
        }
    }
    private double _phase;



    public double Frequency
    {
        get { return _frequency; }
        set
        { 
            _frequency = value;

            _isValid = true;

            ClearErrors(nameof(Frequency));
            if (value == null || value == 0) {
                AddError(nameof(Frequency), "Invalid value.");
                _isValid = false;
            }
            BuildVoltageString();
            ValidateValues();
        }
    }
    private double _frequency;


    public int Poles
    {
        get { return _poles; }
        set
        {
            _poles = value;

            _isValid = true;

            ClearErrors(nameof(Poles));
            if (value == null || value == 0) {
                AddError(nameof(Poles), "Invalid value.");
                _isValid = false;
            }
            BuildVoltageString();
            ValidateValues();

        }
    }
    private int _poles;


    private bool _isValid;

    public override bool IsValid()
    {
        ValidateValues();

        if (_isValid && HasErrors == false) {
            return true;
        }
        return false;
    }

    private void BuildVoltageString()
    {
        VoltageString = $"{Voltage}-{Phase}-{Frequency}";
    }

    private void ValidateValues()
    {
        //var _voltageTypeValidationString = $"{Voltage}-{Phase}-{Frequency}-{Poles}";
        //var existingVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Voltage &&
        //                                                                        vt.Phase == Phase &&
        //                                                                        vt.Frequency == Frequency &&
        //                                                                        vt.Poles == Poles);
        //ClearErrors(nameof(Voltage));

        //if (existingVoltageType != null) {
        //    _isValid = false;
        //    AddError(nameof(Voltage), "This voltage type is already in the library.");
        //}
    }
}
