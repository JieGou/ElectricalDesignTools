using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace EDTLibrary.Models.DPanels
{

    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class DpnModel : DistributionEquipment.DistributionEquipment, IDpn
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
                    Tag = "",

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
                newLoadCircuit.PanelSide = cctNo % 2 != 0 ? DPanels.PnlSide.Left.ToString() : DPanels.PnlSide.Right.ToString();
                newLoadCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                newLoadCircuit.OnPropertyUpdated();

                ScenarioManager.ListManager.LoadCircuitList.Add(newLoadCircuit);
                AssignedCircuits.Add(newLoadCircuit);
                sideCircuitList = newLoadCircuit.PanelSide == DPanels.PnlSide.Left.ToString() ? LeftCircuits : RightCircuits;
                sideCircuitList.Add(newLoadCircuit);

            }
        }
        public virtual void Initialize() 
        {
            var sideCirctuitList = new ObservableCollection<IPowerConsumer>();
            foreach (var item in AssignedLoads) {
                sideCirctuitList = item.PanelSide == DPanels.PnlSide.Left.ToString() ? LeftCircuits : RightCircuits;
                sideCirctuitList.Add(item);
            }
            foreach (var item in AssignedCircuits) {
                sideCirctuitList = item.PanelSide == DPanels.PnlSide.Left.ToString() ? LeftCircuits : RightCircuits;
                sideCirctuitList.Add(item);
            }
            OrderCircuitsByCircuitNumber(LeftCircuits);
            OrderCircuitsByCircuitNumber(RightCircuits);
        }

        public override void Delete()
        {
            foreach (var loadCircuit in AssignedCircuits) {
                loadCircuit.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
                DaManager.PrjDb.DeleteRecord(GlobalConfig.LoadCircuitTable, loadCircuit.Id);
            }
        }

        public void SetCircuits()
        {
            SetLeftCircuits();
            SetRightCircuits();
            CalculatePhaseLoading();
        }
        public int CircuitCount
        {
            get => _circuitCount;
            set
            {
                var oldValue = _circuitCount;
                _circuitCount = value;
                if (_circuitCount < oldValue && DaManager.GettingRecords == false) {
                    MessageBox.Show($"Reducing the number of circuits will cause some loads to be assigned to new circuit numbers");
                }

                if (_circuitCount > 90) {
                    _circuitCount = 90;
                }

                else if (_circuitCount < _minCircuitCount) {
                    _circuitCount = _minCircuitCount;
                }
                else {
                    _circuitCount = _circuitCount % 2 == 0 ? _circuitCount : _circuitCount + 1;
                }

                if (GetMinCircuitCount() > _circuitCount) {
                    MessageBox.Show($"Minimum circuits requried for loads: {GetMinCircuitCount()}");
                    _circuitCount = oldValue;
                }

                SetCircuits();
                OnPropertyUpdated();
            }
        }

        private int GetMinCircuitCount()
        {
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
            int minCircuits = Math.Max(minCircuitsForLoadsLeft, minCircuitsForLoadsRight);
            if (_circuitCount / 2 < minCircuitsForLoadsLeft || _circuitCount / 2 < minCircuitsForLoadsRight) {
                //MessageBox.Show($"Minimum circuits requried for loads: {minCircuits * 2}");
            }
            return minCircuits * 2;
        }

        public ObservableCollection<DpnCircuit> CircuitList { get; private set; } = new ObservableCollection<DpnCircuit>();

        public ObservableCollection<DpnCircuit> CircuitNumbersLeft
        {
            get
            {
                return SetLeftCircuitNumbers();
            }
            //set
            //{
            //    _circuitNumbersLeft = value;
            //}

        }
        private ObservableCollection<DpnCircuit> _circuitNumbersLeft;
        private ObservableCollection<DpnCircuit> SetLeftCircuitNumbers()
        {
            var cctList = new ObservableCollection<DpnCircuit>();

            for (int i = 1; i <= _circuitCount; i += 2) {
                cctList.Add(new DpnCircuit { CircuitNumber = i, VoltageType = VoltageType });
            }
            //CircuitNumbersLeft = cctList;
            return cctList;
        }

        public ObservableCollection<DpnCircuit> CircuitNumbersRight
        {
            get
            {
                return SetCircuitNumbersRight();
            }
        }
        private ObservableCollection<DpnCircuit> _circuitNumbersRight;

        private ObservableCollection<DpnCircuit> SetCircuitNumbersRight()
        {
            var cctList = new ObservableCollection<DpnCircuit>();

            for (int i = 2; i <= _circuitCount; i += 2) {
                cctList.Add(new DpnCircuit { CircuitNumber = i });
            }
            //CircuitNumbersRight = cctList;

            return cctList;
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

            poleCount = AddAssignedLoadsToPanelSide(sideCircuitList, poleCount, PnlSide.Left);

            //Add AssignedCircuits
            for (int i = 0; i < AssignedCircuits.Count; i++) {

                //loads thats have a side assigned
                if (AssignedCircuits[i].PanelSide == PnlSide.Left.ToString()) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);

                }
                //loads that do NOT have a side assigned
                else if (AssignedCircuits[i].PanelSide != PnlSide.Right.ToString() && i % 2 == 0) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);
                    AssignedCircuits[i].PanelSide = PnlSide.Left.ToString();
                }
            }

            DeleteSurplusCircuits(surplusCircuitsToDelete);

            CreateAdditionalCircuitsToFillPanelSide(sideCircuitList, poleCount, PnlSide.Left);

            //assign circuit numbers

            PoleCountLeft = poleCount;
            LeftCircuits = sideCircuitList;
            
            //DpnCircuitManager.AssignSequenceNumbers(LeftCircuits);
            //DpnCircuitManager.AssignCircuitNumbers(LeftCircuits);

            OrderCircuitsByCircuitNumber(LeftCircuits);
            SetLeftCircuitNumbers();
        }


        //Right Circuits
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

            poleCount = AddAssignedLoadsToPanelSide(sideCircuitList, poleCount, PnlSide.Right);

            //Add AssignedCircuits
            for (int i = 0; i < AssignedCircuits.Count; i++) {

                //known side
                if (AssignedCircuits[i].PanelSide == PnlSide.Right.ToString()) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);

                }
                //unknown side, set side
                else if (AssignedCircuits[i].PanelSide != PnlSide.Left.ToString() && i % 2 != 0) {
                    poleCount = AddAssignedCircuitToPanelSide(sideCircuitList, poleCount, surplusCircuitsToDelete, AssignedCircuits[i]);
                    AssignedCircuits[i].PanelSide = PnlSide.Right.ToString();
                }
            }

            // delete surplus spare circuits
            DeleteSurplusCircuits(surplusCircuitsToDelete);

            CreateAdditionalCircuitsToFillPanelSide(sideCircuitList, poleCount, PnlSide.Right);

            //assign circuit numbers

            PoleCountRight = poleCount;
            RightCircuits = sideCircuitList;

            //DpnCircuitManager.AssignSequenceNumbers(RightCircuits);
            //DpnCircuitManager.AssignCircuitNumbers(RightCircuits);

            OrderCircuitsByCircuitNumber(RightCircuits);
            SetCircuitNumbersRight();
        }

        


        #region SetCircuits Methods
        private int AddAssignedLoadsToPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount, PnlSide dpnSide)
        {
            PnlSide otherSide;
            otherSide = dpnSide == DPanels.PnlSide.Left ? DPanels.PnlSide.Right : DPanels.PnlSide.Right;

            for (int i = 0; i < AssignedLoads.Count; i++) { //assignedCircuit.CircuitSide == Left

                //If the circuit is already asssigned to left or right, assign it
                if (AssignedLoads[i].PanelSide == dpnSide.ToString()) {
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
                // if the circuit is not assigned to a side, assign it based on its position in the list
                // for Left Side
                else if (dpnSide == DPanels.PnlSide.Left && AssignedLoads[i].PanelSide != DPanels.PnlSide.Right.ToString() && i % 2 == 0) {
                    AssignedLoads[i].PanelSide = DPanels.PnlSide.Left.ToString();
                    sideCircuitList.Add(AssignedLoads[i]);
                    poleCount += AssignedLoads[i].VoltageType.Poles;
                }
                // for Right Side
                else if (dpnSide == DPanels.PnlSide.Right && AssignedLoads[i].PanelSide != DPanels.PnlSide.Left.ToString() && i % 2 != 0) {
                    AssignedLoads[i].PanelSide = DPanels.PnlSide.Right.ToString();
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

        public void CreateAdditionalCircuitsToFillPanelSide(ObservableCollection<IPowerConsumer> sideCircuitList, int poleCount,PnlSide dpnSide)
        {
            var newCircuit = new LoadCircuit();

            for (int i = 1; i <= CircuitCount / 2 - poleCount; i++) {

                newCircuit = new LoadCircuit {
                    Tag = "", //+ DpnCircuitConfig.AddedCircuitDescription,
                    Description = "",
                    VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120),
                    VoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == 120).Id,
                    FedFromId = Id,
                    FedFromType = typeof(DpnModel).ToString(),
                    FedFrom = this,
                    PanelSide = dpnSide.ToString(),
                };

                if (DaManager.GettingRecords == false) {


                    if (sideCircuitList.Count > 0) {
                        newCircuit.SequenceNumber = sideCircuitList.Max(sc => sc.SequenceNumber) + 1;
                        newCircuit.CircuitNumber = sideCircuitList.Max(sc => sc.CircuitNumber);
                        newCircuit.CircuitNumber += sideCircuitList.FirstOrDefault(sc => sc.CircuitNumber == newCircuit.CircuitNumber).VoltageType.Poles * 2;
                    }
                    

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
            return DpnCircuitManager.CanAdd(this, load);
        }
        private static int _leftCctsAvailable = 0;
        private static int _rightCctsAvailable = 0;

        public override bool AddNewLoad(IPowerConsumer load)
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
            sideCircuitList = load.PanelSide == DPanels.PnlSide.Left.ToString() ? LeftCircuits : RightCircuits;

            var loadCircuitToRemove = sideCircuitList.FirstOrDefault(lc => lc.CircuitNumber == load.CircuitNumber && lc.GetType() == typeof(LoadCircuit));
            sideCircuitList.Remove(loadCircuitToRemove);
            AssignedCircuits.Remove((LoadCircuit)loadCircuitToRemove);
            loadCircuitToRemove.PropertyUpdated -= DaManager.OnLoadCircuitPropertyUpdated;

            DpnCircuitManager.DeleteLoadCircuit((DpnModel)load.FedFrom, loadCircuitToRemove, ScenarioManager.ListManager);
            sideCircuitList.Insert(load.SequenceNumber, load);
            OrderCircuitsByCircuitNumber(sideCircuitList);
            AssignedLoads.Add(load);

        }

        public override void RemoveAssignedLoad(IPowerConsumer load)
        {
            //Db deletion done by caller
            
            var sideCircuitList = load.PanelSide == DPanels.PnlSide.Left.ToString() ? LeftCircuits : RightCircuits;
            
            //insert LoadCircuits in place of the
            for (int i = 0; i < load.VoltageType.Poles; i++) {
                var newLoadCircuit = new LoadCircuit {
                    Tag = "",

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

        public void OrderCircuitsByCircuitNumber(ObservableCollection<IPowerConsumer> sideCircuitList)
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


        public override void CalculateLoading(string propertyName = "")
        {
            if (DaManager.GettingRecords) return;
            //if (DaManager.Importing) return;

            base.CalculateLoading(propertyName);
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
            else if (LineVoltageType.Phase == 1) {

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
            if (e.PropertyName == nameof(VoltageType) || e.PropertyName == nameof(LineVoltageType) || e.PropertyName== nameof(LoadVoltageType)) {
                CalculateLoading();
                int newCircuitCount = GetMinCircuitCount();
                if (newCircuitCount > CircuitCount) {
                    MessageBox.Show("Additional circuits need to be added to the panel to allow this breaker size change. Circuits will be bumped down to make room for the larger breaker.");
                    CircuitCount = GetMinCircuitCount();
                    
                }
                SetCircuits();
                DpnCircuitManager.AssignCircuitNumbers(LeftCircuits);
                DpnCircuitManager.AssignCircuitNumbers(RightCircuits);
      
            }
        }

        public void OnSpaceConverted(object source, EventArgs e)
        {
            // redraw panel if a circuit has changed
            SetCircuits();
        }

        //*******************************
        
    }

}
