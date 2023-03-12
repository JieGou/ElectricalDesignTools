using EdtLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using System;

namespace EDTLibrary.Models.Loads
{
    public interface IPowerConsumer : ICableUser, IComponentUser

    {

        public void ValidateCableSizes();
        int ProtectionDeviceId { get; set; }
        IProtectionDevice ProtectionDevice { get; set; }


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


        double SCCA { get; set; }
        double SCCR { get; set; }

        void CalculateLoading(string propertyName = "");

        



        //Events
        event EventHandler<CalculateLoadingEventArgs> LoadingCalculated;
        abstract void OnLoadingCalculated(string propertyName = "");


        

        
    }
}