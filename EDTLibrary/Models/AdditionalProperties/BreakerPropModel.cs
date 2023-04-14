using EdtLibrary.Models.AdditionalProperties;
using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.Models.TypeProperties;
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

    public bool Is100PercentRated
    {
        get { return _is100PercentRated; }
        set
        {
            _is100PercentRated = value;
            if (DaManager.GettingRecords) return;

            if (_is100PercentRated)
            {
                ((ComponentModelBase)Owner).AmpacityFactor = 1;
            }
            else
            {
                ((ComponentModelBase)Owner).AmpacityFactor = 1.25;
            }

            OnPropertyUpdated(nameof(Is100PercentRated));
        }
    }
    private bool _is100PercentRated;




    public string MvRelayType
    {
        get { return _mvRelayType; }
        set { _mvRelayType = value; }
    }
    private string _mvRelayType;

    public int PhaseCtPrimaryRatio
    {
        get { return _phaseCtPrimaryRatio; }
        set { _phaseCtPrimaryRatio = value; 
            OnPropertyUpdated(nameof(BreakerType));
        }
    }
    private int _phaseCtPrimaryRatio = 600;


    public int PhaseCtSecondaryRatio
    {
        get { return _phaseCtSecondaryRatio; }
        set { _phaseCtSecondaryRatio = value;
            OnPropertyUpdated(nameof(BreakerType));
        }
    }
    private int _phaseCtSecondaryRatio=5;


    public int ZeroCtPrimaryRatio
    {
        get { return _zeroCtPrimaryRatio; }
        set { _zeroCtPrimaryRatio = value; 
            OnPropertyUpdated(nameof(BreakerType));
        }
    }
    private int _zeroCtPrimaryRatio=50;


    public int ZeroCtSecondaryRatio
    {
        get { return _zeroCtSecondaryRatio; }
        set { _zeroCtSecondaryRatio = value;
            OnPropertyUpdated(nameof(BreakerType));
        }
    }
    private int _zeroCtSecondaryRatio=5;


}
