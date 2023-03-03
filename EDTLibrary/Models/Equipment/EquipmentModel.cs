using EDTLibrary.Models.Areas;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Equipment;
public class EquipmentModel : IEquipment
{

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
        return;
    }


    public bool IsSelected { get; set; } = false;

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

    public event EventHandler PropertyUpdated;
    public event EventHandler AreaChanged;

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