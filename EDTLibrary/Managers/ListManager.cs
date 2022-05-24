﻿using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDTLibrary
{
    [AddINotifyPropertyChangedInterface]
    public class ListManager {

        public ObservableCollection<IArea> AreaList { get; set; } = new ObservableCollection<IArea>();


        public ObservableCollection<IEquipment>  EqList { get; set; } = new ObservableCollection<IEquipment>();
        public ObservableCollection<IDteq> IDteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<DistributionEquipment> DteqList { get; set; } = new ObservableCollection<DistributionEquipment>();
        public ObservableCollection<XfrModel> XfrList { get; set; } = new ObservableCollection<XfrModel>();
        public ObservableCollection<SwgModel> SwgList { get; set; } = new ObservableCollection<SwgModel>();
        public ObservableCollection<MccModel> MccList { get; set; } = new ObservableCollection<MccModel>();


        public ObservableCollection<ILoad> LoadList { get; set; } = new ObservableCollection<ILoad>();
        public ObservableCollection<IComponent> CompList { get; set; } = new ObservableCollection<IComponent>();

        public ObservableCollection<PowerCableModel> CableList { get; set; } = new ObservableCollection<PowerCableModel>();


        //public async Task SetDteq()
        //{
        //    Task<ObservableCollection<DteqModel>> dteqList;
        //    dteqList = Task.Run(() => DaManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable));
        //    await Task.Delay(1000);
        //    CreateDteqDict();
        //}

        public void CreateEquipmentList()
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
        }
        public void GetProjectTablesAndAssigments()
        {
            GlobalConfig.GettingRecords = true;

            try {
                //Get
                GetAreas();
                GetDteq();
                GetLoads();
                //TODO - Get Components for each type and create a master component list
                GetComponents();
                AssignComponents();

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

            GlobalConfig.GettingRecords = false;
        }

        
        public ObservableCollection<IArea> GetAreas()
        {
            AreaList.Clear();
            var list = DaManager.prjDb.GetRecords<AreaModel>(GlobalConfig.AreaTable);
            foreach (var item in list) {
                AreaList.Add(item);
            }
            return AreaList;
        }
        public ObservableCollection<DistributionEquipment> GetDteq() {

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

            DteqList.Insert(0, new DteqModel() { Tag = GlobalConfig.Utility });


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
                if (dteq.FedFromTag.Contains("Deleted") || dteq.FedFromType.Contains("Deleted")) {
                    dteq.FedFrom = new DteqModel() { Tag = GlobalConfig.Deleted };
                }

                //Actual and Valid
                fedFrom = IDteqList.FirstOrDefault(d => d.Id == dteq.FedFromId &&
                                                   d.GetType().ToString() == dteq.FedFromType);
                if (fedFrom != null) {
                    dteq.FedFrom = fedFrom;
                }
            }


            return DteqList;
        }
        public ObservableCollection<ILoad> GetLoads() {
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
            //CreateILoadDict();
            return LoadList;
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
                        comp.PropertyUpdated += DaManager.OnComponentPropertyUpdated;
                        comp.Owner = load;
                        //Aux Components
                        if (comp.Category == Categories.AuxComponent.ToString()) {
                            load.AuxComponents.Add(comp);
                            if (comp.Type == ComponentTypes.LCS.ToString()) {
                                load.Lcs = (ComponentModel)comp;
                            }
                        }

                        //Cct Components
                        else if (comp.Category == Categories.CctComponent.ToString()) {
                            load.CctComponents.Add(comp);
                        }
                    }
                }
            }
        }

        public ObservableCollection<PowerCableModel> GetCables()
        {
            CableList.Clear();
            CableList = DaManager.prjDb.GetRecords<PowerCableModel>(GlobalConfig.PowerCableTable);
            AssignCableTypes();
            return CableList;
        }
        private void AssignCableTypes()
        {
            Random random = new Random();
            foreach (var cable in CableList) {
                cable.TypeModel = TypeManager.GetCableType(cable.Type);
#if DEBUG
                //if (cable.Length==0) {
                    cable.Length = random.Next(1, 750);
                //}
#endif
            }        
        }


        #region MajorEquipment
        public async Task CalculateDteqLoadingAsync() // LoadList Manager
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
                GlobalConfig.GettingRecords = true;
                {
                    foreach (var load in LoadList) {
                        sw.Restart();

                        load.CalculateLoading();
                        load.PowerCable.CalculateCableQtyAndSize();
                        load.PowerCable.CalculateAmpacity(load);

                        subTotal += sw.Elapsed.TotalMilliseconds;

                    }
                }
                GlobalConfig.GettingRecords = false;

                Debug.Print("Loads: " + subTotal);
                total += subTotal;
                subTotal = 0;

                //Dteq
                foreach (var dteq in IDteqList) {
                    sw.Restart();

                    dteq.CalculateLoading();
                    dteq.PowerCable.CalculateCableQtyAndSize();
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
                assignedLoad.PropertyUpdated -= DaManager.OnDteqPropertyUpdated;
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


        //Lists
        #region Lists
        public void CreateCableList() {
            CableList.Clear();
            foreach (var dteq in IDteqList) {
                dteq.PowerCable.AssignOwner(dteq);
                if (dteq.PowerCable.OwnerId != null && dteq.PowerCable.OwnerType != null) {
                    CableList.Add(dteq.PowerCable);
                }
            }
            foreach (var load in LoadList) {
                load.PowerCable.AssignOwner(load);
                if (load.PowerCable.OwnerId != null && load.PowerCable.OwnerType != null) {
                    CableList.Add(load.PowerCable);

                }
            }
        }
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
                    DaManager.UpsertDteq(dteq);
                }
            }

            //Load
            foreach (var load in LoadList) {
                load.PowerCable.AssignOwner(load);
                DaManager.UpsertLoad((LoadModel)load);
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
                    if (dteq.Id == cable.OwnerId &&
                        dteq.GetType().ToString() == cable.OwnerType) {
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
                    if (load.Id == cable.OwnerId &&
                        load.GetType().ToString() == cable.OwnerType) {
                        load.PowerCable = cable;
                        cable.Load = load;
                        cable.CreateTypeList(load);
                        cable.PropertyUpdated += DaManager.OnPowerCablePropertyUpdated;
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
        public void DeleteDteq(IDteq IDteq) //where T : class
        {
            IDteqList.Remove(IDteq);

            if (IDteq.GetType() == typeof(DteqModel)) {
                var model = IDteq as DteqModel;
                DteqList.Remove(model);
            }
            else if (IDteq.GetType() == typeof(XfrModel)) {
                var model = IDteq as XfrModel;
                XfrList.Remove(model);
                DteqList.Remove(model);

            }
            else if (IDteq.GetType() == typeof(SwgModel)) {
                var model = IDteq as SwgModel;
                SwgList.Remove(model);
                DteqList.Remove(model);

            }
            else if (IDteq.GetType() == typeof(MccModel)) {
                var model = IDteq as MccModel;
                MccList.Remove(model);
                DteqList.Remove(model);

            }
            return;
        }

        #endregion

    }
}