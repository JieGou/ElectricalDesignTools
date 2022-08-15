using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Models;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDTLibrary.Managers
{
    [AddINotifyPropertyChangedInterface]
    public class ListManager
    {

        public ObservableCollection<IArea> AreaList { get; set; } = new ObservableCollection<IArea>();


        public ObservableCollection<IEquipment> EqList { get; set; } = new ObservableCollection<IEquipment>();
        public ObservableCollection<IDteq> IDteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<DistributionEquipment> DteqList { get; set; } = new ObservableCollection<DistributionEquipment>();
        public ObservableCollection<XfrModel> XfrList { get; set; } = new ObservableCollection<XfrModel>();
        public ObservableCollection<SwgModel> SwgList { get; set; } = new ObservableCollection<SwgModel>();
        public ObservableCollection<MccModel> MccList { get; set; } = new ObservableCollection<MccModel>();


        public ObservableCollection<ILoad> LoadList { get; set; } = new ObservableCollection<ILoad>();
        public ObservableCollection<IComponentEdt> CompList { get; set; } = new ObservableCollection<IComponentEdt>();
        public ObservableCollection<DriveModel> DriveList { get; set; } = new ObservableCollection<DriveModel>();
        public ObservableCollection<DisconnectModel> DisconnectList { get; set; } = new ObservableCollection<DisconnectModel>();
        public ObservableCollection<ILocalControlStation> LcsList { get; set; } = new ObservableCollection<ILocalControlStation>();

        public ObservableCollection<CableModel> CableList { get; set; } = new ObservableCollection<CableModel>();


        public void GetProjectTablesAndAssigments()
        {
            DaManager.GettingRecords = true;

            try {
                //Get
                GetAreas();
                GetDteq();
                GetLoads();

                //TODO - Get Components for each type and create a master component list
                GetDrives();
                GetDisconnects();
                GetComponents();
                AssignComponents();

                GetLocalControlStations();
                AssignLocalControlStations();
                GetCables();

                //Assign
                AssignAreas();
                AssignLoadsAndEventsToAllDteq();
                AssignCables();
            }
            catch (Exception ex) {

                if (ex.Data.Contains("UserMessage") == false) {
                    ex.Data.Add("UserMessage", "The project database may have been deleted or corrupted since opening. Go to the home screen and reopen the project.");
                }
                throw;
            }

            DaManager.GettingRecords = false;
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
            return EqList;
        }

        private void GetDisconnects()
        {
            DisconnectList.Clear();
            var list = DaManager.prjDb.GetRecords<DisconnectModel>(GlobalConfig.DisconnectTable);
            foreach (var item in list) {
                DisconnectList.Add(item);
                item.PropertyUpdated += DaManager.OnDisconnectPropertyUpdated;

            }
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
            return AreaList;
        }
        private void GetDteq()
        {

            IDteqList.Clear();
            DteqList.Clear();
            XfrList.Clear();
            SwgList.Clear();
            MccList.Clear();

            //Dteq
            //TODO - Clean up DteqModel vs abstract Dteq
            var list = DaManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable);
            foreach (var item in list) {
                DteqList.Add(item);
            }

            IDteq fedFrom;

            foreach (var model in DteqList) {
                IDteqList.Add(model);
            }

            DteqList.Insert(0, new DteqModel() { Tag = GlobalConfig.Utility, Area = new AreaModel() });


            //XFR
            XfrList = DaManager.prjDb.GetRecords<XfrModel>(GlobalConfig.XfrTable);
            foreach (var model in XfrList) {
                IDteqList.Add(model);
                DteqList.Add(model);
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

            //Assign FedFrom
            foreach (var dteq in IDteqList) {
                //Utility

                if (dteq.FedFromTag == GlobalConfig.Utility) {
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
        private void GetLoads()
        {

            var list = DaManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadTable); //new List<LoadModel>(); //
            LoadList.Clear();
            foreach (var item in list) {
                LoadList.Add(item);
            }
            IDteq fedFrom;

            foreach (var load in LoadList) {

                if (load.FedFromTag.Contains("Deleted") || load.FedFromType.Contains("Deleted")) {
                    load.FedFrom = GlobalConfig.DteqDeleted;
                }
                fedFrom = IDteqList.FirstOrDefault(d => d.Id == load.FedFromId &&
                                                   d.GetType().ToString() == load.FedFromType);
                if (fedFrom != null) load.FedFrom = fedFrom;

                if (load.Description == null) load.Description = "";
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
                        if (comp.SubCategory == SubCategories.CctComponent.ToString()) {
                            load.CctComponents.Add(comp);
                            load.CctComponents.OrderBy(c => comp.SequenceNumber);
                            if (comp.SubType == ComponentSubTypes.DefaultDrive.ToString()) {
                                load.Drive = (ComponentModel)comp;
                                load.FedFrom.AreaChanged += comp.MatchOwnerArea;
                            }
                            if (comp.SubType == ComponentSubTypes.DefaultDcn.ToString()) {
                                load.Disconnect = (ComponentModel)comp;
                                load.AreaChanged += comp.MatchOwnerArea;
                            }
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
            Random random = new Random();
            foreach (var cable in CableList) {
                cable.TypeModel = TypeManager.GetCableTypeModel(cable.Type);
                cable.CreateSizeList();
            }
        }


        #region MajorEquipment
        //Move to Distribution Manager
        public async Task CalculateDteqLoadingAsync()
        {
            await Task.Run(() => {
                Stopwatch sw = new Stopwatch();
                sw.Restart();
                Debug.Print("CalculateDteqLoadingAsync Start");
                UnregisterAllDteqFromAllLoadEvents();
                AssignLoadsAndEventsToAllDteq();
                double total = 0;
                double subTotal = 0;
                Debug.Print(sw.Elapsed.TotalMilliseconds.ToString());

                //Loads
                DaManager.GettingRecords = false;
                {
                    foreach (var load in LoadList) {
                        sw.Restart();

                        load.CalculateLoading();
                        load.PowerCable.AutoSize();
                        load.PowerCable.CalculateAmpacity(load);

                        subTotal += sw.Elapsed.TotalMilliseconds;

                    }
                }
                DaManager.GettingRecords = false;

                Debug.Print("Loads: " + subTotal);
                total += subTotal;
                subTotal = 0;

                //Dteq
                foreach (var dteq in IDteqList) {
                    sw.Restart();

                    dteq.CalculateLoading();
                    dteq.PowerCable.AutoSize();
                    dteq.PowerCable.CalculateAmpacity(dteq);

                    subTotal += sw.Elapsed.TotalMilliseconds;

                }
                Debug.Print("Dteq: " + subTotal);
                total += subTotal;
                Debug.Print("Total: " + total);
            });

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
                    load.PropertyUpdated += DaManager.OnLoadPropertyUpdated;

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
                if (area.Tag != GlobalConfig.Utility) {
                    DaManager.UpsertArea((AreaModel)area);
                }
            }
            //Dteq
            foreach (var dteq in DteqList) {
                if (dteq.Tag != GlobalConfig.Utility) {
                    dteq.PowerCable.AssignOwner(dteq);
                    DaManager.UpsertDteqAsync(dteq);
                }
            }

            //Load
            foreach (var load in LoadList) {
                load.PowerCable.AssignOwner(load);
                DaManager.UpsertLoadAsycn((LoadModel)load);
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
                            cable.CreateTypeList(dteq);
                            cable.PropertyUpdated += DaManager.OnPowerCablePropertyUpdated;
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
                            cable.CreateTypeList(load);
                            cable.PropertyUpdated += DaManager.OnPowerCablePropertyUpdated;
                            break;
                    }
                }
            }

            foreach (var comp in CompList) {
                foreach (var cable in CableList) {
                    if (comp.Id == cable.OwnerId &&  comp.GetType().ToString() == cable.OwnerType) 
                    {
                        comp.PowerCable = cable;
                        break;
                    }
                }
            }

            foreach (var lcs in LcsList) {
                string lcsType = lcs.GetType().ToString();

                foreach (var cable in CableList) {

                    if (lcs.Id == cable.OwnerId &&
                        lcs.GetType().ToString() == cable.OwnerType && cable.UsageType == CableUsageTypes.Control.ToString()) {
                        lcs.ControlCable = cable;
                        break;
                    }
                    else if (lcs.Id == cable.OwnerId &&
                        lcs.GetType().ToString() == cable.OwnerType && cable.UsageType == CableUsageTypes.Instrument.ToString()) {
                        lcs.ControlCable = cable;
                        break;
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
            return;
        }

        #endregion

    }
}
