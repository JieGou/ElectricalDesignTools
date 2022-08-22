using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
{

    [AddINotifyPropertyChangedInterface]
    public class MccModel : DistributionEquipment
    {
        public MccModel()
        {

        }
        public MccModel(ListManager listManager)
        {
            Category = Categories.DTEQ.ToString();
            Voltage = LineVoltage;
            ListManager = listManager;
        }

    }

}
