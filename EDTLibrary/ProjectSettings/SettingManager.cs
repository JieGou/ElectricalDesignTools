using EDTLibrary.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EDTLibrary.ProjectSettings {
    public class SettingManager {

        public static List<SettingModel> SettingList { get; set; } = new List<SettingModel>();
        public static List<SettingModel> StringSettingList { get; set; } = new List<SettingModel>();
        public static List<SettingModel> TableSettingList { get; set; } = new List<SettingModel>();

        public static Dictionary<string, SettingModel> SettingDict { get; set; } = new Dictionary<string, SettingModel>();
        public static Dictionary<string, SettingModel> StringSettingDict { get; set; } = new Dictionary<string, SettingModel>();
        public static Dictionary<string, SettingModel> TableSettingDict { get; set; } = new Dictionary<string, SettingModel>();

        public static void InitializeSettings()
        {
            SettingList.Clear();
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");

            // LISTS
            // -Strings
            StringSettingList.Clear();
            foreach (var setting in SettingList) {
                if (setting.Type != "DataTable") {
                    StringSettingList.Add(setting);

                }
            }

            // -Tables
            ArrayList tables = DbManager.prjDb.GetListOfTablesNamesInDb();
            TableSettingList.Clear();
            foreach (var setting in SettingList) {
                if (setting.Type == "DataTable") {
                    //if in Db get DataTable
                    if (tables.Contains(setting.Name)) {
                        setting.TableValue = DbManager.prjDb.GetDataTable(setting.Name);
                    }
                    TableSettingList.Add(setting);
                }
            }


            // PROPERTIES
            // -Strings
            DataTable settings = DbManager.prjDb.GetDataTable("ProjectSettings");
            Type prjSettings = typeof(EdtSettings);
            string propValue = "";

            for (int i = 0; i < settings.Rows.Count; i++) {
                foreach (var prop in prjSettings.GetProperties()) {
                    if (settings.Rows[i]["Name"].ToString() == prop.Name && settings.Rows[i]["Type"].ToString() != "DataTable") {
                        prop.SetValue(propValue, settings.Rows[i]["Value"].ToString());
                    }
                }
            }

            // -Tables
            for (int i = 0; i < settings.Rows.Count; i++) {
                foreach (var prop in prjSettings.GetProperties()) {

                    if (settings.Rows[i]["Name"].ToString() == prop.Name 
                        && settings.Rows[i]["Type"].ToString() == "DataTable"
                        && tables.Contains(prop.Name))
                    {
                        prop.SetValue(propValue, DbManager.prjDb.GetDataTable(prop.Name));
                    }
                }
            }
        
            SettingDict.Clear();
            SettingDict = SettingList.ToDictionary(x => x.Name);

            CreateCableAmpsUsedInProject();
        }


        public static List<SettingModel> GetStringSettings()
        {
            List<SettingModel> settingList = new List<SettingModel>();
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");

            foreach (var setting in SettingList) {
                if (setting.Type != "DataTable") {
                    settingList.Add(setting);
                }
            }
            return settingList;
        }

        /// <summary>
        /// Creates the list of TableSettings and populates its TableValue property if it exists in the Db
        /// </summary>
        /// <returns></returns>
        public static List<SettingModel> GetTableSettings()
        {

            List<SettingModel> settingList = new List<SettingModel>();
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");
            ArrayList tables = DbManager.prjDb.GetListOfTablesNamesInDb();
           
            foreach (var setting in SettingList) {
                if (setting.Type == "DataTable") {
                    //if in Db get DataTable
                    if (tables.Contains(setting.Name)) {
                        setting.TableValue = DbManager.prjDb.GetDataTable(setting.Name);
                    }
                    settingList.Add(setting);
                }

            }  
            
            var d = SettingDict;
            return settingList;
        }


        public static void SaveStringSetting(SettingModel setting)
        {
            DbManager.prjDb.UpdateRecord<SettingModel>(setting, "ProjectSettings");
        }

        public static void SaveTableSetting(SettingModel tableSetting)
        {
            DbManager.prjDb.SaveDataTable(tableSetting.TableValue, tableSetting.Name);
        }



        #region Setting Initializations
        public static void CreateCableAmpsUsedInProject()
        {
            DataTable dtAmps = new DataTable();
            DataTable dtCables = new DataTable();

            if (LibraryTables.CableAmpacities != null) {
            
                SettingDict["CableAmpsUsedInProject"].TableValue = LibraryTables.CableAmpacities.Copy();
                dtAmps = SettingDict["CableAmpsUsedInProject"].TableValue;

                dtCables = SettingDict["CableSizesUsedInProject3CLV"].TableValue;

                foreach (DataRow cablePrj in dtCables.Rows) {
                    if (cablePrj.Field<bool>("UsedInProject") == false) {
                        string size = cablePrj.Field<string>("Size");

                        for (int i = dtAmps.Rows.Count - 1; i >= 0; i--) {
                            DataRow cable = dtAmps.Rows[i];
                            if (cable["Size"].ToString() == size) {
                                dtAmps.Rows.Remove(cable);
                            }
                        }
                    }
                }
                dtAmps.AcceptChanges();
            }
        }

        #endregion














        // OLD WAY
        // String Settings
        public static string GetStringSettingOld(string settingName) {
            string settingValue = "";
            Type type = typeof(EdtSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    settingValue = prop.GetValue(null).ToString();
                }
            }
            return settingValue;
        }

        public static void SaveStringSettingOld(string settingName, string settingValue) {
            Type type = typeof(EdtSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    prop.SetValue(settingValue, settingValue);
                }
            }
        }

        //Table Settings

        public static DataTable GetTableSettingOld(string settingName) {
            DataTable dt = new DataTable();
            Type type = typeof(EdtSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    dt = (DataTable)prop.GetValue(null);
                }
            }
            return dt;
        }
    }
}
