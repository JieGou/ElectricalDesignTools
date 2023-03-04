using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;
internal class DummyComponent : IComponentEdt
{

    public bool IsAreaLocked
    {
        get { return _isAreaLocked; }
        set
        {
            _isAreaLocked = value;
            OnPropertyUpdated();

        }
    }
    private bool _isAreaLocked;

    public bool IsCalculationLocked
    {
        get { return _isCalculationLocked; }
        set
        {
            _isCalculationLocked = value;
            OnPropertyUpdated();
        }
    }
    private bool _isCalculationLocked;
    public bool IsValid { get; set; } = true;
    public void Validate()
    {
        var isValid = true;

        IsValid = isValid;
        OnPropertyUpdated();

        return;
    }


    public bool SettingTag { get; set; }



    public double SCCA { get; set; }
    public double SCCR { get; set; }
    public double AIC { get; set; }


    public double FrameAmps { get; set; }
    public double TripAmps { get; set; }
    public string SubCategory { get; set; }
    public string SubType { get; set; }
    public List<string> TypeList { get; set; }
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public IEquipment Owner { get; set; }
    public int SequenceNumber { get; set; }
    public CableModel PowerCable { get; set; }
    public bool IsSelected { get; set; }
    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public int AreaId { get; set; }
    public IArea Area { get; set; }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public double Voltage { get; set; }
    public double HeatLoss { get; set; }
    public string StarterSize { get; set; }

    public event EventHandler PropertyUpdated;
    public event EventHandler AreaChanged;

    public void CalculateSize(IPowerConsumer load)
    {
        throw new NotImplementedException();
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public Task OnAreaChanged()
    {
        throw new NotImplementedException();
    }

    public Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        throw new NotImplementedException();
    }

    public Task UpdateAreaProperties()
    {
        throw new NotImplementedException();
    }
}
