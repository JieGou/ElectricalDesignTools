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

namespace EDTLibrary
{
    [AddINotifyPropertyChangedInterface]
    public class ListManager {

        public ObservableCollection<LocationModel> LocationList { get; set; } = new ObservableCollection<LocationModel>();


        public static ObservableCollection<IDteq> IDteqList { get; set; } = new ObservableCollection<IDteq>();
        public ObservableCollection<DteqModel> DteqList { get; set; } = new ObservableCollection<DteqModel>();
        public ObservableCollection<LoadModel> LoadList { get; set; } = new ObservableCollection<LoadModel>();
        public ObservableCollection<PowerCableModel> CableList { get; set; } = new ObservableCollection<PowerCableModel>();
        public ObservableCollection<CircuitComponentModel> CompList { get; set; } = new ObservableCollection<CircuitComponentModel>();


        public ObservableCollection<IEquipment> eqList { get; set; } = new ObservableCollection<IEquipment>();
        public ObservableCollection<IPowerConsumer> masterLoadList { get; set; } = new ObservableCollection<IPowerConsumer>();

        public Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();
        public Dictionary<string, IEquipment> eqDict { get; set; } = new Dictionary<string, IEquipment>();
        public Dictionary<string, IPowerConsumer> iLoadDict { get; set; } = new Dictionary<string, IPowerConsumer>();


        public void CreateIDteqList()
        {
            foreach (var item in DteqList) {
                IDteqList.Add(item);
            }
        }
        public ObservableCollection<DteqModel> GetDteq() {
            DteqList = DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqListTable);
            CreateDteqDict();
            return DteqList;
        }
        public ObservableCollection<LoadModel> GetLoads() {     
            LoadList = DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadListTable);
            CreateILoadDict();
            return LoadList;
        }



        public ObservableCollection<PowerCableModel> GetCableList()
        {
            return DbManager.prjDb.GetRecords<PowerCableModel>("Cables");
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
        public bool IsTagAvailable(string tag) {
            var tagCheck = masterLoadList.Where(e => e.Tag == tag );
                if (tagCheck == null) {
                    return true;
                }
            return false;
        }


        #region MajorEquipment
        //Todo - Pass List Back or add to both lists
        public void CalculateDteqLoading() // LoadList Manager
        {
            AssignLoadsToDteq();
            //Calculates the loading of each load. Recursive for dteq
            foreach (DteqModel dteq in DteqList) {
                dteq.CalculateLoading();
            }
        }
        public void OnFedFromCalculated(object sender, EventArgs e)
        {
            UnassignLoads();
            AssignLoadsToDteq();
        }

        private void UnassignLoads()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.FedFromChanged -= this.OnFedFromCalculated;

                //Adds Dteq as Loads
                foreach (DteqModel dteqAsLoad in DteqList) {
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteqAsLoad.LoadingCalculated -= dteq.OnDteqLoadingCalculated;
                        dteqAsLoad.LoadingCalculated -= DbManager.OnDteqLoadingCalculated;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in LoadList) {
                    load.FedFromChanged -= this.OnFedFromCalculated;
                    if (load.FedFrom == dteq.Tag) {
                        load.LoadingCalculated -= dteq.OnDteqLoadingCalculated;
                        load.LoadingCalculated -= DbManager.OnLoadLoadingCalculated;
                    }
                }
            }
        }

        public void AssignLoadsToDteq()
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;
                dteq.FedFromChanged += this.OnFedFromCalculated;

                //Adds Dteq as Loads
                foreach (DteqModel dteqAsLoad in DteqList) {
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;

                        dteqAsLoad.LoadingCalculated += dteq.OnDteqLoadingCalculated;
                        dteqAsLoad.LoadingCalculated += DbManager.OnDteqLoadingCalculated;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in LoadList) {
                    load.FedFromChanged += this.OnFedFromCalculated;
                    if (load.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(load);
                        dteq.LoadCount += 1;

                        load.LoadingCalculated += dteq.OnDteqLoadingCalculated;
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
        public void CreateMasterLoadList() {
            masterLoadList.Clear();
            foreach (var dteq in DteqList) {
                masterLoadList.Add(dteq);
            }
            foreach (var load in LoadList) {
                masterLoadList.Add(load);
            }

            //CalculateSystemLoading();

            CreateEqDict();
            CreateDteqDict();
            CreateILoadDict();
        }
       
        public void CreateComponentList() {
            CompList.Clear();
            foreach (var load in LoadList) {
                load.SizeComponents();
                foreach (var comp in load.InLineComponents) {
                    CompList.Add(comp);
                }
            }
        }
 
        /// <summary>
        /// Creteas a list of CableModel for all equipment and components based on the masterLoadList
        /// </summary>
        /// 
        public void CreateCableList() {
            CableList.Clear();
           
        }

        public void CalculateCableAmps() {
            foreach (var cable in CableList) {
                cable.CalculateAmpacity();
            }
        }
        #endregion

    }
}
