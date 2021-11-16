using System;
using System.Linq;
using System.Collections.Generic;
using EDTLibrary.Models;

namespace EDTLibrary {
    public static partial class ListManager {
        public static List<IEquipmentModel> eqList = new List<IEquipmentModel>();
        public static Dictionary<string, IEquipmentModel> eqDict = new Dictionary<string, IEquipmentModel>();

        public static List<ILoadModel> masterLoadList = new List<ILoadModel>();
        public static Dictionary<string, DistributionEquipmentModel> dteqDict = new Dictionary<string, DistributionEquipmentModel>();

        public static List<DistributionEquipmentModel> dteqList = new List<DistributionEquipmentModel>();
        public static List<LoadModel> loadList = new List<LoadModel>();
        public static List<CableModel> cableList = new List<CableModel>();
        public static List<ComponentModel> compList = new List<ComponentModel>();


        public static void CreateEqDict() {
            foreach (var eq in masterLoadList) {
                eqDict.Add(eq.Tag, eq);
            }
            foreach (var comp in compList) {
                eqDict.Add(comp.Tag, comp);
            }
        }

        public static void CreateDteqDict() {
            dteqDict.Clear();
            foreach (var dteq in dteqList) {
                dteqDict.Add(dteq.Tag, dteq);
            }
        }

        public static bool CheckDuplicateTags(string Tag) {
            if (eqDict.ContainsKey(Tag)) {
                return true;
            }
            return false;
        }

        #region MajorEquipment
        public static void AssignLoadsToDteq() // LoadList Manager
        {
            foreach (DistributionEquipmentModel dteq in dteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;    

                //Adds Dteq as Loads
                foreach (DistributionEquipmentModel dteqAsLoad in dteqList) {
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in loadList) {
                    if (load.FedFrom == dteq.Tag) {
                        load.CalculateLoading();
                        dteq.AssignedLoads.Add(load);
                        dteq.LoadCount += 1;
                    }
                }
            }
            //Calculates the loading of each load. Recursive for dteq
            foreach (DistributionEquipmentModel dteq in dteqList) {
                dteq.CalculateLoading();
            }
        }
        //takes in List of LoadModel and Matches Tags to each MajorEquipment in an dteqList
        public static void AssignLoadsToDteq(List<DistributionEquipmentModel> dteqList, List<LoadModel> loadList) // LoadList Manager
        {
            foreach (DistributionEquipmentModel dteq in dteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;                
                foreach (DistributionEquipmentModel dteqAsLoad in dteqList) {
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;
                    }
                }

                foreach (LoadModel load in loadList) {
                    if (load.FedFrom == dteq.Tag) {
                        load.CalculateLoading();
                        dteq.AssignedLoads.Add(load);
                        dteq.LoadCount += 1;
                    }
                }
            }
            foreach (DistributionEquipmentModel dteq in dteqList) {
                dteq.CalculateLoading();
            }
        }
        public static List<string> GetDteqTags(List<DistributionEquipmentModel> list) {
            List<string> output = new List<string>();
            foreach (DistributionEquipmentModel item in list) {
                output.Add(item.Tag.ToString());
            }
            return output;
        }
        #endregion

        //Loads
        #region Loads
        public static void CalculateLoads() {
            foreach (var load in loadList) {
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
            foreach (var dteq in dteqList) {
                masterLoadList.Add(dteq);
            }
            foreach (var load in loadList) {
                masterLoadList.Add(load);
            }
        }
        public static List<ILoadModel> CreateMasterLoadList(List<DistributionEquipmentModel> dteqList, List<LoadModel> loadList) {
            List<ILoadModel> masterLoadList = new List<ILoadModel>();
            masterLoadList.Clear();
            foreach (var dteq in dteqList) {
                masterLoadList.Add(dteq);
            }
            foreach (var load in loadList) {
                masterLoadList.Add(load);
            }
            return masterLoadList;
        }
        /// <summary>
        /// Creates a list of ComponentModel
        /// </summary>
        public static void CreateComponentList() {
            compList.Clear();
            foreach (var load in loadList) {
                load.SizeComponents();
                compList.AddRange(load.InLineComponents);
            }
        }
        public static List<ComponentModel> CreateComponentList(List<LoadModel> loadList) {
            List<ComponentModel> compList = new List<ComponentModel>();
            compList.Clear();
            foreach (var load in loadList) {
                load.SizeComponents();
                compList.AddRange(load.InLineComponents);
            }
            return compList;
        }
        /// <summary>
        /// Creteas a list of CableModel for all equipment and components based on the masterLoadList
        /// </summary>
        public static void CreateCableList() {
            CreateMasterLoadList();
            cableList.Clear();
            foreach (var load in masterLoadList) {
                //No Components
                if (load.InLineComponents.Count == 0) {
                    cableList.Add(new CableModel { Conn1 = load.FedFrom, Conn2 = load.Tag });
                }
                //Has Components
                else {
                    for (int i = 0; i < load.InLineComponents.Count; i++) {
                        //First cable = FedFrom to component[0]
                        if (i == 0) {
                            cableList.Add(new CableModel { Conn1 = load.FedFrom, Conn2 = load.InLineComponents[i].Tag });
                        }
                        //Last cable = component[n] to Load
                        else if (i == load.InLineComponents.Count - 1) {
                            cableList.Add(new CableModel { Conn1 = load.InLineComponents[i].Tag, Conn2 = load.Tag });
                        }
                        else {
                            cableList.Add(new CableModel { Conn1 = load.InLineComponents[i - 1].Tag, Conn2 = load.InLineComponents[i].Tag });
                        }
                    }
                }
            }
            foreach (var cable in cableList) {
                cable.Tag = $"{cable.Conn1}-{cable.Conn2}";
            }
        }

        public static List<CableModel> CreateCableList(List<ILoadModel> masterLoadList) {
            List<CableModel> cableList = new List<CableModel>();
            List<string> trays = new List<string>();

            foreach (var load in masterLoadList) {
                //No Components
                if (load.InLineComponents.Count == 0) {
                    cableList.Add(new CableModel { Conn1 = load.FedFrom, Conn2 = load.Tag });
                }
                //Has Components
                else {
                    for (int i = 0; i < load.InLineComponents.Count; i++) {
                        //First cable = FedFrom to component[0]
                        if (i == 0) {
                            cableList.Add(new CableModel { Conn1 = load.FedFrom, Conn2 = load.InLineComponents[i].Tag });
                        }
                        //Last cable = component[n] to Load
                        else if (i == load.InLineComponents.Count - 1) {
                            cableList.Add(new CableModel { Conn1 = load.InLineComponents[i].Tag, Conn2 = load.Tag });
                        }
                        else {
                            cableList.Add(new CableModel { Conn1 = load.InLineComponents[i - 1].Tag, Conn2 = load.InLineComponents[i].Tag });
                        }
                    }
                }
            }
            return cableList;
        }
        #endregion
        
    }
}
