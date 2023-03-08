using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Raceways;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.Managers
{
    [AddINotifyPropertyChangedInterface]
    public class ListManager
    {
        public ListManager()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public ObservableCollection<IArea> AreaList { get; set; } = new ObservableCollection<IArea>();


        public ObservableCollection<IEquipment> EqList { get; set; } = new ObservableCollection<IEquipment>();
        public ObservableCollection<IDteq> IDteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<IDteq> DteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<XfrModel> XfrList { get; set; } = new ObservableCollection<XfrModel>();
        public ObservableCollection<SwgModel> SwgList { get; set; } = new ObservableCollection<SwgModel>();
        public ObservableCollection<MccModel> MccList { get; set; } = new ObservableCollection<MccModel>();
        public ObservableCollection<DpnModel> DpnList { get; set; } = new ObservableCollection<DpnModel>();

        public ObservableCollection<SplitterModel> SplitterList { get; set; } = new ObservableCollection<SplitterModel>();

        public ObservableCollection<ILoadCircuit> LoadCircuitList { get; set; } = new ObservableCollection<ILoadCircuit>();

        public ObservableCollection<ILoad> LoadList { get; set; } = new ObservableCollection<ILoad>();
        public ObservableCollection<IComponentEdt> CompList { get; set; } = new ObservableCollection<IComponentEdt>();
        public ObservableCollection<DriveModel> DriveList { get; set; } = new ObservableCollection<DriveModel>();
        public ObservableCollection<ILocalControlStation> LcsList { get; set; } = new ObservableCollection<ILocalControlStation>();

        public ObservableCollection<ProtectionDeviceModel> PdList { get; set; } = new ObservableCollection<ProtectionDeviceModel>();

        public ObservableCollection<CableModel> CableList { get; set; } = new ObservableCollection<CableModel>();
        public ObservableCollection<RacewayModel> RacewayList { get; set; } = new ObservableCollection<RacewayModel>();
        public ObservableCollection<RacewayRouteSegment> RacewaySegmentList { get; set; } = new ObservableCollection<RacewayRouteSegment>();
        public ObservableCollection<BreakerPropModel> BreakerPropModels { get; set; } = new ObservableCollection<BreakerPropModel>();
        public ObservableCollection<DisconnectPropModel> DisconnectPropModels { get; set; } = new ObservableCollection<DisconnectPropModel>();


        public void GetProjectTablesAndAssigments()
        {
            DaManager.GettingRecords = true;

            try {
                //Get
                GetAreas();
                GetDteq();
                GetLoadsAndAssignPropertyUpdatedEvent();
                GetLoadCircuitsAndAddToAssignedCircuits();
                GetVoltages();

                //TODO - Get Components for each type and create a master component list
                GetDrives();
                GetComponents();
                GetProtectionDevices();
                AssignComponents();
                AssignProtectionDevices();

                GetLocalControlStations();
                AssignLocalControlStations();
                GetCables();

                //Assign
                AssignAreas();
                AssignLoadsAndEventsToAllDteq();
                CreateEquipmentList();
                AssignCables();
                AssignListManagerToEquipment(EqList);
                InitializeDpns();
                GetRaceways();
                GetRacewayRouting();

                GetPropertyModels();
                AssignPropertyModels();

                ValidateAll();


                DaManager.GettingRecords = false;

            }
            catch (Exception ex) {

                if (ex.Data.Contains("UserMessage") == false) {
                    ex.Data.Add("UserMessage", "The project database may have been deleted or corrupted since opening. Go to the home screen and reopen the project.");
                }
                throw;
            }
            finally { DaManager.GettingRecords = false;}

        }

        private void GetPropertyModels()
        {
            
            BreakerPropModels.Clear();
            var breakersProps = DaManager.prjDb.GetRecords<BreakerPropModel>(GlobalConfig.BreakerPropsTable) ;

            foreach (var propModel in breakersProps) {
                BreakerPropModels.Add(propModel);
                propModel.PropertyUpdated += DaManager.OnTypeModelPropertyUpdated;                
            }

            DisconnectPropModels.Clear();
            var disconnectProps = DaManager.prjDb.GetRecords<DisconnectPropModel>(GlobalConfig.DisconnectPropsTable);
            foreach (var item in disconnectProps) {
                DisconnectPropModels.Add(item);
                item.PropertyUpdated += DaManager.OnTypeModelPropertyUpdated;
            }
        }

        void AssignPropertyModels()
        {
            var propModelList = new List<PropertyModelBase>();
            propModelList.AddRange(BreakerPropModels);
            propModelList.AddRange(DisconnectPropModels);

            foreach (var propModel in propModelList) {

                //assign to Protection Devices
                foreach (var item in PdList) {
                    if (item.Type == PdTypes.Breaker.ToString() 
                        && item.PropertyModelId == propModel.Id) {
                        item.PropertyModel = propModel;
                    }
                    else if ( item.Type == PdTypes.FDS.ToString()
                        && item.PropertyModelId == propModel.Id) {
                        item.PropertyModel = propModel;
                    }
                }

                //assign to Components
                foreach (var item in CompList) {
                    if (item.PropertyModel != propModel) continue;

                    if (item.Type == PdTypes.Breaker.ToString()) {
                        item.PropertyModel = propModel;
                    }

                    else if (item.Type == CctComponentTypes.UDS.ToString() 
                        || item.Type == CctComponentTypes.FDS.ToString() 
                        || item.Type == CctComponentTypes.Disconnect.ToString()) {

                        item.PropertyModel = propModel;

                    }
                }
            }
        }


        private void ValidateAll()
        {
            foreach (var cable in CableList) {
                cable.Validate(cable);
            }
            CreateEquipmentList();
            foreach (var eq in EqList) {
                eq.Validate();

            }
        }

        private void InitializeDpns()
        {
            foreach (var item in DpnList) {
                item.Initialize();
            }        
        }

        private void GetVoltages()
        {
            foreach (var dteq in IDteqList) {

                //dteq.LineVoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == dteq.LineVoltage).Id;
                dteq.LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Id == dteq.LineVoltageTypeId);

                //dteq.LoadVoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == dteq.LoadVoltage).Id;
                dteq.LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Id == dteq.LoadVoltageTypeId);

            }
            foreach (var load in LoadList) {

                //load.VoltageTypeId = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == load.Voltage).Id;
                load.VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Id == load.VoltageTypeId);
            }
        }

        public ObservableCollection<IEquipment> CreateEquipmentList()
        {
            EqList.Clear();
            foreach (var item in IDteqList) {
                EqList.Add(item);
            }
            foreach (var item in LoadList) {
                EqList.Add(item);
            }
            foreach (var item in CompList) {
                EqList.Add(item);
            }
            foreach (var item in PdList) {
                EqList.Add(item);
            }
            foreach (var item in LcsList) {
                EqList.Add(item);
            }
            return EqList;
        }

        public void AssignListManagerToEquipment(ObservableCollection<IEquipment> equipmentList)
        {
            //foreach (var item in equipmentList) {
            //    item.ListManager = this;
            //}
        }
        private void GetDisconnects()
        {
            //DisconnectList.Clear();
            //var list = DaManager.prjDb.GetRecords<DisconnectModel>(GlobalConfig.DisconnectTable);
            //foreach (var item in list) {
            //    DisconnectList.Add(item);
            //    item.PropertyUpdated += DaManager.OnDisconnectPropertyUpdated;

            //}
        }

        private void GetDrives()
        {
            DriveList.Clear();
            var list = DaManager.prjDb.GetRecords<DriveModel>(GlobalConfig.DriveTable);
            foreach (var item in list) {
                DriveList.Add(item);
                item.PropertyUpdated += DaManager.OnDrivePropertyUpdated;

            }
        }

        public ObservableCollection<IArea> GetAreas()
        {
            AreaList.Clear();
            var list = DaManager.prjDb.GetRecords<AreaModel>(GlobalConfig.AreaTable);
            foreach (var item in list) {
                AreaList.Add(item);
                item.PropertyUpdated += DaManager.OnAreaPropertyUpdated;
            }
            
            //if (AreaList.FirstOrDefault(a => a.Tag == "SITE") == null) {
            //    AreaList.Insert(0, GlobalConfig.DefaultAreaModel);
            //    DaManager.prjDb.InsertRecord(GlobalConfig.DefaultAreaModel, GlobalConfig.AreaTable, NoSaveLists.AreaNoSaveList);
            //}
            return AreaList;
        }

        private void GetDteq()
        {

            IDteqList.Clear();
            DteqList.Clear();
            XfrList.Clear();
            SwgList.Clear();
            MccList.Clear();
            DpnList.Clear();
            SplitterList.Clear();

            //Dteq
            //TODO - Clean up DteqModel vs abstract Dteq
            IDteq fedFrom;

            foreach (var model in DteqList) {
                IDteqList.Add(model);
            }

            //DteqList.Insert(0, new DteqModel() { Tag = GlobalConfig.UtilityTag, Area = new AreaModel() });
            DteqList.Insert(0,  GlobalConfig.UtilityModel);


            //XFR
            XfrList = DaManager.prjDb.GetRecords<XfrModel>(GlobalConfig.XfrTable);
            foreach (var model in XfrList) {
                IDteqList.Add(model);
                DteqList.Add(model);
                model.PrimaryWiringType = TypeManager.TransformerWiringTypes.FirstOrDefault(tw => tw.WiringType == model.PrimaryWiring);
                model.SecondaryWiringType = TypeManager.TransformerWiringTypes.FirstOrDefault(tw => tw.WiringType == model.SecondaryWiring);
            }
            //Swg
            SwgList = DaManager.prjDb.GetRecords<SwgModel>(GlobalConfig.SwgTable);
            foreach (var model in SwgList) {
                IDteqList.Add(model);
                DteqList.Add(model);
            }
            //Mcc
            MccList = DaManager.prjDb.GetRecords<MccModel>(GlobalConfig.MccTable);
            foreach (var model in MccList) {
                IDteqList.Add(model);
                DteqList.Add(model);
            }

            //Dpn
            DpnList = DaManager.prjDb.GetRecords<DpnModel>(GlobalConfig.DpnTable);
            foreach (var model in DpnList) {
                IDteqList.Add(model);
                DteqList.Add(model);
            }


            //Splitters
            SplitterList = DaManager.prjDb.GetRecords<SplitterModel>(GlobalConfig.SplitterTable);
            foreach (var model in SplitterList) {
                IDteqList.Add(model);
                DteqList.Add(model);
            }
            //Assign FedFrom
            foreach (var dteq in IDteqList) {
                //Utility

                if (dteq.FedFromTag == GlobalConfig.UtilityTag) {
                    dteq.FedFrom = DteqList[0];
                }
                //Deleted
                if (dteq.FedFromTag.Contains("Deleted")) { //|| dteq.FedFromType.Contains("Deleted")) {
                    dteq.FedFrom = new DteqModel() { Tag = GlobalConfig.Deleted };
                }

                //Actual and Valid
                fedFrom = IDteqList.FirstOrDefault(d => d.Id == dteq.FedFromId &&
                                                   d.GetType().ToString() == dteq.FedFromType);
                if (fedFrom != null) {
                    dteq.FedFrom = fedFrom;
                }
            }
        }
        private void GetLoadsAndAssignPropertyUpdatedEvent()
        {

            var list = DaManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadTable);
            LoadList.Clear();
            foreach (var item in list) {
                LoadList.Add(item);
            }

            IDteq fedFrom;

            foreach (var load in LoadList) {

                //Set FedFromrom to Deleted if supplier was previously deleted
                if (load.FedFromTag.Contains("Deleted") || load.FedFromType.Contains("Deleted")) {
                    load.FedFrom = GlobalConfig.DteqDeleted;
                }

                //Set FedFrom
                fedFrom = IDteqList.FirstOrDefault(d => d.Id == load.FedFromId &&
                                                   d.GetType().ToString() == load.FedFromType);
                if (fedFrom != null) load.FedFrom = fedFrom;

                //DbNull error prevention
                if (load.Description == null) load.Description = "";

                load.PropertyUpdated += DaManager.OnLoadPropertyUpdated;

            }
        }
        private void GetLoadCircuitsAndAddToAssignedCircuits()
        {
            LoadCircuitList.Clear();
            var list = DaManager.prjDb.GetRecords<LoadCircuit>(GlobalConfig.LoadCircuitTable);
            foreach (var item in list) {
                LoadCircuitList.Add(item);
                item.PropertyUpdated += DaManager.OnLoadCircuitPropertyUpdated;
            }
            IDpn dpn = new DpnModel();
            var sideCircuitList = new ObservableCollection<IPowerConsumer>();
            foreach (var dteq in IDteqList) {
                if (dteq.Type==DteqTypes.CDP.ToString() || dteq.Type == DteqTypes.DPN.ToString()) {
                    foreach (var loadCircuit in LoadCircuitList) {
                        if (dteq.Id == loadCircuit.FedFromId && loadCircuit.FedFromType == typeof(DpnModel).ToString()) {
                            dpn = (IDpn)dteq;
                            dpn.AssignedCircuits.Add((LoadCircuit)loadCircuit);
                            loadCircuit.FedFrom = dpn;
                            loadCircuit.SpaceConverted += dpn.OnSpaceConverted;
                            loadCircuit.VoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Id == loadCircuit.VoltageTypeId);
                        }
                    }
                }
            }
        }

    
        private void AssignDpnCircuits()
        {
            var dPanels = IDteqList.Where(d => d.GetType() == typeof(DpnModel)).ToList();
            foreach (var panel in dPanels) {
                foreach (var load in LoadList) {
                    if (load.FedFromId == panel.Id) {

                    }
                }
            }

        }
        private void GetComponents()
        {
            CompList.Clear();
            var list = DaManager.prjDb.GetRecords<ComponentModel>(GlobalConfig.ComponentTable);
            foreach (var item in list) {
                CompList.Add(item);
            }

        }
        private void AssignComponents()
        {

            // Loads
            foreach (var load in LoadList) {
                foreach (var comp in CompList) {
                    if (comp.OwnerId == load.Id && comp.OwnerType == typeof(LoadModel).ToString()) {
                        comp.Owner = load;
                        comp.PropertyUpdated += DaManager.OnComponentPropertyUpdated;

                        //Cct Components
                        if (comp.Category == Categories.CctComponent.ToString()) {
                            load.CctComponents.Add(comp);
                            load.CctComponents.OrderBy(c => comp.SequenceNumber);

                            if (comp.SubType == CctComponentSubTypes.DefaultStarter.ToString()) {
                                load.StandAloneStarter = (ComponentModel)comp;
                                if (load.FedFrom != null) {
                                    load.FedFrom.AreaChanged += comp.MatchOwnerArea;
                                }
                            }
                            if (comp.SubType == CctComponentSubTypes.DefaultDcn.ToString()) {
                                load.Disconnect = (ComponentModel)comp;
                                load.AreaChanged += comp.MatchOwnerArea;
                            }
                        }
                    }
                }
                load.CctComponents = new ObservableCollection<IComponentEdt>(load.CctComponents.OrderBy(c => c.SequenceNumber).ToList());
            }
        }
        private void GetProtectionDevices()
        {
            PdList.Clear();
            var pdList = DaManager.prjDb.GetRecords<ProtectionDeviceModel>(GlobalConfig.ProtectionDeviceTable);

            foreach (var pd in pdList) {

                PdList.Add(pd);
            }
        }
        private void AssignProtectionDevices()
        {
            // Dteq
            foreach (var dteq in DteqList) {
                foreach (var pd in PdList) {
                    if (pd.OwnerId == dteq.Id && pd.OwnerType == dteq.GetType().ToString()) {
                        dteq.ProtectionDevice = pd;
                        pd.Owner = dteq;
                        pd.PropertyUpdated += DaManager.OnProtectioneDevicePropertyUpdated;

                        if (dteq.FedFrom != null) {
                            dteq.FedFrom.AreaChanged += pd.MatchOwnerArea;
                        }

                        //Cct Components
                        if (pd.IsStandAlone) {
                            dteq.CctComponents.Add(pd);
                            dteq.CctComponents.OrderBy(c => pd.SequenceNumber);
                        }
                    }
                }
                dteq.CctComponents = new ObservableCollection<IComponentEdt>(dteq.CctComponents.OrderBy(c => c.SequenceNumber).ToList());
            }

            // Loads
            foreach (var load in LoadList) {
                foreach (var pd in PdList) {
                    if (pd.OwnerId == load.Id && pd.OwnerType == typeof(LoadModel).ToString()) {
                        load.ProtectionDevice = pd;
                        pd.Owner = load;
                        pd.PropertyUpdated += DaManager.OnProtectioneDevicePropertyUpdated;

                        if (load.FedFrom != null) {
                            load.FedFrom.AreaChanged += pd.MatchOwnerArea;
                        }

                        //Cct Components
                        if (pd.IsStandAlone) {
                            load.CctComponents.Add(pd);
                            load.CctComponents.OrderBy(c => pd.SequenceNumber);
                        }
                    }
                }
                load.CctComponents = new ObservableCollection<IComponentEdt>(load.CctComponents.OrderBy(c => c.SequenceNumber).ToList());
            }
        }

        private void GetLocalControlStations()
        {
            LcsList.Clear();
            var list = DaManager.prjDb.GetRecords<LocalControlStationModel>(GlobalConfig.LcsTable);
            foreach (var item in list) {
                LcsList.Add(item);
            }
        }
        private void AssignLocalControlStations()
        {
            foreach (var load in LoadList) {
                foreach (var lcs in LcsList) {
                    if (lcs.OwnerId == load.Id && lcs.OwnerType == typeof(LoadModel).ToString()) {
                        lcs.Owner = load;
                        load.Lcs = lcs;
                        lcs.PropertyUpdated += DaManager.OnLcsPropertyUpdated;
                        load.AreaChanged += lcs.MatchOwnerArea;

                        lcs.TypeModel = TypeManager.GetLcsTypeModel(lcs.TypeId);

                        
                    }
                }
            }
        }
        private void GetCables()
        {
            CableList.Clear();
            CableList = DaManager.prjDb.GetRecords<CableModel>(GlobalConfig.CableTable);
            AssignCableTypesAndSizes();
        }
        private void AssignCableTypesAndSizes()
        {
            foreach (var cable in CableList) {
                cable.TypeModel = TypeManager.GetCableTypeModel(cable.TypeId);
                //cable.TypeModel = TypeManager.GetCableTypeModel(cable.Type);
                cable.CreateSizeList();

                cable.SetTypeProperties();
            }
        }


        private void GetRaceways()
        {
            RacewayList.Clear();
            var list = DaManager.prjDb.GetRecords<RacewayModel>(GlobalConfig.RacewayTable);
            foreach (var item in list) {
                RacewayList.Add(item);
                item.PropertyUpdated += DaManager.OnRacewayPropertyUpdated;
            }
        }

        private void GetRacewayRouting()
        {
            ObservableCollection<RacewayRouteSegment> segmentList;
            foreach (var cable in CableList) {
                RacewaySegmentList.Clear();
                segmentList = DaManager.prjDb.GetRecords<RacewayRouteSegment>(GlobalConfig.RacewayRouteSegmentsTable);

                foreach (var segment in segmentList) {
                    RacewaySegmentList.Add(segment);
                    segment.RacewayModel = RacewayList.FirstOrDefault(r => r.Id == segment.RacewayId);
                    if (cable.Id == segment.CableId) {
                            cable.RacewaySegmentList.Add(segment);
                    }
                }
                cable.RacewaySegmentList.OrderBy(c => c.SequenceNumber);
            }
        }

        #region MajorEquipment
        //Move to Distribution Manager
        public void CalculateDteqLoadingAsync()
        {
            //await Task.Run(() => {
               
                UnregisterAllDteqFromAllLoadEvents();
                AssignLoadsAndEventsToAllDteq();
             
                //Loads
                DaManager.GettingRecords = false;
                {
                    foreach (var load in LoadList) {

                        load.CalculateLoading();
                        load.PowerCable.AutoSizeAll_IfEnabled();
                        load.PowerCable.CalculateAmpacity(load);

                    }
                }
                DaManager.GettingRecords = false;

             
                //Dteq
                foreach (var dteq in IDteqList) {

                    dteq.CalculateLoading();
                    dteq.PowerCable.AutoSizeAll_IfEnabled();
                    dteq.PowerCable.CalculateAmpacity(dteq);


                }
               
            //});

        }

        public void UnregisterAllDteqFromAllLoadEvents()
        {
            foreach (IDteq dteq in DteqList) {
                //dteq.FedFromChanged -= this.OnFedFromChanged;

                UnregisterDteqFromLoadEvents(dteq);
            }
        }
        public void UnregisterDteqFromLoadEvents(IDteq dteq)
        {
            foreach (var assignedLoad in dteq.AssignedLoads) {
                assignedLoad.LoadingCalculated -= dteq.OnAssignedLoadReCalculated;
                //assignedLoad.PropertyUpdated -= DaManager.OnDteqPropertyUpdated;
            }
        }
        public void AssignLoadsAndEventsToAllDteq()
        {
            foreach (IDteq dteq in DteqList) {
                dteq.AssignedLoads.Clear();

                AssignLoadsAndSubscribeToEvents(dteq);
                dteq.AssignedLoads = new ObservableCollection<IPowerConsumer>(dteq.AssignedLoads.OrderBy(x => x.SequenceNumber).ToList());
                dteq.CalculateLoading();
            }
        }
        public void AssignLoadsAndSubscribeToEvents(IDteq dteq)
        {
            dteq.AssignedLoads.Clear();
            foreach (var dteqAsLoad in IDteqList) {
                if (dteqAsLoad.FedFrom != null) {
                    if (dteqAsLoad.FedFrom.Id == dteq.Id && dteqAsLoad.FedFrom.Type == dteq.Type) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteqAsLoad.LoadingCalculated += dteq.OnAssignedLoadReCalculated;
                        dteqAsLoad.PropertyUpdated += DaManager.OnDteqPropertyUpdated;
                    }
                }
            }

            foreach (var load in LoadList) {
                if (load.FedFrom != null) {
                    if (load.FedFrom.Tag == dteq.Tag && load.FedFrom.Type == dteq.Type) {
                        dteq.AssignedLoads.Add(load);
                        load.LoadingCalculated += dteq.OnAssignedLoadReCalculated;
                    }

                }
            }
        }

        #endregion



        #region Lists
        public void AssignAreas()
        {
            foreach (var childArea in AreaList) {
                foreach (var parentArea in AreaList) {
                    if (childArea.ParentAreaId == parentArea.Id) {
                        childArea.ParentArea = parentArea;
                        break;
                    }
                }
            }

            foreach (var dteq in IDteqList) {
                foreach (var area in AreaList) {
                    if (dteq.AreaId == area.Id) {
                        dteq.Area = area;
                        area.PropertyChanged += dteq.OnAreaPropertiesChanged;
                        break;
                    }
                }
            }
            foreach (var load in LoadList) {
                foreach (var area in AreaList) {
                    if (load.AreaId == area.Id) {
                        load.Area = area;
                        area.PropertyChanged += load.OnAreaPropertiesChanged;
                        break;
                    }
                }
            }
            foreach (var comp in CompList) {
                foreach (var area in AreaList) {
                    if (comp.AreaId == area.Id) {
                        comp.Area = area;
                        area.PropertyChanged += comp.OnAreaPropertiesChanged;
                        break;
                    }
                }
            }
            foreach (var lcs in LcsList) {
                foreach (var area in AreaList) {
                    if (lcs.AreaId == area.Id) {
                        lcs.Area = area;
                        area.PropertyChanged += lcs.OnAreaPropertiesChanged;
                        break;
                    }
                }
            }
        }

        public void SaveAll()
        {
            //Area
            foreach (var area in AreaList) {
                if (area.Tag != GlobalConfig.UtilityTag) {
                    DaManager.UpsertArea((AreaModel)area);
                }
            }
            //Dteq
            foreach (var dteq in IDteqList) {
                if (dteq.Tag != GlobalConfig.UtilityTag) {
                    DaManager.UpsertDteqAsync(dteq);
                }
            }

            //Load
            foreach (var load in LoadList) {
                DaManager.UpsertLoadAsync((LoadModel)load);
            }

            //Load
            foreach (var comp in CompList) {
                DaManager.UpsertComponentAsync((ComponentModel)comp);
            }

            foreach (var pd in PdList) {
                DaManager.UpsertComponentAsync((ProtectionDeviceModel)pd);
            }

            //Cables
            foreach (var cable in CableList) {
                DaManager.UpsertCable(cable);
            }
        }

        public void AssignCables()
        {
            foreach (var dteq in IDteqList) {
                foreach (var cable in CableList) {
                    //if (dteq.Id == cable.OwnerId && dteq.GetType().ToString() == cable.OwnerType) {
                    if (dteq.Id == cable.LoadId && cable.LoadType == dteq.GetType().ToString()) {
                        dteq.PowerCable = cable;
                        cable.Load = dteq;
                        cable.DestinationModel = dteq;
                        cable.CreateTypeList(dteq);
                        break;
                    }
                }
            }

            foreach (var load in LoadList) {
                foreach (var cable in CableList) {
                    //if (load.Id == cable.OwnerId && load.GetType().ToString() == cable.OwnerType) {
                    if (load.Id == cable.LoadId && cable.LoadType == load.GetType().ToString()) {
                        load.PowerCable = cable;
                        cable.Load = load;
                        cable.DestinationModel = load;
                        cable.CreateTypeList(load);
                    }
                }
            }
            //Protection Devices
            foreach (var pd in PdList) {
                foreach (var cable in CableList) {
                    if (pd.IsStandAlone == true && pd.Id == cable.OwnerId && pd.GetType().ToString() == cable.OwnerType) {
                        pd.PowerCable = cable;
                        cable.DestinationModel = pd;
                        break;
                    }
                }
            }

            //Cct Components
            foreach (var comp in CompList) {
                foreach (var cable in CableList) {
                    if (comp.Id == cable.OwnerId && comp.GetType().ToString() == cable.OwnerType) {
                        comp.PowerCable = cable;
                        cable.DestinationModel = comp;
                        break;
                    }
                }
            }

            //LCS
            foreach (var lcs in LcsList) {
                string lcsType = lcs.GetType().ToString();

                foreach (var cable in CableList) {

                    if (lcs.Id == cable.OwnerId) {
                        if (lcs.GetType().ToString() == cable.OwnerType && cable.UsageType == CableUsageTypes.Control.ToString()) {
                            lcs.ControlCable = cable;
                            cable.DestinationModel = lcs;
                        }

                        else if (lcs.GetType().ToString() == cable.OwnerType && cable.UsageType == CableUsageTypes.Instrument.ToString()) {
                            lcs.AnalogCable = cable;
                            cable.DestinationModel = lcs;
                        }
                    }
                }
            }

            //cable DaManager.PropertyUpdated, SourceModel, DestinationModel
            foreach (var eq in EqList) {
                foreach (var cable in CableList) {
                    cable.PropertyUpdated += DaManager.OnPowerCablePropertyUpdated;

                    if (eq.Id == cable.SourceId && cable.SourceType == eq.GetType().ToString()) {
                        cable.SourceModel = eq;
                    }

                }
            }
        }
        public void AddDteq(IDteq iDteq)
        {
            if (iDteq == null) {
                return;
            }
            if (iDteq.GetType() == typeof(DteqModel)) {
                var model = (DteqModel)iDteq;
                DteqList.Add(model);
            }
            else if (iDteq.GetType() == typeof(XfrModel)) {
                var model = (XfrModel)iDteq;
                XfrList.Add(model);
                DteqList.Add(model);
            }
            else if (iDteq.GetType() == typeof(SwgModel)) {
                var model = (SwgModel)iDteq;
                SwgList.Add(model);
                DteqList.Add(model);
            }
            else if (iDteq.GetType() == typeof(MccModel)) {
                var model = (MccModel)iDteq;
                MccList.Add(model);
                DteqList.Add(model);
            }
            else if (iDteq.GetType() == typeof(DpnModel)) {
                var model = (DpnModel)iDteq;
                DpnList.Add(model);
                DteqList.Add(model);
            }
            else if (iDteq.GetType() == typeof(SplitterModel)) {
                var model = (SplitterModel)iDteq;
                SplitterList.Add(model);
                DteqList.Add(model);
            }
            IDteqList.Add(iDteq);
        }

        //TODO move to Distribution Manager with _listManager as parameter
        public void DeleteDteq(IDteq IDteq)
        {
            IDteqList.Remove(IDteq);

            if (IDteq.GetType() == typeof(DteqModel)) {
                var model = IDteq as DteqModel;
                DteqList.Remove(model);
                IDteqList.Remove(model);
            }
            else if (IDteq.GetType() == typeof(XfrModel)) {
                var model = IDteq as XfrModel;
                XfrList.Remove(model);
                DteqList.Remove(model);
                IDteqList.Remove(model);

            }
            else if (IDteq.GetType() == typeof(SwgModel)) {
                var model = IDteq as SwgModel;
                SwgList.Remove(model);
                DteqList.Remove(model);
                IDteqList.Remove(model);

            }
            else if (IDteq.GetType() == typeof(MccModel)) {
                var model = IDteq as MccModel;
                MccList.Remove(model);
                DteqList.Remove(model);
                IDteqList.Remove(model);
            }

            else if (IDteq.GetType() == typeof(DpnModel)) {
                var model = IDteq as DpnModel;
                DpnList.Remove(model);
                DteqList.Remove(model);
                IDteqList.Remove(model);
            }
            return;
        }

        #endregion

    }
}
