using EdtLibrary.LibraryData.Voltage;
using EDTLibrary.Models.Loads;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary.Models.DistributionEquipment
{
    public interface IDteq : IPowerConsumer
    {
        void MoveLoadUp(IPowerConsumer load);
        void MoveLoadDown(IPowerConsumer load);
        bool IsMainLugsOnly { get; set; }
        void Create();
        void Initialize();
        void Delete();
        double CalculateSCCA();

        bool IsCalculating { get; set; }

        double PercentLoaded { get; set; }
        int LineVoltageTypeId { get; set; }
        VoltageType LineVoltageType { get; set; }
        double LineVoltage { get; set; }

        int LoadVoltageTypeId { get; set; }
        VoltageType LoadVoltageType { get; set; }
        double LoadVoltage { get; set; }

        ObservableCollection<IPowerConsumer> AssignedLoads { get; set; }

        double SCCA { get; set; }

        double LoadCableDerating { get; set; }

        bool CanAdd(IPowerConsumer load);
        bool AddNewLoad(IPowerConsumer load);
        void RemoveAssignedLoad(IPowerConsumer load);

        public abstract void OnAssignedLoadReCalculated(object source, CalculateLoadingEventArgs e);
        void SetLoadProtectionDevice(IPowerConsumer load);
    }
}