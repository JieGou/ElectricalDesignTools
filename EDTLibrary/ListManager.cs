﻿using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary
{
    [AddINotifyPropertyChangedInterface]
    public class ListManager {

        public ObservableCollection<LocationModel> LocationList { get; set; } = new ObservableCollection<LocationModel>();


        public static ObservableCollection<DteqModel> DteqList { get; set; } = new ObservableCollection<DteqModel>();
        public static ObservableCollection<DteqModel> OcDteq { get; set; } = new ObservableCollection<DteqModel>();

        public static ObservableCollection<LoadModel> LoadList { get; set; } = new ObservableCollection<LoadModel>();
        public static ObservableCollection<LoadModel> OcLoads { get; set; } = new ObservableCollection<LoadModel>();

        public static ObservableCollection<PowerCableModel> CableList { get; set; } = new ObservableCollection<PowerCableModel>();
        public static ObservableCollection<PowerCableModel> OcCables { get; set; } = new ObservableCollection<PowerCableModel>();



        public static ObservableCollection<CircuitComponentModel> CompList { get; set; } = new ObservableCollection<CircuitComponentModel>();


        public static ObservableCollection<IEquipment> eqList { get; set; } = new ObservableCollection<IEquipment>();
        public static ObservableCollection<IPowerConsumer> masterLoadList { get; set; } = new ObservableCollection<IPowerConsumer>();

        public static Dictionary<string, DteqModel> dteqDict { get; set; } = new Dictionary<string, DteqModel>();
        public static Dictionary<string, IEquipment> eqDict { get; set; } = new Dictionary<string, IEquipment>();
        public static Dictionary<string, IPowerConsumer> iLoadDict { get; set; } = new Dictionary<string, IPowerConsumer>();



        public static ObservableCollection<DteqModel> GetDteq() {
            DteqList = DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqListTable);
            CreateDteqDict();
            OcDteq = new ObservableCollection<DteqModel>(DteqList);
            //CalculateDteqLoading();
            return OcDteq;
        }

        public static ObservableCollection<LoadModel> GetLoads() {     
            LoadList = DbManager.prjDb.GetRecords<LoadModel>(GlobalConfig.LoadListTable);
            CreateILoadDict();
            OcLoads = new ObservableCollection<LoadModel>(LoadList);
            //CalculateDteqLoading();
            return OcLoads;
        }

        public static ObservableCollection<PowerCableModel> GetCableList()
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
        //Todo - Pass List Back or add to both lists
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

            DteqList = new ObservableCollection<DteqModel>(dteqList);

            LoadList = new ObservableCollection<LoadModel>(loadList);
        }

        #endregion

        //Loads
        #region Loads
        public static void CalculateLoads() {
            foreach (var load in LoadList) {
                load.CalculateLoading();
            }
        }
        public static void CalculateLoads(ObservableCollection<LoadModel> loadList) {
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
