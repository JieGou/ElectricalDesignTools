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
        public int CircuitCount { get; set; } = 42;

        public ObservableCollection<DpnCircuit> CircuitList {
            get
            {
                var cctList = new ObservableCollection<DpnCircuit>();

                for (int i = 1; i <= CircuitCount; i+=2) {
                    cctList.Add( new DpnCircuit { CircuitNumber = i.ToString() } );
                }
                return cctList;
            }
            
        }

        public ObservableCollection<IPowerConsumer> LeftCircuits
        {
            get
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
                    cctList.Add(new LoadModel { Tag = "-", Description="SPACE" });
                }

                return cctList;
            }
        }

        public ObservableCollection<IPowerConsumer> RightCircuits
        {
            get
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

                for (int i = cctList.Count+1; i <= CircuitCount / 2 - poleCount; i++) {
                    cctList.Add(new LoadModel { Tag = i.ToString() });
                }

                return cctList;
            }

        }

    }

}
