using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EDTLibrary.ProjectSettings {
    public class SettingManager {

        public static List<SettingModel> SettingList = new List<SettingModel>();
        public static List<SettingModel> StringSettingList = new List<SettingModel>();
        public static List<SettingModel> TableSettingList = new List<SettingModel>();

        public static Dictionary<string,SettingModel> StringSettingDict = new Dictionary<string, SettingModel>();

        public static void GetSettings()
        {
            SettingList.Clear();
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");
            foreach (var setting in SettingList) {

               // String Settings
                if (setting.Type != "DataTable") {
                    StringSettingList.Add(setting);
                }

                // Table Settings
                else if (setting.Type == "DataTable") { 
                    setting.TableValue = DbManager.prjDb.GetDataTable(setting.Name);
                    TableSettingList.Add(setting);
                }

            }
            StringSettingDict.Clear();
            StringSettingDict = StringSettingList.ToDictionary(x => x.Name);
        }

        public static List<SettingModel> GetStringSettings()
        {
            List<SettingModel> settingList = new List<SettingModel>();
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");
            foreach (var setting in SettingList) {

                // String Settings
                if (setting.Type != "DataTable") {
                    settingList.Add(setting);
                }

            }
            StringSettingDict = settingList.ToDictionary(x => x.Name);
            return settingList;
        }

        public static List<SettingModel> GetTableSettings()
        {
            List<SettingModel> settingList = new List<SettingModel>();
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");
            foreach (var setting in SettingList) {

                // String Settings
                if (setting.Type == "DataTable") {
                    setting.TableValue = DbManager.prjDb.GetDataTable(setting.Name);
                    settingList.Add(setting);
                }

            }            
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

        // OLD WAY
        // String Settings
        public static string GetStringSetting(string settingName) {
            string settingValue = "";
            Type type = typeof(Settings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    settingValue = prop.GetValue(null).ToString();
                }
            }
            return settingValue;
        }

        public static void SaveStringSetting(string settingName, string settingValue) {
            Type type = typeof(Settings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    prop.SetValue(settingValue, settingValue);
                }
            }
        }

        //Table Settings

        public static DataTable GetTableSetting(string settingName) {
            DataTable dt = new DataTable();
            Type type = typeof(Settings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    dt = (DataTable)prop.GetValue(null);
                }
            }
            return dt;
        }
    }
}
