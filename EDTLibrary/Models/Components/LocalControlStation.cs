using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;
public class LocalControlStation : IComponent
{
    public LocalControlStation()
    {
        Type = "LCS";
    }
    public int Id { get; set; }
    public string Tag { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    
    public string SubType { get; set; }
    public IEquipment Owner { get; set; }

    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public int SequenceNumber { get; set; }
    
    public int AreaId { get; set; }
    public IArea Area { get; set; }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public ICable PowerCable { get; set; }

    public event EventHandler PropertyUpdated;

    public async Task OnPropertyUpdated()
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
}
