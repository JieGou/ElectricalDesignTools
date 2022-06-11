using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Managers;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;

[AddINotifyPropertyChangedInterface]

public class ComponentModel : IComponent
{
    public ComponentModel()
    {
        //Category = Categories.Component.ToString();
    }
    public int Id { get; set; }

    private string _tag;
    public string Tag
    {
        get { return _tag; }
        set
        {
            var oldValue = _tag;
            _tag = value;
            if (GlobalConfig.GettingRecords == false) {
                if (Owner != null) {
                    if (CableManager.IsUpdatingPowerCables == false) {
                        CableManager.UpdateLoadPowerComponentCablesAsync((IPowerConsumer)Owner, ScenarioManager.ListManager);
                    }
                }
            }
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(Tag), OldValue = oldValue, NewValue = _tag };
                Undo.UndoList.Add(cmd);
            }
            OnPropertyUpdated();
        }
    }



    public string Description { get; set; }
    public string Category { get; set; } //Component
    public string SubCategory { get; set; } //Aux vs Cct Component

    public string Type { get; set; } //DefaultDCN, DefaultDrive, FDS, UDS, LCS, VSD, RVS 
    public string SubType { get; set; } //Lcs Type
    public ObservableCollection<LcsTypeModel> SubTypeList { get; set; } = new ObservableCollection<LcsTypeModel>();

    public int AreaId { get; set; }
    private IArea _area;
    private int _sequenceNumber;

    public IArea Area
    {
        get { return _area; }
        set
        {
            var oldValue = _area;
            _area = value;
            if (Area != null) {
                AreaManager.UpdateArea(this, _area, oldValue);

                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(Area), OldValue = oldValue, NewValue = _area };
                    Undo.UndoList.Add(cmd);
                }
                OnPropertyUpdated();
            }

        }
    }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public IEquipment Owner { get; set; }
    public int SequenceNumber
    {
        get => _sequenceNumber;
        set
        {
            _sequenceNumber = value;
            OnPropertyUpdated();
        }
    }


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
