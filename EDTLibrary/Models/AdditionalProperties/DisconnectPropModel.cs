using EdtLibrary.Models.AdditionalProperties;
using EDTLibrary.LibraryData.TypeModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.Models.TypeProperties;

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
