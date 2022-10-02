using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Calculations;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;

namespace EDTLibrary.Models.Loads
{
    public interface IPowerConsumer : ICableUser, IComponentUser

    {

        CalculationFlags CalculationFlags { get; set; }

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

        int SequenceNumber { get; set; }
        string PanelSide { get; set; }
        int CircuitNumber { get; set; }

        void CalculateLoading(string propertyName = "");

        



        //Events
        event EventHandler<CalculateLoadingEventArgs> LoadingCalculated;
        abstract void OnLoadingCalculated(string propertyName = "");


        

        
    }
}