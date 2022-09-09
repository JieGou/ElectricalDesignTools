using EDTLibrary.LibraryData;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Models.DistributionEquipment
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
                if (value > 60) {
                    _circuitCount = 60;
                }

                else if (value < 0) {
                    _circuitCount = 10;
                }

                else {
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

                for (int i = 1; i <= CircuitCount; i += 2) {
                    cctList.Add(new DpnCircuit { CircuitNumber = i.ToString() });
                }
                return cctList;
            }

        }
        public ObservableCollection<DpnCircuit> CircuitNumbersRight
        {
            get
            {
                var cctList = new ObservableCollection<DpnCircuit>();

                for (int i = 2; i <= CircuitCount; i += 2) {
                    cctList.Add(new DpnCircuit { CircuitNumber = i.ToString() });
                }
                return cctList;
            }

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

        private ObservableCollection<IPowerConsumer> SetLeftCircuits()
        {
            var cctList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;

            for (int i = 0; i < AssignedLoads.Count; i++) {
                if (i % 2 == 0) {
                    cctList.Add(AssignedLoads[i]);
                    if (AssignedLoads[i].Type == "MOTOR") {
                        poleCount += 1;
                    }
                    if (AssignedLoads[i].Type == "PANEL") {
                        poleCount += 2;
                    }
                }
            }

            for (int i = cctList.Count; i < CircuitCount / 2 - poleCount; i++) {
                cctList.Add(new LoadModel { Tag = "-", Description = "SPACE" });
            }

            return cctList;
        }
        private ObservableCollection<IPowerConsumer> SetRightCircuits()
        {
            var cctList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;

            for (int i = 2; i < AssignedLoads.Count; i++) {
                if (i % 2 != 0) {
                    cctList.Add(AssignedLoads[i]);
                    if (AssignedLoads[i].Type == "MOTOR") {
                        poleCount += 1;
                    }
                    if (AssignedLoads[i].Type == "PANEL") {
                        poleCount += 2;
                    }
                }
            }

            for (int i = cctList.Count + 1; i <= CircuitCount / 2 - poleCount; i++) {
                cctList.Add(new LoadModel { Tag = "-", Description = "SPACE" });
            }

            return cctList;
        }
    }

}
