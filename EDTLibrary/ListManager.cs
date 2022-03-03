using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDTLibrary
{
    [AddINotifyPropertyChangedInterface]
    public class ListManager {

        public ObservableCollection<LocationModel>      LocationList { get; set; } = new ObservableCollection<LocationModel>();
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

            foreach (var dteq in DteqList) {
                if (dteq.FedFromTag == "UTILITY") {
                    dteq.FedFrom = DteqList[0];
                }

                //fedFrom = DteqList.FirstOrDefault(d => d.Tag == dteq.FedFromTag);

                //if (fedFrom != null) {
                //    dteq.FedFromId = fedFrom.Id;
                //    dteq.FedFromType = fedFrom.GetType().ToString();
                    
                //}

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
        public void DeleteDteq<T>(T model) where T : class
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
                //fedFrom = IDteqList.FirstOrDefault(d => d.Tag == load.FedFrom.Tag);

                //if (fedFrom != null) {
                //    load.FedFromId = fedFrom.Id;
                //    load.FedFromType = fedFrom.GetType().ToString();
                //}

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
        public ObservableCollection<LocationModel> GetLocations()
        {
            LocationList = DbManager.prjDb.GetRecords<LocationModel>(GlobalConfig.LocationTable);
            return LocationList;
        }


        public void CreateEqDict() {
            eqDict.Clear();
            foreach (var item in DteqList) {
                eqDict.Add(item.Tag, item);
            }
            foreach (var item in LoadList) {
                eqDict.Add(item.Tag, item);
            }
            foreach (var item in CompList) {
                eqDict.Add(item.Tag, item);
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

            AssignLoadsToAllDteq();
            //Calculates the loading of each load. Recursive for dteq
            foreach (var item in LoadList) {
                item.CalculateLoading();
            }
        }
        public void OnFedFromChanged(object sender, EventArgs e)
        {
            UnassignLoadEventsAllDteq();
            AssignLoadsToAllDteq();
        }

        public void UnassignLoadEventsAllDteq()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.FedFromChanged -= this.OnFedFromChanged;

                UnassignLoadEventsDteq(dteq);
            }
        }
        public void AssignLoadsToAllDteq()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;
                dteq.FedFromChanged += this.OnFedFromChanged;

               AssignLoadsAndEventsToDteq(dteq);
                dteq.CalculateLoading();
            }
        }

        public void UnassignLoadEventsDteq(IDteq dteq)
        {
            foreach (var load in dteq.AssignedLoads) {
                load.LoadingCalculated -= dteq.OnDteqLoadingCalculated;
                load.LoadingCalculated -= DbManager.OnDteqLoadingCalculated;
            }
        }
        public void AssignLoadsAndEventsToDteq(IDteq dteq)
        {
            dteq.AssignedLoads.Clear();
            foreach (var dteqAsLoad in IDteqList) {
                if (dteqAsLoad.FedFrom.Tag == dteq.Tag) {
                    dteq.AssignedLoads.Add(dteqAsLoad);
                    dteqAsLoad.LoadingCalculated += dteq.OnDteqLoadingCalculated;
                    dteqAsLoad.LoadingCalculated += DbManager.OnDteqLoadingCalculated;
                }
            }
            foreach (var load in LoadList) {
                if (load.FedFrom.Tag == dteq.Tag) {
                    dteq.AssignedLoads.Add(load);
                    load.LoadingCalculated += dteq.OnDteqLoadingCalculated;
                    load.LoadingCalculated += DbManager.OnLoadLoadingCalculated;
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

        //Loads
        #region Loads
        public void CalculateLoads() {
            foreach (var load in LoadList) {
                load.CalculateLoading();
            }
        }
        public void CalculateLoads(ObservableCollection<LoadModel> loadList) {
            foreach (var load in loadList) {
                load.CalculateLoading();
            }
        }
        #endregion

        //Lists
        #region Lists
        /// <summary>
        /// Creates a list of ILoadModel for all equipment (not components)
        /// </summary>
       
 
        /// <summary>
        /// Creteas a list of CableModel for all equipment and components based on the masterLoadList
        /// </summary>
        /// 
        public void CreateCableList() {
            CableList.Clear();
            foreach (var dteq in IDteqList) {
                dteq.Cable.AssignOwner(dteq);
                if (dteq.Cable.OwnedById != null && dteq.Cable.OwnedByType != null) {
                    CableList.Add(dteq.Cable);
                }
            }
            foreach (var load in LoadList) {
                load.Cable.AssignOwner(load);
                if (load.Cable.OwnedById != null && load.Cable.OwnedByType != null) {
                    CableList.Add(load.Cable);
                }
            }
        }
        public void AssignCables()
        {
            foreach (var dteq in IDteqList) {
                foreach (var cable in CableList) {
                    if (dteq.Id == cable.OwnedById &&
                        dteq.GetType().ToString() == cable.OwnedByType) {
                        dteq.Cable = cable;
                    }
                }
            }
            foreach (var load in LoadList) {
                foreach (var cable in CableList) {
                    if (load.Id == cable.OwnedById &&
                        load.GetType().ToString() == cable.OwnedByType) {
                        load.Cable = cable;
                    }
                }
            }
        }
    
        #endregion

    }
}
