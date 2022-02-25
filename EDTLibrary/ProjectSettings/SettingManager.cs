using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
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
            var test = EdtSettings.CableAmpsUsedInProject_3C1kV;
        }


        

        public static void SaveStringSetting(SettingModel setting)
        {
            //SettingsModel
            DbManager.prjDb.UpdateRecord<SettingModel>(setting, "ProjectSettings");

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
            DbManager.prjDb.SaveDataTable(tableSetting.TableValue, tableSetting.Name);
        }



        #region Setting Initializations
        public static void CreateCableAmpsUsedInProject()
        {
            DataTable ampsUsedInProjectTable = new DataTable();
            DataTable dtCableSizesUsedInProject = new DataTable();

            if (LibraryTables.CableAmpacities != null) {

                var settingName = nameof(EdtSettings.CableAmpsUsedInProject_3C1kV);
                SettingDict[nameof(EdtSettings.CableAmpsUsedInProject_3C1kV)].TableValue = LibraryTables.CableAmpacities.Copy();
                ampsUsedInProjectTable = SettingDict[nameof(EdtSettings.CableAmpsUsedInProject_3C1kV)].TableValue;

                settingName = nameof(EdtSettings.CableSizesUsedInProject_3C1kV);
                dtCableSizesUsedInProject = SettingDict[nameof(EdtSettings.CableSizesUsedInProject_3C1kV)].TableValue;

                string size;
                DataRow cable;
                foreach (DataRow cableInProject in dtCableSizesUsedInProject.Rows) {
                    if (cableInProject.Field<bool>("UsedInProject") == false) {
                        size = cableInProject.Field<string>("Size");

                        for (int i = ampsUsedInProjectTable.Rows.Count - 1; i >= 0; i--) {
                            cable = ampsUsedInProjectTable.Rows[i];
                            if (cable["Size"].ToString() == size) {
                                ampsUsedInProjectTable.Rows.Remove(cable);
                            }
                        }
                    }
                }
                ampsUsedInProjectTable.AcceptChanges();
                EdtSettings.CableAmpsUsedInProject_3C1kV = ampsUsedInProjectTable;
            }
        }

        public static void CreateCableAmpsUsedInProject(DataTable cableAmpsTable, DataTable cableSizeTable)
        {
            DataTable ampsUsedInProjectTable = new DataTable();
            DataTable dtCables = new DataTable();

            if (LibraryTables.CableAmpacities != null) {

                //amps
                var settingName = nameof(cableAmpsTable);
                SettingDict[nameof(cableAmpsTable)].TableValue = LibraryTables.CableAmpacities.Copy();
                ampsUsedInProjectTable = SettingDict[nameof(cableAmpsTable)].TableValue;

                //size
                settingName = nameof(cableSizeTable);
                dtCables = SettingDict[nameof(cableSizeTable)].TableValue;

                string size;
                foreach (DataRow cablePrj in dtCables.Rows) {
                    if (cablePrj.Field<bool>("UsedInProject") == false) {
                        size = cablePrj.Field<string>("Size");

                        for (int i = ampsUsedInProjectTable.Rows.Count - 1; i >= 0; i--) {
                            DataRow cable = ampsUsedInProjectTable.Rows[i];
                            if (cable["Size"].ToString() == size) {
                                ampsUsedInProjectTable.Rows.Remove(cable);
                            }
                        }
                    }
                }
                ampsUsedInProjectTable.AcceptChanges();
                //EdtSettings.CableAmpsUsedInProject_3C1kV = ampsUsedInProjectTable;
            }
        }
        #endregion

    }
}
