using EdtLibrary.Commands;
using EDTLibrary.DataAccess;
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
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Xml;

namespace EDTLibrary.Models.DistributionEquipment.DPanels
{

    [AddINotifyPropertyChangedInterface]
    public class DpnModel : DistributionEquipment, IDpn
    {


        public DpnModel()
        {
            SetLeftCircuits();
            SetRightCircuits();

            //MoveUpRightCommand = new RelayCommand(MoveUpRight);
            //MoveDownRightCommand = new RelayCommand(MoveDownRight);
        }

        int _minCircuitCount = 12;
        private int _circuitCount = 24;
        public int CircuitCount
        {
            get => _circuitCount;
            set
            {
                var oldValue = _circuitCount;
                _circuitCount = value;

                if (_circuitCount > 90) {
                    _circuitCount = 90;
                }

                else if (_circuitCount < _minCircuitCount) {
                    _circuitCount = _minCircuitCount;
                }
                else {
                    _circuitCount = _circuitCount % 2 == 0 ? _circuitCount : _circuitCount + 1;
                }

                int minCircuitsForLoadsLeft = 0;
                int minCircuitsForLoadsRight = 0;
                foreach (var item in LeftCircuits) {
                    if (item.GetType() == typeof(LoadModel)) {
                        minCircuitsForLoadsLeft += item.VoltageType.Poles;
                    }
                }
                foreach (var item in RightCircuits) {
                    if (item.GetType() == typeof(LoadModel)) {
                        minCircuitsForLoadsRight += item.VoltageType.Poles;
                    }
                }
                if (_circuitCount / 2 < minCircuitsForLoadsLeft || _circuitCount / 2 < minCircuitsForLoadsRight) {
                    int minCircuits = Math.Max(minCircuitsForLoadsLeft, minCircuitsForLoadsRight);
                    MessageBox.Show($"Minimum circuits requried for loads: {minCircuits * 2}");
                    _circuitCount = oldValue;
                }

                SetLeftCircuits();
                SetRightCircuits();

                OnPropertyUpdated();
            }
        }
        public ObservableCollection<DpnCircuit> CircuitList { get; private set; } = new ObservableCollection<DpnCircuit>();

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
        private void SetLeftCircuits()
        {
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;
            var surplusCircuitsToDelete = new ObservableCollection<LoadCircuit>();

            poleCount = AddAssignedLoadsToPanelSide(sideCircuitList, poleCount, DpnSide.Left);

            //Add AssignedCircuits
            for (int i = 0; i < AssignedCircuits.Count; i++) {

                //loads thats have a side assigned
                if (AssignedCircuits[i].PanelSide == DpnSide.Left.ToString()) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);

                }
                //loads that do NOT have a side assigned
                else if (AssignedCircuits[i].PanelSide != DpnSide.Right.ToString() && i % 2 == 0) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);
                    AssignedCircuits[i].PanelSide = DpnSide.Left.ToString();
                }
            }

            DeleteSurplusCircuits(surplusCircuitsToDelete);

            CreateAdditionalCircuitsToFillPanelSide(sideCircuitList, poleCount, DpnSide.Left);

            PoleCountLeft = poleCount;
            LeftCircuits = sideCircuitList;

            //order circuits
            var list = LeftCircuits.OrderBy(c => c.SequenceNumber).ToList();
            LeftCircuits.Clear();
            foreach (var item in list) {
                LeftCircuits.Add(item);
            }


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
        private void SetRightCircuits()
        {
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;
            var surplusCircuitsToDelete = new ObservableCollection<LoadCircuit>();

            poleCount = AddAssignedLoadsToPanelSide(sideCircuitList, poleCount, DpnSide.Right);

            //Add AssignedCircuits
            for (int i = 0; i < AssignedCircuits.Count; i++) {

                //known side
                if (AssignedCircuits[i].PanelSide == DpnSide.Right.ToString()) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);

                }
                //unknown side, set side
                else if (AssignedCircuits[i].PanelSide != DpnSide.Left.ToString() && i % 2 != 0) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);
                    AssignedCircuits[i].PanelSide = DpnSide.Right.ToString();
                }
            }

            // delete surplus spare circuits
            DeleteSurplusCircuits(surplusCircuitsToDelete);

            CreateAdditionalCircuitsToFillPanelSide(sideCircuitList, poleCount, DpnSide.Right);

            PoleCountRight = poleCount;
            RightCircuits = sideCircuitList;

            //order circuits
            var list = RightCircuits.OrderBy(c => c.SequenceNumber).ToList();
            RightCircuits.Clear();
            foreach (var item in list) {
                RightCircuits.Add(item);
            }

        }


