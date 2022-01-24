using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models {
    public interface IDteq: IPowerConsumer //, IHasComponents 
    {


        double LineVoltage { get; set; }
        double LoadVoltage { get; set; }

        List<IPowerConsumer> AssignedLoads { get; set; }
        int LoadCount { get; set; }


        void CalculateLoading();
    }
}