using EDTLibrary.LibraryData.LocalControlStations;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;

public interface ILocalControlStation: IEquipment
{
   
  
    ICable Cable { get; set; }
    ICable AnalogCable { get; set; }
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


    //already in IEquipment

    //event EventHandler PropertyUpdated;
    //Task OnPropertyUpdated(string property = "default");
    //Task UpdateAreaProperties();
    
}