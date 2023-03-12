using EdtLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components;

public interface ILocalControlStation: IEquipment
{
   
  
    ICable ControlCable { get; set; }
    ICable AnalogCable { get; set; }
    IEquipment Owner { get; set; }
    int OwnerId { get; set; }
    string OwnerType { get; set; }
    int SequenceNumber { get; set; }
    string SubCategory { get; set; }
    string SubType { get; set; }
    int TypeId { get; set; }
    ObservableCollection<LcsTypeModel> TypeList { get; set; }
    string Tag { get; set; }
    string Type { get; set; }
    LcsTypeModel TypeModel { get; set; }

    void UpdateTypelist(bool driveBool);


    //already in IEquipment

    //event EventHandler PropertyUpdated;
    //Task OnPropertyUpdated(string property = "default");
    //Task UpdateAreaProperties();

}