#region SetCircuits Methods
        private int AddAssignedLoadsToPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount, DpnSide dpnSide)
        {
            DpnSide otherSide;
            otherSide = dpnSide == DpnSide.Left ? DpnSide.Right : DpnSide.Right;

            for (int i = 0; i < AssignedLoads.Count; i++) { //assignedCircuit.CircuitSide == Left

                //If the circuit is already asssigned to left or right, assign it
                if (AssignedLoads[i].PanelSide == dpnSide.ToString()) {
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
                // if the circuit is not assigned to a side, assign it based on its position in the list
                // for Left Side
                else if (dpnSide == DpnSide.Left && AssignedLoads[i].PanelSide != DpnSide.Right.ToString() && i % 2 == 0) {
                    AssignedLoads[i].PanelSide = DpnSide.Left.ToString();
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
                // for Right Side
                else if (dpnSide == DpnSide.Right && AssignedLoads[i].PanelSide != DpnSide.Left.ToString() && i % 2 != 0) {
                    AssignedLoads[i].PanelSide = DpnSide.Right.ToString();
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
            }

            return poleCount;
        }
        private int AddAssignedCircuitToPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount,  ObservableCollection<LoadCircuit> spareCircuitsToDelete,  LoadCircuit loadCircuit)
        {
            //set default voltage/poles of circuit/breaker
            //SetDefaultVoltageAndPoles(loadCircuit);

            // spaRe - has a breaker assigned and/or is more than 1 pole
            if (loadCircuit.VoltageType != null) {
                //add the circuit if it fits in the panel
                if ((poleCount + loadCircuit.VoltageType.Poles) <= (CircuitCount / 2)) {
                    sideCircuitList.Add(loadCircuit);
                    poleCount += loadCircuit.VoltageType.Poles;
                }
                else {
                    spareCircuitsToDelete.Add(loadCircuit);
                }
            }
            //spaCe - single pole breaker
            else {
                //fill remaining poles
                if ((poleCount + 1) <= (CircuitCount / 2)) {
                    sideCircuitList.Add(loadCircuit);
                    poleCount += 1;
                }
                else {
                    spareCircuitsToDelete.Add(loadCircuit);

                }
            }
            return poleCount;

           
        }
        private void SetDefaultVoltageAndPoles(LoadCircuit loadCircuit)
        {
            if (loadCircuit.VoltageType == null) {
                loadCircuit.VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120);
            }
            else if (LineVoltageType.Voltage >= 300) {
                loadCircuit.VoltageType = VoltageType;
            }
        }

        private void CreateAdditionalCircuitsToFillPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount,DpnSide dpnSide)
        {
            var newCircuit = new LoadCircuit();

            for (int i = 1; i <= CircuitCount / 2 - poleCount; i++) {

                newCircuit = new LoadCircuit {
                    Tag = "-",
                    Description = "",
                    //VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120),
                    FedFromId = Id,
                    FedFromType = typeof(DpnModel).ToString(),
                    FedFrom = this,
                    SequenceNumber = 999,
                    PanelSide = dpnSide.ToString(),
                };

                if (DaManager.GettingRecords == false) {
                    if (ScenarioManager.ListManager.LoadCircuitList.Count > 0) {
                        newCircuit.Id = ScenarioManager.ListManager.LoadCircuitList.Max(l => l.Id) + 1;
                    }
                    else {
                        newCircuit.Id = 1;
                    }
                    ScenarioManager.ListManager.LoadCircuitList.Add(newCircuit);
                    AssignedCircuits.Add(newCircuit);
                    newCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                    newCircuit.OnPropertyUpdated();
                    newCircuit.SpaceConverted += this.OnSpaceConverted;
                    sideCircuitList.Add(newCircuit);
                    ScenarioManager.ListManager.DpnCircuitList.Add(new DpnCircuit { DpnId = this.Id, LoadId = newCircuit.Id });
                }

            }
        }
        private void DeleteSurplusCircuits(ObservableCollection<LoadCircuit> spareCircuitsToDelete)
        {
            foreach (var item in spareCircuitsToDelete) {
                DpnCircuitManager.DeleteLoadCircuit(this, item, ScenarioManager.ListManager);
                AssignedCircuits.Remove(item);
                item.SpaceConverted -= OnSpaceConverted;
            }
        }

        //AddLoad
        public override bool AddAssignedLoad(IPowerConsumer load)
        {
            if (base.AddAssignedLoad(load)) {
                SetCircuits();
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
                SetCircuits();

                return true;
            }
            SetCircuits();
            return false;

        }

        public void SetCircuits()
        {
            SetLeftCircuits();
            SetRightCircuits();
            CalculatePhaseLoading();
        }
