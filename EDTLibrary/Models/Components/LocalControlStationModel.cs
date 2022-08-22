using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;
public class LocalControlStationModel :  ILocalControlStation
{
    public ListManager ListManager { get; set; }

    public LocalControlStationModel()
    {
        Type = "LCS";
    }
    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }


    public string Type { get; set; }
    public LcsTypeModel TypeModel { get; set; }

    public string Description { get; set; }


    public string SubType { get; set; }

    public ObservableCollection<LcsTypeModel> SubTypeList { get; set; } = new ObservableCollection<LcsTypeModel>();

    public IEquipment Owner { get; set; }

    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public int SequenceNumber { get; set; }

    public int AreaId { get; set; }

    public IArea _area;
    public IArea Area
    {
        get { return _area; }
        set
        {
            var oldValue = _area;
            _area = value;
            AreaManager.UpdateArea(this, _area, oldValue);
        }
    }
    public double Voltage { get; set; }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public ICable ControlCable { get; set; }




    public double HeatLoss { get; set; } //not used







    public event EventHandler PropertyUpdated;

    public virtual async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        await Task.Run(() => {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        });
    }

    public async Task UpdateAreaProperties()
    {
        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
        }));
    }
   
    public event EventHandler AreaChanged;
    public async Task OnAreaChanged()
    {
        await Task.Run(() => {
            if (AreaChanged != null) {
                AreaChanged(this, EventArgs.Empty);
            }
        });
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        IEquipment owner = (IEquipment)source;
        AreaManager.UpdateArea(this, owner.Area, Area);
        OnPropertyUpdated();
    }

   
}
