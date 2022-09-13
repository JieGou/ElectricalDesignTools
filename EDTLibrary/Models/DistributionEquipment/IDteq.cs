using EDTLibrary.Models.Loads;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.DistributionEquipment
{
    public interface IDteq : IPowerConsumer
    {


        double LineVoltage { get; set; }
        double LoadVoltage { get; set; }

        ObservableCollection<IPowerConsumer> AssignedLoads { get; set; }

        double SCCR { get; set; }

        double LoadCableDerating { get; set; }

        bool AddAssignedLoad(IPowerConsumer load);

    }
}