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
        private ObservableCollection<IPowerConsumer> _leftCircuits;
        private ObservableCollection<IPowerConsumer> _rightCircuits;
        private int _circuitCount = 24;

        public DpnModel()
        {

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

        private int _leftPoleCount;
        public int PoleCountLeft
        {
            get { return _leftPoleCount; }
            set { _leftPoleCount = value; }
        }
        public ObservableCollection<IPowerConsumer> LeftCircuits
        {
            get
            {
                return SetLeftCircuits();
            }
            set
            {
                _leftCircuits = value;
            }
        }
        

        public ObservableCollection<IPowerConsumer> SetLeftCircuits()
        {
            var cctList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;
            DpnSide dpnSide = DpnSide.Left;

            //Todo - PoleCount
            for (int i = 0; i < AssignedLoads.Count; i++) {

                if (i % 2 == 0) {
                    poleCount += 1;
                    cctList.Add(AssignedLoads[i]);
                    if (AssignedLoads[i].Type == "MOTOR") {
                        poleCount += 1;
                    }
                    if (AssignedLoads[i].Type == "PANEL") {
                        poleCount += 2;
                    }
                }
            }

            for (int i = cctList.Count; i <= CircuitCount / 2 - poleCount; i++) {
                cctList.Add(new LoadModel { Tag = "-", Description = "SPACE" });
            }
            PoleCountLeft = poleCount;
            return cctList;
        }




        public ObservableCollection<IPowerConsumer> RightCircuits
        {
            get
            {
                return SetRightCircuits();
            }
            set
            {
                _rightCircuits = value;
            }
        }
        private int _rightPoleCount;

        public int PoleCountRight
        {
            get { return _rightPoleCount; }
            set { _rightPoleCount = value; }
        }

        public ObservableCollection<DpnCircuit> DpnCircuitList { get; private set; }

        public ObservableCollection<IPowerConsumer> SetRightCircuits()
        {
            var cctList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;

            //Todo - PoleCount
            for (int i = 2; i < AssignedLoads.Count; i++)
            {
                if (i % 2 != 0)
                {
                    poleCount += 1;
                    cctList.Add(AssignedLoads[i]);
                    if (AssignedLoads[i].Type == "MOTOR")
                    {
                        poleCount += 1;
                    }
                    if (AssignedLoads[i].Type == "PANEL")
                    {
                        poleCount += 2;
                    }
                }
            }

            for (int i = cctList.Count + 1; i <= CircuitCount / 2 - poleCount+1; i++)
            {
                cctList.Add(new LoadModel { Tag = "-", Description = "SPACE" });
            }

            PoleCountRight = poleCount;
            return cctList;
        }

        public ObservableCollection<IPowerConsumer> SetCircuits()
        {
            
            var cctList = LeftCircuits;

            //Todo - PoleCount
            for (int i = 0; i < AssignedLoads.Count; i++) {

                if (i % 2 == 0) {
                    cctList.Add(AssignedLoads[i]);
                    if (AssignedLoads[i].Type == "MOTOR") {
                    }
                    if (AssignedLoads[i].Type == "PANEL") {
                    }
                }
                cctList = cctList == LeftCircuits ? RightCircuits : LeftCircuits; 
            }

            for (int i = LeftCircuits.Count; i <= CircuitCount / 2; i++) {
                cctList.Add(new LoadModel { Tag = "-", Description = "SPACE" });
            }

            for (int i = LeftCircuits.Count; i <= CircuitCount / 2; i++) {
                cctList.Add(new LoadModel { Tag = "-", Description = "SPACE" });
            }
            return cctList;
        }

        public override bool AddAssignedLoad(IPowerConsumer load)
        {
            if (base.AddAssignedLoad(load))
            {
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
