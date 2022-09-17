using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads
{
    public interface IPowerConsumer : ICableUser, IComponentUser

    {
        int VoltageTypeId { get; set; }
        VoltageType VoltageType { get; set; }
        ///Lookups
        double PowerFactor { get; set; }

        // Calculated Values
        double ConnectedKva { get; set; }
        double DemandKva { get; set; }
        double DemandKw { get; set; }
        double DemandKvar { get; set; }
        double RunningAmps { get; set; }


       

        void CalculateLoading();

        



        //Events
        event EventHandler LoadingCalculated;
        abstract void OnLoadingCalculated();

       
        public void OnAssignedLoadReCalculated(object source, EventArgs e)
        {
            CalculateLoading();
        }

        
    }
}