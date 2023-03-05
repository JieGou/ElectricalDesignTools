using EDTLibrary.Models.Equipment;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;
[AddINotifyPropertyChangedInterface]
public class BreakerPropModel : PropertyModelBase
{

	public string BreakerType
    {
		get { return _breakerType; }
		set
		{
			_breakerType = value;

			OnPropertyUpdated(nameof(BreakerType));
		}
	}
    private string _breakerType;

}
