using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using FastSerialization;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace EDTLibrary.Models.DistributionEquipment.DPanels
{

    [AddINotifyPropertyChangedInterface]
    public class DpnModel : DistributionEquipment
    {
        
        private int _circuitCount = 24;

        public DpnModel()
        {
            SetLeftCircuits();
            SetRightCircuits();
        }



        public int CircuitCount
        {
            get => _circuitCount;
            set
            {
                if (value > 60)
                {
                    _circuitCount = 60;
                }

                else if (value < 0)
                {
                    _circuitCount = 10;
                }

                else
                {
                    _circuitCount = value;
                }
                LeftCircuits = SetLeftCircuits();
                RightCircuits = SetRightCircuits();

                OnPropertyUpdated();
            }
        }

        public ObservableCollection<DpnCircuit> CircuitNumbersLeft
        {
            get
            {
                var cctList = new ObservableCollection<DpnCircuit>();

                for (int i = 1; i <= CircuitCount; i += 2)
                {
                    cctList.Add(new DpnCircuit { CircuitNumber = i });
                }
                return cctList;
            }

        }
        public ObservableCollection<DpnCircuit> CircuitNumbersRight
        {
            get
            {
                var cctList = new ObservableCollection<DpnCircuit>();

                for (int i = 2; i <= CircuitCount; i += 2)
                {
                    cctList.Add(new DpnCircuit { CircuitNumber = i });
                }
                return cctList;
            }

        }

        public ObservableCollection<DpnCircuit> DpnCircuitList { get; private set; }
        
        public int PoleCountLeft
        {
            get { return _poleCountLeft; }
            set { _poleCountLeft = value; }
        }
        private int _poleCountLeft;

        public ObservableCollection<IPowerConsumer> LeftCircuits
        {
            get
            {
                return _leftCircuits;
            }
            set
            {
                _leftCircuits = value;
            }
        }
        private ObservableCollection<IPowerConsumer> _leftCircuits;
        public ObservableCollection<IPowerConsumer> SetLeftCircuits()
        {
            var cctList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;

            //Todo - PoleCount
            for (int i = 0; i < AssignedLoads.Count; i++) {

                if (i % 2 == 0) {
                    cctList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
            }
            int cctCount = CircuitCount / 2;

            for (int i = 1; i <= cctCount - poleCount; i++) {
                cctList.Add(new LoadModel {
                    Tag = "-",
                    Description = "SPACE",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120)
                });
            }
            PoleCountLeft = poleCount;
            LeftCircuits = cctList;
            return cctList;
        }



        public int PoleCountRight
        {
            get { return _poleCountRight; }
            set { _poleCountRight = value; }
        }
        private int _poleCountRight;
        public ObservableCollection<IPowerConsumer> RightCircuits
        {
            get
            {
                return _rightCircuits;
            }
            set
            {
                _rightCircuits = value;
            }
        }
        private ObservableCollection<IPowerConsumer> _rightCircuits;
        public ObservableCollection<IPowerConsumer> SetRightCircuits()
        {
            var cctList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;

            //Todo - PoleCount
            for (int i = 0; i < AssignedLoads.Count; i++)
            {
                if (i % 2 != 0)
                {
                    cctList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
            }

            int cctCount = CircuitCount / 2;
            for (int i = 2; i <= cctCount - poleCount+1; i++)
            {
                cctList.Add(new LoadModel {
                    Tag = "-",
                    Description = "SPACE",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120)
                });
            }

            PoleCountRight = poleCount;
            RightCircuits = cctList;
            return cctList;
        }


        public ObservableCollection<IPowerConsumer> SetCircuits()
        {
            
            var cctList = LeftCircuits;

            //Todo - PoleCount
            for (int i = 0; i < AssignedLoads.Count; i++) {

                if (i % 2 == 0) {
                    cctList.Add(AssignedLoads[i]);
                    
                }
                //toggle panel side
                cctList = cctList == LeftCircuits ? RightCircuits : LeftCircuits; 
            }

            for (int i = LeftCircuits.Count; i <= CircuitCount / 2; i++) {
                cctList.Add(new LoadModel { 
                    Tag = "-",
                    Description = "SPACE", 
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120)});
            }

            for (int i = LeftCircuits.Count; i <= CircuitCount / 2; i++) {
                cctList.Add(new LoadModel {
                    Tag = "-",
                    Description = "SPACE",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120)
                });
            }
            return cctList;
        }
        public override bool AddAssignedLoad(IPowerConsumer load)
        {
            if (base.AddAssignedLoad(load))
            {
                if (ScenarioManager.ListManager.DpnCircuitList.Count == 0) {
                    return false;
                }
                var circuitToAdd = new DpnCircuit { 
                    
                    Id = ScenarioManager.ListManager.DpnCircuitList.Max(x => x.Id) + 1,
                    
                    DpnId = this.Id, 
                    DpnType = this.Type,
                    LoadId = load.Id,
                    LoadType = load.Type,
                    CircuitNumber = ScenarioManager.ListManager.DpnCircuitList.Max(x => x.CircuitNumber),
                    


                };

                DpnCircuitList.Add(circuitToAdd);

                return true;
            }

            return false;

        }
    }

}
