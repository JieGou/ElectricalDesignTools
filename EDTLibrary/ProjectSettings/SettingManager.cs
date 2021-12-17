using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EDTLibrary.ProjectSettings {
    public class SettingManager {

        public static List<SettingModel> SettingList = new List<SettingModel>();
        public static Dictionary<string,SettingModel> SettingDict = new Dictionary<string, SettingModel>();
        public static void GetSettings()
        {
            SettingList = DbManager.prjDb.GetRecords<SettingModel>("ProjectSettings");
            SettingDict = SettingList.ToDictionary(x => x.Name);
        }
       
        //String Settings
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
