﻿using EDTLibrary.Models.Areas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components
{
    public class EquipmenttComponentModel : IComponent
    {
        public EquipmenttComponentModel()
        {
            Category = Categories.COMP.ToString();
        }
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int AreaId { get; set; }
        public IArea Area { get; set; }
        public string NemaRating { get; set; }
        public string AreaClassification { get; set; }

        public string Type { get; set; }
        public string SubType { get; set; }
        public string Owner { get; set; }
        public ObservableCollection<IComponent> Components { get; set; } = new ObservableCollection<IComponent>();
        

        IEquipment IComponent.Owner { get; set; }
        public int OwnerId { get; set; }
        public string OwnerType { get; set; }
        public int SequenceNumber { get; set; }

        public event EventHandler PropertyUpdated;

        public void OnPropertyUpdated()
        {
            throw new NotImplementedException();
        }

        public void UpdateAreaProperties()
        {
            throw new NotImplementedException();
        }
    }
}
