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

        public static List<CableModel> CableList { get; set; } = new List<CableModel>();
        public static ObservableCollection<CableModel> OcCables { get; set; } = new ObservableCollection<CableModel>();



        public static List<ComponentModel> compList { get; set; } = new List<ComponentModel>();


        public static List<IEquipmentModel> eqList { get; set; } = new List<IEquipmentModel>();
        public static List<ILoadModel> masterLoadList { get; set; } = new List<ILoadModel>();

        public static Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();
        public static Dictionary<string, IEquipmentModel> eqDict { get; set; } = new Dictionary<string, IEquipmentModel>();
        public static Dictionary<string, ILoadModel> iLoadDict { get; set; } = new Dictionary<string, ILoadModel>();



        public static ObservableCollection<DteqModel> GetDteq() {
            DteqList = DbManager.prjDb.GetRecords<DteqModel>("DistributionEquipment");
            CreateDteqDict();
            OcDteq = new ObservableCollection<DteqModel>(DteqList);
            return OcDteq;
        }

        public static ObservableCollection<LoadModel> GetLoads() {     
            LoadList = DbManager.prjDb.GetRecords<LoadModel>("Loads");
            CreateILoadDict();
            OcLoads = new ObservableCollection<LoadModel>(LoadList);
            return OcLoads;
        }

        public static List<CableModel> GetCableList()
        {
            return DbManager.prjDb.GetRecords<CableModel>("Cables");
        }



        public static void CreateEqDict() {
            eqDict.Clear();
            foreach (var eq in masterLoadList) {
                eqDict.Add(eq.Tag, eq);
            }
            foreach (var comp in compList) {
                eqDict.Add(comp.Tag, comp);
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
                dteqDict.Add(dteq.Tag, dteq);
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
        public static void CalculateSystemLoading() // LoadList Manager
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

        public static void CalculateSystemLoading(ObservableCollection<DteqModel> dteqList, ObservableCollection<LoadModel> loadList) // LoadList Manager
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


        public static List<string> GetDteqTags(List<DteqModel> list) {
            List<string> output = new List<string>();
            foreach (DteqModel item in list) {
                output.Add(item.Tag.ToString());
            }
            return output;
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
            compList.Clear();
            foreach (var load in LoadList) {
                load.SizeComponents();
                foreach (var comp in load.InLineComponents) {
                    compList.Add(comp);
                }
            }
        }
 
        /// <summary>
        /// Creteas a list of CableModel for all equipment and components based on the masterLoadList
        /// </summary>
        /// 
        public static void CreateCableList() {
            CreateMasterLoadList();
            CableList.Clear();
            foreach (var load in masterLoadList) {
                //No Components
                if (load.InLineComponents.Count == 0) {
                    CableList.Add(new CableModel { Source = load.FedFrom, Destination = load.Tag });
                }

                //Inline Components
                //TODO - Change this to read from Component list directly with component count                 
                else {
                    for (int i = 0; i < load.InLineComponents.Count; i++) {
                        //First cable = FedFrom to component[0]
                        if (i == 0) {
                            CableList.Add(new CableModel { Source = load.FedFrom, Destination = load.InLineComponents[i].Tag });
                        }
                        //Last cable = component[n] to Load
                        else if (i == load.InLineComponents.Count - 1) {
                            CableList.Add(new CableModel { Source = load.InLineComponents[i].Tag, Destination = load.Tag });
                        }
                        else {
                            CableList.Add(new CableModel { Source = load.InLineComponents[i - 1].Tag, Destination = load.InLineComponents[i].Tag });
                        }
                    }
                }
            }
            foreach (var cable in CableList) {
                cable.CreateTag();
                cable.GetCableParametersOld();
                cable.CalculateCableQtySize();
            }
        }

        public static void CalculateCableAmps() {
            foreach (var cable in CableList) {
                cable.CalculateAmpacity();
            }
        }
        #endregion

    }
}
