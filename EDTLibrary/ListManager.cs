using System;
using System.Linq;
using System.Collections.Generic;
using EDTLibrary.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EDTLibrary.DataAccess;

namespace EDTLibrary {
    public class ListManager {

        public static List<DteqModel> DteqList { get; set; } = new List<DteqModel>();
        public static ObservableCollection<DteqModel> OcDteq { get; set; } = new ObservableCollection<DteqModel>();

        public static List<LoadModel> LoadList { get; set; } = new List<LoadModel>();
        public static ObservableCollection<LoadModel> OcLoads { get; set; } = new ObservableCollection<LoadModel>();

        public static List<PowerCableModel> CableList { get; set; } = new List<PowerCableModel>();
        public static ObservableCollection<PowerCableModel> OcCables { get; set; } = new ObservableCollection<PowerCableModel>();



        public static List<CircuitComponentModel> CompList { get; set; } = new List<CircuitComponentModel>();


        public static List<IEquipment> eqList { get; set; } = new List<IEquipment>();
        public static List<IPowerConsumer> masterLoadList { get; set; } = new List<IPowerConsumer>();

        public static Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();
        public static Dictionary<string, IEquipment> eqDict { get; set; } = new Dictionary<string, IEquipment>();
        public static Dictionary<string, IPowerConsumer> iLoadDict { get; set; } = new Dictionary<string, IPowerConsumer>();



        public static ObservableCollection<DteqModel> GetDteq() {
            DteqList = DbManager.prjDb.GetRecords<DteqModel>("DistributionEquipment");
            CreateDteqDict();
            OcDteq = new ObservableCollection<DteqModel>(DteqList);
            //CalculateDteqLoading();
            return OcDteq;
        }

        public static ObservableCollection<LoadModel> GetLoads() {     
            LoadList = DbManager.prjDb.GetRecords<LoadModel>("Loads");
            CreateILoadDict();
            OcLoads = new ObservableCollection<LoadModel>(LoadList);
            //CalculateDteqLoading();
            return OcLoads;
        }

        public static List<PowerCableModel> GetCableList()
        {
            return DbManager.prjDb.GetRecords<PowerCableModel>("Cables");
        }



        public static void CreateEqDict() {
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
        public static void CreateILoadDict() {
            iLoadDict.Clear();
            foreach (var eq in DteqList) {
                iLoadDict.Add(eq.Tag, eq);
            }
            foreach (var eq in LoadList) {
                iLoadDict.Add(eq.Tag, eq);
            }
        }
        public static void CreateDteqDict() {
            dteqDict.Clear();
            foreach (var dteq in DteqList) {
                dteqDict.Add(dteq.Tag, dteq);
            }
        }
        public static void CreateDteqDict(ObservableCollection<DteqModel> dteqList)
        {
            dteqDict.Clear();
            foreach (var dteq in dteqList) {
                if (dteqDict.ContainsKey(dteq.Tag) ==false ) {
                    dteqDict.Add(dteq.Tag, dteq);
                }
            }
        }
        public static bool IsTagAvailable(string tag) {
            var tagCheck = masterLoadList.Where(e => e.Tag == tag );
                if (tagCheck == null) {
                    return true;
                }
            return false;
        }


        #region MajorEquipment
        //Toco - Pass List Back or add to both lists
        public static void CalculateDteqLoading() // LoadList Manager
        {
            foreach (DteqModel dteq in DteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;    

                //Adds Dteq as Loads
                foreach (DteqModel dteqAsLoad in DteqList) {
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in LoadList) {
                    if (load.FedFrom == dteq.Tag) {
                        load.CalculateLoading();
                        dteq.AssignedLoads.Add(load);
                        dteq.LoadCount += 1;
                    }
                }
            }
            //Calculates the loading of each load. Recursive for dteq
            foreach (DteqModel dteq in DteqList) {
                dteq.CalculateLoading();
            }
        }

        public static void CalculateDteqLoading(ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList) // LoadList Manager
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

            DteqList = new List<DteqModel>(dteqList);

            LoadList = new List<LoadModel>(loadList);
        }

        #endregion

        //Loads
        #region Loads
        public static void CalculateLoads() {
            foreach (var load in LoadList) {
                load.CalculateLoading();
            }
        }
        public static void CalculateLoads(List<LoadModel> loadList) {
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
        public static void CreateMasterLoadList() {
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
       
        public static void CreateComponentList() {
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
        public static void CreateCableList() {
            CableList.Clear();
           
        }

        public static void CalculateCableAmps() {
            foreach (var cable in CableList) {
                cable.CalculateAmpacity();
            }
        }
        #endregion

    }
}
