using EDTLibrary.LibraryData.TypeModels;
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
public class DisconnectPropModel : PropertyModelBase
{
    public string FuseType
    {
        get { return _fuseType; }
        set
        {
            _fuseType = value;
            OnPropertyUpdated(nameof(FuseType));
        }
    }
    private string _fuseType;


}
