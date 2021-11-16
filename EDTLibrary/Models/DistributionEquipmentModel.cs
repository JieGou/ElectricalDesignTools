using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models {
    public class DistributionEquipmentModel: ILoadModel, IDistributionEquipmentModel {
        public DistributionEquipmentModel() {
            Category = Categories.DIST.ToString();
            Voltage = LineVoltage;
        }

        //Properties

        #region IEquipmentModel

        [System.ComponentModel.Browsable(false)] // make this property non-visisble by grids/databindings
        public int Id { get; set; } //was private beo
        public string Tag { get; set; }
        public string Category { get; set; } //dteq, load, component, cable,
        public string Type { get; set; }
        public string Description { get; set; }
        #endregion

        #region ILoadModel - User inputs
        public int Voltage { get; set; }
        public double Size { get; set; }
        public string Unit { get; set; }
        public string FedFrom { get; set; }
        #endregion

        public int LineVoltage { get; set; }
        public int LoadVoltage { get; set; }


        #region ILoadModel - Privately Calculated Values
        public double Fla { get; set; }
        public double ConnectedKva { get; set; }
        public double DemandKw { get; set; }
        public double DemandKva { get; set; }
        public double DemandKvar { get; set; }
        public double PowerFactor { get; set; }
        public double RunningAmps { get; set; }

        public int CableQty { get; set; }
        public string CableSize { get; set; }
        #endregion

        #region Publicly Calculated Values
        public List<ILoadModel> AssignedLoads { get; set; } = new List<ILoadModel>();
        public int LoadCount { get; set; }
        #endregion

        #region Lists
        public List<ComponentModel> InLineComponents { get; set; } = new List<ComponentModel>();
        //public List<CableModel> Cables { get; set; } = new List<CableModel>();
        #endregion

        #region ILoadModel Interface Un-used
        public double LoadFactor { get; set; }
        public double Efficiency { get; set; }

        #endregion


        //Methods
        public void CalculateLoading() {
            //Calculates the individual loads of each MJEQ load
            foreach (ILoadModel load in AssignedLoads) {
                if (load.Category == Categories.DIST.ToString()) {
                    load.CalculateLoading();
                }
            }

            //Sums all Assinged loads
            Voltage = LineVoltage;

            ConnectedKva = (from x in AssignedLoads select x.ConnectedKva).Sum();
            ConnectedKva = Math.Round(ConnectedKva, GlobalConfig.SigFigs);

            DemandKva = (from x in AssignedLoads select x.DemandKva).Sum();
            DemandKva = Math.Round(DemandKva, GlobalConfig.SigFigs);

            DemandKw = (from x in AssignedLoads select x.DemandKw).Sum();
            DemandKw = Math.Round(DemandKw, GlobalConfig.SigFigs);

            DemandKvar = (from x in AssignedLoads select x.DemandKvar).Sum();
            DemandKvar = Math.Round(DemandKvar, GlobalConfig.SigFigs);

            PowerFactor = DemandKw / DemandKva;
            PowerFactor = Math.Round(PowerFactor, GlobalConfig.SigFigs);

            Fla = ConnectedKva * 1000 / Voltage / Math.Sqrt(3);
            Fla = Math.Round(Fla, GlobalConfig.SigFigs);
        }
    }
}
