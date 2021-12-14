using System;
using System.Linq;
using System.Collections.Generic;
using EDTLibrary.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EDTLibrary.DataAccess;

namespace EDTLibrary {
    public class ListManager {

        public static List<DteqModel> dteqList { get; set; } = new List<DteqModel>();
        public static List<LoadModel> list { get; set; } = new List<LoadModel>();
        public static List<CableModel> cableList { get; set; } = new List<CableModel>();
        public static List<ComponentModel> compList { get; set; } = new List<ComponentModel>();
        public static List<LoadModel> loadListOC { get; set; } = new List<LoadModel>();
           
        public static List<IEquipmentModel> eqList { get; set; } = new List<IEquipmentModel>();
        public static List<ILoadModel> masterLoadList { get; set; } = new List<ILoadModel>();
        public static List<string> tagList { get; set; } = new List<string>();

        public static Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();
        public static Dictionary<string, IEquipmentModel> eqDict { get; set; } = new Dictionary<string, IEquipmentModel>();
        public static Dictionary<string, ILoadModel> iLoadDict { get; set; } = new Dictionary<string, ILoadModel>();



        public static List<DteqModel> GetDteq() {
            return DbManager.prjDb.GetRecords<DteqModel>("DistributionEquipment");
        }

        public static List<LoadModel> GetLoads() {     
            return DbManager.prjDb.GetRecords<LoadModel>("Loads");
        }

        public static List<CableModel> GetCableList()
        {
            return DbManager.prjDb.GetRecords<CableModel>("Cables");
        }



        #region WinFormCoreUI

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
            foreach (var eq in masterLoadList) {
                iLoadDict.Add(eq.Tag, eq);
            }
        }
        public static void CreateDteqDict() {
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
        public static void AssignLoadsToDteq() // LoadList Manager
        {
            foreach (DteqModel dteq in dteqList) {
                dteq.AssignedLoads.Clear();
                dteq.LoadCount = 0;    

                //Adds Dteq as Loads
                foreach (DteqModel dteqAsLoad in dteqList) {
                    if (dteqAsLoad.FedFrom == dteq.Tag) {
                        dteq.AssignedLoads.Add(dteqAsLoad);
                        dteq.LoadCount += 1;
                    }
                }
                //Adds loads as loads
                foreach (LoadModel load in list) {
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
            foreach (var load in list) {
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
            foreach (var load in loadListOC) {
                masterLoadList.Add(load);
            }
            AssignLoadsToDteq();
            CreateEqDict();
            CreateDteqDict();
            CreateILoadDict();
        }
       
        public static void CreateComponentList() {
            compList.Clear();
            foreach (var load in list) {
                load.SizeComponents();
                foreach (var comp in load.InLineComponents) {
                    compList.Add(comp);
                }
            }
        }
        //public static List<ComponentModel> CreateComponentList(List<LoadModel> loadList) {
        //    List<ComponentModel> compList = new List<ComponentModel>();
        //    compList.Clear();
        //    foreach (var load in loadList) {
        //        load.SizeComponents();
        //        compList.AddRange(load.InLineComponents);
        //    }
        //    return compList;
        //}
        /// <summary>
        /// Creteas a list of CableModel for all equipment and components based on the masterLoadList
        /// </summary>
        public static void CreateCableList() {
            CreateMasterLoadList();
            cableList.Clear();
            foreach (var load in masterLoadList) {
                //No Components
                if (load.InLineComponents.Count == 0) {
                    cableList.Add(new CableModel { Source = load.FedFrom, Destination = load.Tag });
                }

                //Inline Components
                //TODO - Change this to read from Component list directly with component count                 
                else {
                    for (int i = 0; i < load.InLineComponents.Count; i++) {
                        //First cable = FedFrom to component[0]
                        if (i == 0) {
                            cableList.Add(new CableModel { Source = load.FedFrom, Destination = load.InLineComponents[i].Tag });
                        }
                        //Last cable = component[n] to Load
                        else if (i == load.InLineComponents.Count - 1) {
                            cableList.Add(new CableModel { Source = load.InLineComponents[i].Tag, Destination = load.Tag });
                        }
                        else {
                            cableList.Add(new CableModel { Source = load.InLineComponents[i - 1].Tag, Destination = load.InLineComponents[i].Tag });
                        }
                    }
                }
            }
            foreach (var cable in cableList) {
                cable.CreateTag();
                cable.GetCableParameters();
                cable.CalculateQtySize();
            }
        }

        public static void CalculateCableAmps() {
            foreach (var cable in cableList) {
                cable.CalculateAmpacity();
            }
        }
        #endregion
        #endregion

    }
}
