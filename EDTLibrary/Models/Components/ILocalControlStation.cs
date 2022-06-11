using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;

public interface ILocalControlStation: IEquipment
{
   
  
    ICable ControlCable { get; set; }
    IEquipment Owner { get; set; }
    int OwnerId { get; set; }
    string OwnerType { get; set; }
    int SequenceNumber { get; set; }
    string SubCategory { get; set; }
    string SubType { get; set; }
    ObservableCollection<LcsTypeModel> SubTypeList { get; set; }
    string Tag { get; set; }
    string Type { get; set; }
    LcsTypeModel TypeModel { get; set; }

    event EventHandler PropertyUpdated;

    Task OnPropertyUpdated();
    Task UpdateAreaProperties();
    void UpdateArea(object source, EventArgs e);

    public void OnAreaPropertiesChanged(object source, EventArgs e);
    
}