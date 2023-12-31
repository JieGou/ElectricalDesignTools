﻿using EDTLibrary.Models.Equipment;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.Components
{
    public interface IComponentUser : IEquipment
    {
        ObservableCollection<IComponentEdt> AuxComponents { get; set; }
        ObservableCollection<IComponentEdt> CctComponents { get; set; }

        bool StandAloneStarterBool { get; set; }
        int StandAloneStarterId { get; set; }
        bool DisconnectBool { get; set; }
        int DisconnectId { get; set; }

        bool LcsBool { get; set; }
        public ILocalControlStation Lcs { get; set; }
        public IComponentEdt StandAloneStarter { get; set; }
        public IComponentEdt Disconnect { get; set; }
        public IComponentEdt SelectedComponent { get; set; }

    }
}