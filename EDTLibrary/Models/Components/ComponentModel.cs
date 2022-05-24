﻿using EDTLibrary.Managers;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components
{
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
                    if (Owner != null ) {
                        CableManager.AssignPowerCables((IPowerConsumer)Owner, ScenarioManager.ListManager);
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
        public string Category { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }

        public int AreaId { get; set; }
        private IArea _area;
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
        public int SequenceNumber { get; set; }


        public event EventHandler PropertyUpdated;

        public void OnPropertyUpdated()
        {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        }

        public void UpdateAreaProperties()
        {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
        }
    }
}
