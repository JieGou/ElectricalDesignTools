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
        public ObservableCollection<XfrModel>   TransformerList { get; set; } = new ObservableCollection<XfrModel>();


        public ObservableCollection<LoadModel>          LoadList { get; set; } = new ObservableCollection<LoadModel>();
        public ObservableCollection<PowerCableModel>    CableList { get; set; } = new ObservableCollection<PowerCableModel>();
        public ObservableCollection<CctComponentModel>  CompList { get; set; } = new ObservableCollection<CctComponentModel>();

        public Dictionary<string, DteqModel>            dteqDict { get; set; } = new Dictionary<string, DteqModel>();
        public Dictionary<string, IEquipment>           eqDict { get; set; } = new Dictionary<string, IEquipment>();
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
            foreach (var item in DteqList) {
                IDteqList.Add(item);
            }

            //XFR
            IDteqList.Add(new XfrModel() { Tag = "asdfasdf" });

            CreateDteqDict();
            return DteqList;
        }
        public void DeteteDteq<T>(T model) where T : class
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
            foreach (var eq in DteqList) {
                iLoadDict.Add(eq.Tag, eq);
            }
            foreach (var eq in LoadList) {
                iLoadDict.Add(eq.Tag, eq);
            }
        }
        public void CreateDteqDict() {
            dteqDict.Clear();
            foreach (var dteq in DteqList) {
                dteqDict.Add(dteq.Tag, dteq);
            }
        }
        public void CreateDteqDict(ObservableCollection<DteqModel> dteqList)
        {
            dteqDict.Clear();
            foreach (var dteq in dteqList) {
                if (dteqDict.ContainsKey(dteq.Tag) ==false ) {
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
        public void OnFedFromCalculated(object sender, EventArgs e)
        {
            UnassignLoadsAllDteq();
            AssignLoadsToAllDteq();
        }

        public void UnassignLoadsAllDteq()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.FedFromChanged -= this.OnFedFromCalculated;

                UnassignLoadsDteq(dteq);
            }
        }
        public void AssignLoadsToAllDteq()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;
                dteq.FedFromChanged += this.OnFedFromCalculated;

               AssignLoadsToDteq(dteq);
            }
        }

        public void UnassignLoadsDteq(IDteq dteq)
        {
            foreach (var dteqAsLoad in dteq.AssignedLoads) {
                dteqAsLoad.LoadingCalculated -= dteq.OnDteqLoadingCalculated;
                dteqAsLoad.LoadingCalculated -= DbManager.OnDteqLoadingCalculated;
            }
        }
        public void AssignLoadsToDteq(IDteq dteq)
        {
            dteq.AssignedLoads.Clear();
            foreach (var dteqAsLoad in IDteqList) {
                if (dteqAsLoad.FedFrom == dteq.Tag) {
                    dteq.AssignedLoads.Add(dteqAsLoad);
                    dteqAsLoad.LoadingCalculated += dteq.OnDteqLoadingCalculated;
                    dteqAsLoad.LoadingCalculated += DbManager.OnDteqLoadingCalculated;
                }
            }
            foreach (var load in LoadList) {
                if (load.FedFrom == dteq.Tag) {
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
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in loadList) {
                    var loadTag = load.Tag;
                    if (load.FedFrom == dteq.Tag) {
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
            foreach (var item in DteqList) {
                item.Cable.AssignOwner(item);
                if (item.Cable.OwnedById != null && item.Cable.OwnedByType != null) {
                    CableList.Add(item.Cable);
                }
            }
            foreach (var item in LoadList) {
                item.Cable.AssignOwner(item);
                if (item.Cable.OwnedById != null && item.Cable.OwnedByType != null) {
                    CableList.Add(item.Cable);
                }
            }
        }
        public void AssignCables()
        {
            foreach (var dteq in DteqList) {
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
