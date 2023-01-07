using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Loads;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.DistributionEquipment
{
    public interface IDteq : IPowerConsumer
    {
        public void Validate();
        bool IsMainLugsOnly { get; set; }
        void Create();
        void Initialize();
        void Delete();
        int LineVoltageTypeId { get; set; }
        VoltageType LineVoltageType { get; set; }
        double LineVoltage { get; set; }

        int LoadVoltageTypeId { get; set; }
        VoltageType LoadVoltageType { get; set; }
        double LoadVoltage { get; set; }

        ObservableCollection<IPowerConsumer> AssignedLoads { get; set; }

        double SCCR { get; set; }

        double LoadCableDerating { get; set; }

        bool CanAdd(IPowerConsumer load);
        bool AddNewLoad(IPowerConsumer load);
        void RemoveAssignedLoad(IPowerConsumer load);

        public abstract void OnAssignedLoadReCalculated(object source, CalculateLoadingEventArgs e);
        void SetLoadProtectionDevice(IPowerConsumer load);
    }
}