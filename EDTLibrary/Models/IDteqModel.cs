using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models {
    public interface IDteqModel: IHasLoading //, IHasComponents 
    {


        double LineVoltage { get; set; }
        double LoadVoltage { get; set; }

        List<IHasLoading> AssignedLoads { get; set; }
        int LoadCount { get; set; }


        void CalculateLoading();
    }
}