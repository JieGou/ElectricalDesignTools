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
        int LoadCount { get; set; }


        void CalculateLoading();
        void CalculateCableAmps();
        void SizeCable();
    }
}