#endregion
        public ObservableCollection<LoadCircuit> AssignedCircuits { get; set; } = new ObservableCollection<LoadCircuit>();
        private ObservableCollection<LoadCircuit> _assignedCircuits;

        public double PhaseA
        {
            get { return _phaseA; }
            set 
            { 
                _phaseA = value; 
            }
        }
        private double _phaseA;

        public double PhaseB
        {
            get { return _phaseB; }
            set
            {
                _phaseB = value;
            }
        }
        private double _phaseB;

        public double PhaseC
        {
            get { return _phaseC; }
            set
            {
                _phaseC = value;
            }
        }
        private double _phaseC;

        public override void CalculateLoading()
        {
            if (DaManager.Importing) return;
            base.CalculateLoading();
            CalculatePhaseLoading();
        }

        public void CalculatePhaseLoading()
        {
            PhaseA = 0;
            PhaseB = 0;
            PhaseC = 0;

            if (LineVoltageType.Phase==3) {
                Calculate3PhaseLoading();
            }
            PhaseA = Math.Round(PhaseA, 2);
            PhaseB = Math.Round(PhaseB, 2);
            PhaseC = Math.Round(PhaseC, 2);


        }
        public void Calculate3PhaseLoading()
        {
            CalculateLeftCircuits3PhaseLoading();
            CalculateRightCircuits3PhaseLoading();
        }
        private void CalculateLeftCircuits3PhaseLoading()
        {
            int cctNo = 1;

            foreach (var load in LeftCircuits) {
                if (load.VoltageType == null) {
                    cctNo += 2;
                    continue;
                }
                // Phase A
                if (cctNo == 1 || (cctNo - 1) % 6 == 0) {
                    switch (load.VoltageType.Poles) {
                        case 1:
                            PhaseA += load.DemandKva;
                            break;
                        case 2:
                            PhaseA += load.DemandKva;
                            PhaseB += load.DemandKva;
                            break;
                        case 3:
                            PhaseA += load.DemandKva / 3;
                            PhaseB += load.DemandKva / 3;
                            PhaseC += load.DemandKva / 3;
                            break;
                    }
                }
                //Phase B
                else if (cctNo % 3 == 0) {
                    switch (load.VoltageType.Poles) {
                        case 1:
                            PhaseB += load.DemandKva;
                            break;
                        case 2:
                            PhaseB += load.DemandKva;
                            PhaseC += load.DemandKva;
                            break;
                        case 3:
                            PhaseA += load.DemandKva / 3;
                            PhaseB += load.DemandKva / 3;
                            PhaseC += load.DemandKva / 3;
                            break;
                    }
                }

                //Phase C
                else if ((cctNo + 1) % 6 == 0) {
                    switch (load.VoltageType.Poles) {
                        case 1:
                            PhaseC += load.DemandKva;
                            break;
                        case 2:
                            PhaseC += load.DemandKva;
                            PhaseB += load.DemandKva;
                            break;
                        case 3:
                            PhaseA += load.DemandKva / 3;
                            PhaseB += load.DemandKva / 3;
                            PhaseC += load.DemandKva / 3;
                            break;
                    }
                }
                cctNo += (load.VoltageType.Poles * 2);
            }

        }
        private void CalculateRightCircuits3PhaseLoading()
        {
            int cctNo = 2;

            foreach (var load in RightCircuits) {
                if (load.VoltageType == null) {
                    cctNo += 2;
                    continue;
                }
                
                //Phase C
                else if (cctNo % 6 == 0) {
                    switch (load.VoltageType.Poles) {
                        case 1:
                            PhaseC += load.DemandKva;
                            break;
                        case 2:
                            PhaseC += load.DemandKva;
                            PhaseA += load.DemandKva;
                            break;
                        case 3:
                            PhaseA += load.DemandKva / 3;
                            PhaseB += load.DemandKva / 3;
                            PhaseC += load.DemandKva / 3;
                            break;
                    }
                }

                //Phase B
                else if (cctNo % 4 == 0) {
                    switch (load.VoltageType.Poles) {
                        case 1:
                            PhaseB += load.DemandKva;
                            break;
                        case 2:
                            PhaseB += load.DemandKva;
                            PhaseC += load.DemandKva;
                            break;
                        case 3:
                            PhaseA += load.DemandKva / 3;
                            PhaseB += load.DemandKva / 3;
                            PhaseC += load.DemandKva / 3;
                            break;
                    }
                }

                // Phase A
                else if (cctNo == 2 || cctNo % 2 == 0) {
                    switch (load.VoltageType.Poles) {
                        case 1:
                            PhaseA += load.DemandKva;
                            break;
                        case 2:
                            PhaseA += load.DemandKva;
                            PhaseB += load.DemandKva;
                            break;
                        case 3:
                            PhaseA += load.DemandKva / 3;
                            PhaseB += load.DemandKva / 3;
                            PhaseC += load.DemandKva / 3;
                            break;
                    }
                }
                cctNo += (load.VoltageType.Poles * 2);
            }
        }


        public void OnSpaceConverted(object source, EventArgs e)
        {
            // redraw panel if a circuit has changed
            SetCircuits();
        }
    }

}
