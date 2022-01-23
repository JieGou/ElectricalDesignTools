using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models {
    public interface IDteq: PowerConsumer //, IHasComponents 
    {


        double LineVoltage { get; set; }
        double LoadVoltage { get; set; }

        List<PowerConsumer> AssignedLoads { get; set; }
        int LoadCount { get; set; }


        void CalculateLoading();
    }
}