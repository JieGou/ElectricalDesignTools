using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.LibraryData.Voltage;
internal class VoltageTypeValidator :TypeValidatorBase
{
	private double _voltage;

	public double Voltage
	{
		get { return _voltage; }
		set 
		{ 
			_voltage = value; 

		}
	}
	private double _phase;

	public double Phase
	{
		get { return _phase; }
		set 
		{
			_phase = value; 
		}
	}
	private string _voltageString;

	public string VoltageString
	{
		get { return _voltageString; }
		set 
		{ 
			_voltageString = value; 
		}
	}
	private int _poles;

	public int Poles
	{
		get { return _poles; }
		set
		{ 
			_poles = value; 
		}
	}


    private bool _isValid;

    public virtual bool Validate()
	{

        string temp;
        string fake = "fake";


       

        if (_isValid && HasErrors == false) {
            return true;
        }
        return false;
    }
}
