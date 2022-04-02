using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Cables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace EDTLibrary.ProjectSettings
{
    public class SettingManager {

        public static ObservableCollection<SettingModel> SettingList { get; set; } = new ObservableCollection<SettingModel>();
        public static ObservableCollection<SettingModel> StringSettingList { get; set; } = new ObservableCollection<SettingModel>();
        public static ObservableCollection<SettingModel> TableSettingList { get; set; } = new ObservableCollection<SettingModel>();

        public static Dictionary<string, SettingModel> SettingDict { get; set; } = new Dictionary<string, SettingModel>();
        public static Dictionary<string, SettingModel> StringSettingDict { get; set; } = new Dictionary<string, SettingModel>();
        public static Dictionary<string, SettingModel> TableSettingDict { get; set; } = new Dictionary<string, SettingModel>();

        
        public static void LoadProjectSettings()
        {
            SettingList.Clear();
            SettingList = DaManager.prjDb.GetRecords<SettingModel>("ProjectSettings");

            // LISTS
            // -Strings
            StringSettingList.Clear();
            foreach (var setting in SettingList) {
                if (setting.Type != "DataTable") {
                    StringSettingList.Add(setting);
                }
            }

            // -Tables
            ArrayList listOfTablesInDb = DaManager.prjDb.GetListOfTablesNamesInDb();
            TableSettingList.Clear();
            foreach (var setting in SettingList) {
                if (setting.Type == "DataTable") {
                    //if in Db get DataTable
                    if (listOfTablesInDb.Contains(setting.Name)) {
                        setting.TableValue = DaManager.prjDb.GetDataTable(setting.Name);
                    }
                    TableSettingList.Add(setting);
                }
            }


            // PROPERTIES
            // -Strings
            DataTable settingsDbTable = DaManager.prjDb.GetDataTable("ProjectSettings");
            Type projectSettingsClass = typeof(EdtSettings);
            string propValue = "";

            for (int i = 0; i < settingsDbTable.Rows.Count; i++) {
                foreach (var property in projectSettingsClass.GetProperties()) {
                    if (settingsDbTable.Rows[i]["Name"].ToString() == property.Name && settingsDbTable.Rows[i]["Type"].ToString() != "DataTable") {
                        property.SetValue(propValue, settingsDbTable.Rows[i]["Value"].ToString());
                    }
                }
            }

            // -Tables
            for (int i = 0; i < settingsDbTable.Rows.Count; i++) {
                foreach (var prop in projectSettingsClass.GetProperties()) {

                    if (settingsDbTable.Rows[i]["Name"].ToString() == prop.Name
                        && settingsDbTable.Rows[i]["Type"].ToString() == "DataTable"
                        && listOfTablesInDb.Contains(prop.Name))
                    {
                        prop.SetValue(propValue, DaManager.prjDb.GetDataTable(prop.Name));
                    }
                }
            }
        
            SettingDict.Clear();
            SettingDict = SettingList.ToDictionary(x => x.Name);

            //CreateCableAmpsUsedInProject();
            //var test = EdtSettings.CableAmpsUsedInProject_3C1kV;

            AssignCodeSettings();
            GetCableSizesUsedInProject();
        }

        private static void GetCableSizesUsedInProject()
        {
            EdtSettings.CableSizesUsedInProject = DaManager.prjDb.GetRecords<CableSizeModel>("CableSizesUsedInProject");
        }

        private static void AssignCodeSettings()
        {
            if (EdtSettings.Code == "CEC") {
                CableSizeManager.CableSizer = new CecCableSizer();
            }
        }

        public static void SaveStringSetting(SettingModel setting)
        {
            //SettingsModel
            DaManager.prjDb.UpdateRecord<SettingModel>(setting, "ProjectSettings");

            //SettingProperty
            Type type = typeof(EdtSettings); // MyClass is static class with static properties
            foreach (var prop in type.GetProperties()) {
                if (setting.Name == prop.Name) {
                    prop.SetValue(setting.Value, setting.Value);
                }
            }
        }

        public static void SaveTableSetting(SettingModel tableSetting)
        {
            DaManager.prjDb.SaveDataTable(tableSetting.TableValue, tableSetting.Name);
        }

    }
}
