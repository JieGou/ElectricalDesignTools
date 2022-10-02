﻿using AutoCAD;
using EdtLibrary.Commands;
using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Equipment;
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

        int _minCircuitCount = 12;
        private int _circuitCount = 24;
        public DpnModel()
        {
        }
        public override void Create()
        {
            base.Create();
            FillPanel();
        }
        private void FillPanel()
        {
            var voltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120);
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            if (LineVoltageType.Voltage != 240 && VoltageType.Voltage != 208 && VoltageType.Voltage != 120) {
                voltageType = LineVoltageType;
            }

            var newLoadCircuit = new LoadCircuit();
            int cctNo = 0;
            for (int i = 0; i < CircuitCount; i++) {
                newLoadCircuit = new LoadCircuit {
                    Tag = DpnCircuitConfig.UnassignedCircuitTag,

                    Description = "",
                    VoltageType = voltageType,
                    VoltageTypeId = voltageType.Id,
                    FedFromId = Id,
                    FedFromType = typeof(DpnModel).ToString(),
                    FedFrom = this,
                };
                if (i == 0) {
                    cctNo = 1;
                }
                else {
                    cctNo ++;
                }
                int id = 0;

                if (ScenarioManager.ListManager.LoadCircuitList.Count ==0) {
                    id = 1;
                }
                else {
                    id = ScenarioManager.ListManager.LoadCircuitList.Max(lc =>lc.Id) + 1;
                }

                newLoadCircuit.Id = id;
                newLoadCircuit.CircuitNumber = cctNo;
                newLoadCircuit.PanelSide = cctNo % 2 != 0 ? DPanels.PanelSide.Left.ToString() : DPanels.PanelSide.Right.ToString();
                newLoadCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                newLoadCircuit.OnPropertyUpdated();

                ScenarioManager.ListManager.LoadCircuitList.Add(newLoadCircuit);
                AssignedCircuits.Add(newLoadCircuit);
                sideCircuitList = newLoadCircuit.PanelSide == DPanels.PanelSide.Left.ToString() ? LeftCircuits : RightCircuits;
                sideCircuitList.Add(newLoadCircuit);

            }
        }
        public virtual void Initialize() 
        {
            var sideCirctuitList = new ObservableCollection<IPowerConsumer>();
            foreach (var item in AssignedLoads) {
                sideCirctuitList = item.PanelSide == DPanels.PanelSide.Left.ToString() ? LeftCircuits : RightCircuits;
                sideCirctuitList.Add(item);
            }
            foreach (var item in AssignedCircuits) {
                sideCirctuitList = item.PanelSide == DPanels.PanelSide.Left.ToString() ? LeftCircuits : RightCircuits;
                sideCirctuitList.Add(item);
            }
            OrderCircuitsByCircuitNumber(LeftCircuits);
            OrderCircuitsByCircuitNumber(RightCircuits);
        }

        public override void Delete()
        {
            foreach (var loadCircuit in AssignedCircuits) {
                loadCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                DaManager.prjDb.DeleteRecord(GlobalConfig.LoadCircuitTable, loadCircuit.Id);
            }
        }

        public void SetCircuits()
        {
            //SetLeftCircuits();
            //SetRightCircuits();
            //CalculatePhaseLoading();
        }
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
        private ObservableCollection<IPowerConsumer> _leftCircuits = new ObservableCollection<IPowerConsumer>();
        private void SetLeftCircuits()
        {
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;
            var surplusCircuitsToDelete = new ObservableCollection<LoadCircuit>();

            var list = AssignedLoads.Where(al => al.CircuitNumber % 2 != 0).ToList();
            list.AddRange(AssignedCircuits.Where(ac => ac.CircuitNumber % 2 != 0).ToList());

            sideCircuitList.Clear();
            foreach (var item in list) {
                sideCircuitList.Add(item);
                poleCount += item.VoltageType.Poles;
            }

            CreateAdditionalCircuitsToFillPanelSide(sideCircuitList, poleCount, DPanels.PanelSide.Left);

            //assign circuit numbers



            //DpnCircuitManager.AssignCircuitNumbers(sideCircuitList);

            PoleCountLeft = poleCount;
            LeftCircuits = sideCircuitList;


            OrderCircuitsByCircuitNumber(LeftCircuits);
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
        private ObservableCollection<IPowerConsumer> _rightCircuits = new ObservableCollection<IPowerConsumer>();
        private void SetRightCircuits()
        {
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            int poleCount = 0;
            var surplusCircuitsToDelete = new ObservableCollection<LoadCircuit>();

            poleCount = AddAssignedLoadsToPanelSide(sideCircuitList, poleCount, DPanels.PanelSide.Right);

            //Add AssignedCircuits
            for (int i = 0; i < AssignedCircuits.Count; i++) {

                //known side
                if (AssignedCircuits[i].PanelSide == DPanels.PanelSide.Right.ToString()) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);

                }
                //unknown side, set side
                else if (AssignedCircuits[i].PanelSide != DPanels.PanelSide.Left.ToString() && i % 2 != 0) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);
                    AssignedCircuits[i].PanelSide = DPanels.PanelSide.Right.ToString();
                }
            }

            // delete surplus spare circuits
            DeleteSurplusCircuits(surplusCircuitsToDelete);

            CreateAdditionalCircuitsToFillPanelSide(sideCircuitList, poleCount, DPanels.PanelSide.Right);

            //assign circuit numbers
            //DpnCircuitManager.AssignCircuitNumbers(sideCircuitList); 
            
            PoleCountRight = poleCount;
            RightCircuits = sideCircuitList;
            
            //order circuits
            var list = RightCircuits.OrderBy(c => c.CircuitNumber).ToList();
            RightCircuits.Clear();
            foreach (var item in list) {
                RightCircuits.Add(item);
            }
        }

        


        #region SetCircuits Methods
        private int AddAssignedLoadsToPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount, PanelSide dpnSide)
        {
            PanelSide otherSide;
            otherSide = dpnSide == DPanels.PanelSide.Left ? DPanels.PanelSide.Right : DPanels.PanelSide.Right;

            for (int i = 0; i < AssignedLoads.Count; i++) { //assignedCircuit.CircuitSide == Left

                //If the circuit is already asssigned to left or right, assign it
                if (AssignedLoads[i].PanelSide == dpnSide.ToString()) {
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
                // if the circuit is not assigned to a side, assign it based on its position in the list
                // for Left Side
                else if (dpnSide == DPanels.PanelSide.Left && AssignedLoads[i].PanelSide != DPanels.PanelSide.Right.ToString() && i % 2 == 0) {
                    AssignedLoads[i].PanelSide = DPanels.PanelSide.Left.ToString();
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
                // for Right Side
                else if (dpnSide == DPanels.PanelSide.Right && AssignedLoads[i].PanelSide != DPanels.PanelSide.Left.ToString() && i % 2 != 0) {
                    AssignedLoads[i].PanelSide = DPanels.PanelSide.Right.ToString();
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

        public void CreateAdditionalCircuitsToFillPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount,PanelSide dpnSide)
        {
            var newCircuit = new LoadCircuit();

            for (int i = 1; i <= CircuitCount / 2 - poleCount; i++) {

                newCircuit = new LoadCircuit {
                    Tag = "Filler-" + DpnCircuitConfig.AddedCircuitDescription,
                    Description = "",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120),
                    VoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120).Id,
                    FedFromId = Id,
                    FedFromType = typeof(DpnModel).ToString(),
                    FedFrom = this,
                    PanelSide = dpnSide.ToString(),
                };

                if (DaManager.GettingRecords == false) {

                    newCircuit.SequenceNumber = DpnCircuitManager.GetAvailableCircuit(this, newCircuit, dpnSide).Item1;
                    newCircuit.CircuitNumber = DpnCircuitManager.GetAvailableCircuit(this, newCircuit, dpnSide).Item2;

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
                    sideCircuitList.Add(newCircuit);
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
        public override bool CanAdd(IPowerConsumer load)
        {
            if (DpnCircuitManager.CanAdd(this, load)) {
                return true;
            }
            return false;
        }
        private static int _leftCctsAvailable = 0;
        private static int _rightCctsAvailable = 0;

        public override bool AdddNewLoad(IPowerConsumer load)
        {

            if (DpnCircuitManager.AddNewLoad(this, load)) {

                OrderCircuitsByCircuitNumber(LeftCircuits);
                OrderCircuitsByCircuitNumber(RightCircuits);
                return true;
            }

            SetCircuits();
            return false;

        }

        public void InsertLoad(IPowerConsumer load)
        {

            if (load == null) return;
            ObservableCollection<IPowerConsumer> sideCircuitList;
            sideCircuitList = load.PanelSide == DPanels.PanelSide.Left.ToString() ? LeftCircuits : RightCircuits;

            var loadCircuitToRemove = sideCircuitList.FirstOrDefault(lc => lc.CircuitNumber == load.CircuitNumber);
            sideCircuitList.Remove(loadCircuitToRemove);
            loadCircuitToRemove.PropertyUpdated -= DaManager.OnLoadCircuitPropertyUpdated;

            DpnCircuitManager.DeleteLoadCircuit((DpnModel)load.FedFrom, loadCircuitToRemove, ScenarioManager.ListManager);

            sideCircuitList.Insert(load.SequenceNumber, load);
            AssignedLoads.Add(load);
            OrderCircuitsByCircuitNumber(sideCircuitList);
          
        }

        public override void RemoveAssignedLoad(IPowerConsumer load)
        {
            //Db deletion done by caller
            
            var sideCircuitList = load.PanelSide == DPanels.PanelSide.Left.ToString() ? LeftCircuits : RightCircuits;
            
            //insert LoadCircuits in place of the
            for (int i = 0; i < load.VoltageType.Poles; i++) {
                var newLoadCircuit = new LoadCircuit {
                    Tag = DpnCircuitConfig.UnassignedCircuitTag,

                    Description = "",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120),
                    VoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120).Id,
                    FedFromId = Id,
                    FedFromType = typeof(DpnModel).ToString(),
                    FedFrom = this,

                    PanelSide = load.PanelSide,
                    SequenceNumber = load.SequenceNumber,
                    CircuitNumber = load.CircuitNumber + 2*i,
                    
                };
                int id = 0;
                if (ScenarioManager.ListManager.LoadCircuitList.Count == 0) {
                    id = 1;
                }
                else {
                    id = ScenarioManager.ListManager.LoadCircuitList.Max(lc => lc.Id) + 1;
                }

                newLoadCircuit.Id = id;
                AssignedCircuits.Add(newLoadCircuit);
                ScenarioManager.ListManager.LoadCircuitList.Add(newLoadCircuit);
                newLoadCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                newLoadCircuit.OnPropertyUpdated();
                sideCircuitList.Add(newLoadCircuit);

            }
            AssignedLoads.Remove(load);
            sideCircuitList.Remove(load);
            load.LoadingCalculated -= OnAssignedLoadReCalculated;
            OrderCircuitsByCircuitNumber(sideCircuitList);
            //SetCircuits();
    

        }

        private void OrderCircuitsByCircuitNumber(ObservableCollection<IPowerConsumer> sideCircuitList)
        {
            //order circuits
            var list = sideCircuitList.OrderBy(c => c.CircuitNumber).ToList();
            sideCircuitList.Clear();
            foreach (var item in list) {
                sideCircuitList.Add(item);
            }
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

        public override void CalculateLoading(string PropetyName = "")
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
            if (LineVoltageType == null) return;
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
                            PhaseA += load.DemandKva/2;
                            PhaseB += load.DemandKva/2;
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
                            PhaseB += load.DemandKva / 2;
                            PhaseC += load.DemandKva / 2;
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
                            PhaseC += load.DemandKva / 2;
                            PhaseB += load.DemandKva / 2;
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
                            PhaseC += load.DemandKva / 2;
                            PhaseA += load.DemandKva / 2;
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
                            PhaseB += load.DemandKva / 2;
                            PhaseC += load.DemandKva / 2;
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

        public override void OnAssignedLoadReCalculated(object source, CalculateLoadingEventArgs e)
        {
           base.OnAssignedLoadReCalculated(source, e);
            if (e.PropertyName == nameof(VoltageType)) {
                SetCircuits();
            }
        }

        public void OnSpaceConverted(object source, EventArgs e)
        {
            // redraw panel if a circuit has changed
            SetCircuits();
        }

        
    }

}
