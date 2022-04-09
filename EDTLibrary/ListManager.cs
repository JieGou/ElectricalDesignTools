using EDTLibrary.DataAccess;
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

        public ObservableCollection<IArea>          AreaList { get; set; } = new ObservableCollection<IArea>();
        public ObservableCollection<IDteq>              IDteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<DistributionEquipment>          DteqList { get; set; } = new ObservableCollection<DistributionEquipment>();
        public ObservableCollection<XfrModel>           XfrList { get; set; } = new ObservableCollection<XfrModel>();
        public ObservableCollection<SwgModel>           SwgList { get; set; } = new ObservableCollection<SwgModel>();
        public ObservableCollection<MccModel>           MccList { get; set; } = new ObservableCollection<MccModel>();


        public ObservableCollection<ILoad>          LoadList { get; set; } = new ObservableCollection<ILoad>();
        public ObservableCollection<PowerCableModel>    CableList { get; set; } = new ObservableCollection<PowerCableModel>();
        public ObservableCollection<CctComponentModel>  CompList { get; set; } = new ObservableCollection<CctComponentModel>();

        public Dictionary<string, IEquipment>           eqDict { get; set; } = new Dictionary<string, IEquipment>();
        public Dictionary<string, IDteq>                dteqDict { get; set; } = new Dictionary<string, IDteq>();
        public Dictionary<string, IPowerConsumer>       iLoadDict { get; set; } = new Dictionary<string, IPowerConsumer>();

        public async Task SetDteq()
        {
            Task<ObservableCollection<DteqModel>> dteqList;
            dteqList = Task.Run(() => DaManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable));
            await Task.Delay(1000);
            CreateDteqDict();
        }
        public void GetProjectTablesAndAssigments()
        {
            GlobalConfig.GettingRecords = true;

            //Get
            GetAreas();
            GetDteq();
            GetLoads();
            GetCables();

            //Assign
            AssignAreas();
            AssignLoadsAndEventsToAllDteq();
            AssignCables();

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

            CreateDteqDict();

            return DteqList;
        }
        public ObservableCollection<ILoad> GetLoads() {     
            var list = DaManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadTable);
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
                if (fedFrom != null) {
                    load.FedFrom = fedFrom;
                }

            }
            //CreateILoadDict();
            return LoadList;
        }
        public ObservableCollection<PowerCableModel> GetCables()
        {
            CableList = DaManager.prjDb.GetRecords<PowerCableModel>(GlobalConfig.PowerCableTable);
            AssignCableTypes();
            return CableList;
        }
        private void AssignCableTypes()
        {
            foreach (var cable in CableList) {
                cable.TypeModel = TypeManager.GetCableType(cable.Type);
            }        
        }

        public void CreateILoadDict() {
            iLoadDict.Clear();
            
            foreach (var eq in LoadList) {
                iLoadDict.Add(eq.Tag, eq);
            }
        }
        public void CreateDteqDict() {
            dteqDict.Clear();
            foreach (var dteq in DteqList) {
                if (dteqDict.ContainsKey(dteq.Tag) == false) {
                    dteqDict.Add(dteq.Tag, dteq);
                }
            }
        }


        #region MajorEquipment
        public void CalculateDteqLoading() // LoadList Manager
        {

            AssignLoadsAndEventsToAllDteq();
            foreach (var load in LoadList) {
                //load.CalculateLoading();
                //load.PowerCable.GetRequiredAmps(load);
                load.PowerCable.CalculateCableQtySizeNew();
                load.PowerCable.CalculateAmpacityNew(load);
            }
            foreach (var dteq in IDteqList) {
                //dteq.CalculateLoading();
                //dteq.PowerCable.GetRequiredAmps(dteq);
                dteq.PowerCable.CalculateCableQtySizeNew();
                dteq.PowerCable.CalculateAmpacityNew(dteq);
            }
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
                assignedLoad.LoadingCalculated -= DaManager.OnDteqLoadingCalculated;
            }
        }
        public void AssignLoadsAndEventsToAllDteq()
        {
            foreach (IDteq dteq in DteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;

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
                        dteqAsLoad.LoadingCalculated += DaManager.OnDteqLoadingCalculated;
                    }
                }
            }

            foreach (var load in LoadList) {
                if (load.FedFrom != null) {
                    if (load.FedFrom.Tag == dteq.Tag && load.FedFrom.Type == dteq.Type) {
                        dteq.AssignedLoads.Add(load);
                        load.LoadingCalculated += dteq.OnAssignedLoadReCalculated;
                        load.LoadingCalculated += DaManager.OnLoadLoadingCalculated;
                    }
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
                if (dteq.PowerCable.OwnedById != null && dteq.PowerCable.OwnedByType != null) {
                    CableList.Add(dteq.PowerCable);
                }
            }
            foreach (var load in LoadList) {
                load.PowerCable.AssignOwner(load);
                if (load.PowerCable.OwnedById != null && load.PowerCable.OwnedByType != null) {
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
        public void AssignCables()
        {
            foreach (var dteq in IDteqList) {
                foreach (var cable in CableList) {
                    if (dteq.Id == cable.OwnedById &&
                        dteq.GetType().ToString() == cable.OwnedByType) {
                        dteq.PowerCable = cable;
                        cable.Load = dteq;
                        break;
                    }
                }
            }
            foreach (var load in LoadList) {
                foreach (var cable in CableList) {
                    if (load.Id == cable.OwnedById &&
                        load.GetType().ToString() == cable.OwnedByType) {
                        load.PowerCable = cable;
                        cable.Load = load;
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
