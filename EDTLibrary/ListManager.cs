using EDTLibrary.DataAccess;
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

        public ObservableCollection<AreaModel>          AreaList { get; set; } = new ObservableCollection<AreaModel>();
        public ObservableCollection<IDteq>              IDteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<DteqModel>          DteqList { get; set; } = new ObservableCollection<DteqModel>();
        public ObservableCollection<XfrModel>           TransformerList { get; set; } = new ObservableCollection<XfrModel>();


        public ObservableCollection<LoadModel>          LoadList { get; set; } = new ObservableCollection<LoadModel>();
        public ObservableCollection<PowerCableModel>    CableList { get; set; } = new ObservableCollection<PowerCableModel>();
        public ObservableCollection<CctComponentModel>  CompList { get; set; } = new ObservableCollection<CctComponentModel>();

        public Dictionary<string, IEquipment>           eqDict { get; set; } = new Dictionary<string, IEquipment>();
        public Dictionary<string, IDteq>                dteqDict { get; set; } = new Dictionary<string, IDteq>();
        public Dictionary<string, IPowerConsumer>       iLoadDict { get; set; } = new Dictionary<string, IPowerConsumer>();

        public async Task SetDteq()
        {
            Task<ObservableCollection<DteqModel>> dteqList;
            dteqList = Task.Run(() => DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable));
            await Task.Delay(1000);
            CreateDteqDict();
        }
        public ObservableCollection<DteqModel> GetDteq() {

            IDteqList.Clear();

            //Dteq
            DteqList = DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable);
            IDteq fedFrom;

            foreach (var dteq in DteqList) {
                IDteqList.Add(dteq);
            }

            DteqList.Insert(0, new DteqModel() { Tag = "UTILITY" });

            foreach (var dteq in IDteqList) {
                if (dteq.FedFromTag == "UTILITY") {
                    dteq.FedFrom = DteqList[0];
                }
                if (dteq.FedFromTag.Contains("Deleted") || dteq.FedFromType.Contains("Deleted")) {
                    dteq.FedFrom = new DteqModel() { Tag = GlobalConfig.Deleted };
                }
                fedFrom = DteqList.FirstOrDefault(d => d.Id == dteq.FedFromId &&
                                                   d.GetType().ToString() == dteq.FedFromType);
                if (fedFrom != null) {
                    dteq.FedFrom = fedFrom;
                }
            }

            //XFR

            CreateDteqDict();

            return DteqList;
        }

        //TODO move to Distribution Manager with _listManager as parameter
        public void DeleteDteq(IDteq model) //where T : class
        {
            if (model.GetType()==typeof (DteqModel)){
                var dteq = model as DteqModel;
                DteqList.Remove(dteq);
                IDteqList.Remove(dteq);
                DbManager.prjDb.DeleteRecord(GlobalConfig.DteqTable, dteq.Id);
            }
            else if (model.GetType()==typeof (XfrModel)) {
               var xfr = model as XfrModel;
                TransformerList.Remove(xfr);
                IDteqList.Remove(xfr);
            }
        }

        public ObservableCollection<LoadModel> GetLoads() {     
            LoadList = DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadTable);
            IDteq fedFrom;

            foreach (var load in LoadList) {
               
                if (load.FedFromTag.Contains("Deleted") || load.FedFromType.Contains("Deleted")) {
                    load.FedFrom = new DteqModel() { Tag = "* Deleted *" };
                }
                fedFrom = IDteqList.FirstOrDefault(d => d.Id == load.FedFromId &&
                                                   d.GetType().ToString() == load.FedFromType);
                if (fedFrom != null) {
                    load.FedFrom = fedFrom;
                }

            }
            CreateILoadDict();
            return LoadList;
        }
        public ObservableCollection<PowerCableModel> GetCables()
        {
            CableList = DbManager.prjDb.GetRecords<PowerCableModel>(GlobalConfig.PowerCableTable);
            return CableList;
        }
        public ObservableCollection<AreaModel> GetAreas()
        {
            AreaList = DbManager.prjDb.GetRecords<AreaModel>(GlobalConfig.AreaTable);
            return AreaList;
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

            AssignLoadsToAllDteq();
            foreach (var load in LoadList) {
                load.CalculateLoading();
            }
        }
        public void OnFedFromChanged(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //TODO - Unregister from specific Dteq only
            //UnregisterAllDteqFromAllLoadEvents();
            //AssignLoadsToAllDteq();

            stopwatch.Stop();
            Debug.Print(stopwatch.ElapsedMilliseconds.ToString());
        }

        public void UnregisterAllDteqFromAllLoadEvents()
        {
            foreach (DteqModel dteq in DteqList) {
                //dteq.FedFromChanged -= this.OnFedFromChanged;

                UnregisterDteqFromLoadEvents(dteq);
            }
        }
        public void UnregisterDteqFromLoadEvents(IDteq dteq)
        {
            foreach (var assignedLoad in dteq.AssignedLoads) {
                assignedLoad.LoadingCalculated -= dteq.OnAssignedLoadReCalculated;
                assignedLoad.LoadingCalculated -= DbManager.OnDteqLoadingCalculated;
            }
        }

        public void AssignLoadsToAllDteq()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;
                //dteq.FedFromChanged += this.OnFedFromChanged;

                AssignLoadsAndSubscribeToEvents(dteq);
                dteq.CalculateLoading();
            }
        }
        
        public void AssignLoadsAndSubscribeToEvents(IDteq dteq)
        {
            dteq.AssignedLoads.Clear();
            foreach (var dteqAsLoad in IDteqList) {
                if (dteqAsLoad.FedFrom != null) {
                    if (dteqAsLoad.FedFrom.Id == dteq.Id) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteqAsLoad.LoadingCalculated += dteq.OnAssignedLoadReCalculated;
                        dteqAsLoad.LoadingCalculated += DbManager.OnDteqLoadingCalculated;
                    }
                }
            }

            foreach (var load in LoadList) {
                if (load.FedFrom != null) {
                    if (load.FedFrom.Tag == dteq.Tag) {
                        dteq.AssignedLoads.Add(load);
                        load.LoadingCalculated += dteq.OnAssignedLoadReCalculated;
                        load.LoadingCalculated += DbManager.OnLoadLoadingCalculated;
                    }
                }
            }
        }


        public void CalculateDteqLoading(ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList) // LoadList Manager
        {
            foreach (DteqModel dteq in dteqList) {
                var dteqTag = dteq.Tag;
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;

                //Adds Dteq as Loads
                foreach (DteqModel dteqAsLoad in dteqList) {
                    var dteqAsLoadTag = dteqAsLoad.Tag;
                    if (dteqAsLoad.FedFromTag == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in loadList) {
                    var loadTag = load.Tag;
                    if (load.FedFromTag == dteq.Tag) {
                        load.CalculateLoading();
                        dteq.AssignedLoads.Add(load);
                        dteq.LoadCount += 1;
                    }
                }
            }
            //Calculates the loading of each load. Recursive for dteq
            foreach (DteqModel dteq in dteqList) {
                dteq.CalculateLoading();
            }

            DteqList = new ObservableCollection<DteqModel>(dteqList);

            LoadList = new ObservableCollection<LoadModel>(loadList);
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
    
        #endregion

    }
}
