using System.Collections.Generic;

namespace EDTLibrary.Models {
    interface IDistributionEquipmentModel: IEquipmentModel {

        //Properties

        #region IEquipmentModel
        new int Id { get; set; } //was private beo
        new string Tag { get; set; }
        new string Category { get; set; } //dteq, load, component, cable,
        new string Type { get; set; }
        string Description { get; set; }
        #endregion

        #region ILoadModel - User inputs
        double Voltage { get; set; }
        double Size { get; set; }
        string Unit { get; set; }
        string FedFrom { get; set; }
        #endregion

        double LineVoltage { get; set; }
        double LoadVoltage { get; set; }


        #region ILoadModel - Privately Calculated Values
        double Fla { get; set; }
        double ConnectedKva { get; set; }
        double DemandKw { get; set; }
        double DemandKva { get; set; }
        double DemandKvar { get; set; }
        double PowerFactor { get; set; }
        double RunningAmps { get; set; }

        int CableQty { get; set; }
        string CableSize { get; set; }
        #endregion

        #region Publicly Calculated Values
        List<ILoadModel> AssignedLoads { get; set; }
        int LoadCount { get; set; }
        #endregion

        #region Lists
        List<ComponentModel> InLineComponents { get; set; }
        //List<CableModel> Cables { get; set; }
        #endregion

        #region ILoadModel Interface Un-used
        double LoadFactor { get; set; }
        double Efficiency { get; set; }

        #endregion

        void CalculateLoading();
    }
}