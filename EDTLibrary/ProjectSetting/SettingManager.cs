using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EDTLibrary.ProjectSetting {
    public class SettingManager {


        //String Settings
        public static string GetStringSetting(string settingName) {
            string settingValue = "";
            Type type = typeof(StringSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    settingValue = prop.GetValue(null).ToString();
                }
            }
            return settingValue;
        }

        public static void SaveStringSetting(string settingName, string settingValue) {
            Type type = typeof(StringSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    prop.SetValue(settingValue, settingValue);
                }
            }

        }

        //Table Settings

        public static DataTable GetTableSetting(string settingName) {
            DataTable dt = new DataTable();
            Type type = typeof(StringSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (settingName == prop.Name) {
                    dt = (DataTable)prop.GetValue(null);
                }
            }
            return dt;
        }
    }
}
