﻿using EDTLibrary.DataAccess;
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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Xml;

namespace EDTLibrary.Models.DistributionEquipment.DPanels
{

    [AddINotifyPropertyChangedInterface]
    public class DpnModel : DistributionEquipment, IDpn
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
                if (value > 90) {
                    _circuitCount = 90;
                }

                else if (value < 10) {
                    _circuitCount = 10;
                }
                else {
                    _circuitCount = value % 2 == 0 ? value : value + 1;
                }

                SetLeftCircuits();
                SetRightCircuits();

                OnPropertyUpdated();
            }
        }
        public ObservableCollection<DpnCircuit> CircuitList { get; private set; }

        public ObservableCollection<DpnCircuit> CircuitNumbersLeft
        {
            get
            {
                var cctList = new ObservableCollection<DpnCircuit>();

                for (int i = 1; i <= CircuitCount; i += 2) {
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

                for (int i = 2; i <= CircuitCount; i += 2) {
                    cctList.Add(new DpnCircuit { CircuitNumber = i });
                }
                return cctList;
            }

        }


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
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;
            var spareCircuitsToDelete = new ObservableCollection<LoadCircuit>();

            //Add AssignedLoads
            for (int i = 0; i < AssignedLoads.Count; i++) { //assignedCircuit.CircuitSide == Left

                if (i % 2 == 0) {
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
            }

            //Add AssignedCircuits

            for (int i = 0; i < AssignedCircuits.Count; i++) {

                if (i / 1 == i) { //assignedCircuit.CircuitSide == Left

                    
                    // spaRe - has a breaker assigned and/or is more than 1 pole
                    if (AssignedCircuits[i].VoltageType != null) {
                        if ((poleCount + AssignedCircuits[i].VoltageType.Poles) <= (CircuitCount/2) ) {
                            sideCircuitList.Add(AssignedCircuits[i]);
                            poleCount += AssignedCircuits[i].VoltageType.Poles;
                        }
                    }
                    //spaCe - single pole breaker
                    else {
                        //fill remaining poles
                        if ((poleCount + 1) <= (CircuitCount / 2)) {
                            sideCircuitList.Add(AssignedCircuits[i]);
                            poleCount += 1;
                        }
                        else {
                            spareCircuitsToDelete.Add(AssignedCircuits[i]);
                            
                        }
                    }
                }
            }

            // delete surplus spare circuits
            foreach (var item in spareCircuitsToDelete) {
                DpnCircuitManager.DeleteLoadCircuit(this, item, ScenarioManager.ListManager);
                AssignedCircuits.Remove(item);
            }

            int cctCount = CircuitCount / 2;
            var newCircuit = new LoadCircuit();

            //Create new circuits to fill panel
            for (int i = 1; i <= cctCount - poleCount; i++) {
                newCircuit = new LoadCircuit {
                    Tag = "-",
                    Description = "SPACE",
                    //VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120),
                    FedFromId = Id,
                    FedFromType = typeof(DpnModel).ToString(),
                    SequenceNumber = 999
                };

                if (DaManager.GettingRecords==false) {
                    if (ScenarioManager.ListManager.LoadCircuitList.Count>0) {
                        newCircuit.Id = ScenarioManager.ListManager.LoadCircuitList.Max(l => l.Id) + 1;
                    }
                    else {
                        newCircuit.Id = 1;
                    }
                    ScenarioManager.ListManager.LoadCircuitList.Add(newCircuit);
                    AssignedCircuits.Add(newCircuit);
                    newCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                    newCircuit.OnPropertyUpdated();
                    sideCircuitList.Add(newCircuit);
                    ScenarioManager.ListManager.DpnCircuitList.Add(new DpnCircuit { DpnId = this.Id, LoadId = newCircuit.Id });
                }
                
            }

            PoleCountLeft = poleCount;
            LeftCircuits = sideCircuitList;

            //order circuits
            var list = LeftCircuits.OrderBy(c => c.SequenceNumber).ToList();
            LeftCircuits.Clear();
            foreach (var item in list) {
                LeftCircuits.Add(item);
            }
            return sideCircuitList;
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

            //Todo - copy from left Circuit
            for (int i = 0; i < AssignedLoads.Count; i++) {
                if (i % 2 != 0) {
                    cctList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
            }

            int cctCount = CircuitCount / 2;
            for (int i = 2; i <= cctCount - poleCount + 1; i++) {
                cctList.Add(new LoadModel {
                    Tag = "-",
                    Description = "SPACE",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120),
                    SequenceNumber = 999
                });
            }

            PoleCountRight = poleCount;
            RightCircuits = cctList;
            var list = RightCircuits.OrderBy(c => c.SequenceNumber).ToList();
            RightCircuits.Clear();
            foreach (var item in list) {
                RightCircuits.Add(item);
            }
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
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120)
                });
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
            if (base.AddAssignedLoad(load)) {
                if (ScenarioManager.ListManager.DpnCircuitList.Count == 0) {
                    return false;
                }

                DpnCircuitManager.AddLoad(this, load, ScenarioManager.ListManager);


                var circuitToAdd = new DpnCircuit {

                    Id = ScenarioManager.ListManager.DpnCircuitList.Max(x => x.Id) + 1,

                    DpnId = this.Id,
                    DpnType = this.Type,
                    LoadId = load.Id,
                    LoadType = load.Type,
                    CircuitNumber = ScenarioManager.ListManager.DpnCircuitList.Max(x => x.CircuitNumber),


                };

                CircuitList.Add(circuitToAdd);

                return true;
            }

            return false;

        }

        public ObservableCollection<LoadCircuit> AssignedCircuits { get; set; } = new ObservableCollection<LoadCircuit>();

        private ObservableCollection<LoadCircuit> _assignedCircuits;

        public void AddAssignedCircuit()
        {

        }
    }

}
