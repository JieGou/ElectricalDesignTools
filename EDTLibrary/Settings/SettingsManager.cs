using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Mappers;
using EDTLibrary.Models.Cables;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Validators;
using EDTLibrary.Validators.Cable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace EDTLibrary.Settings
{
    public class SettingsManager {

        public EdtProjectSettings EdtSettings { get; set; }
        public SettingsManager(EdtProjectSettings edtSettings)
        {
            EdtSettings = edtSettings;
        }

        
        public static ObservableCollection<SettingModel> SettingList { get; set; } = new ObservableCollection<SettingModel>();
        public static ObservableCollection<SettingModel> StringSettingList { get; set; } = new ObservableCollection<SettingModel>();
        public static ObservableCollection<SettingModel> TableSettingList { get; set; } = new ObservableCollection<SettingModel>();


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


            // Load from Db into Objects
            // -Strings
            DataTable settingsDbTable = DaManager.prjDb.GetDataTable("ProjectSettings");
            Type projectSettingsClass = typeof(EdtProjectSettings);
            string propValue = "";

            foreach (var setting in SettingList) {
                foreach (var property in projectSettingsClass.GetProperties()) {
                    if (setting.Name == property.Name && setting.Type != "DataTable") {

                        try {
                            property.SetValue(propValue, setting.Value);

                        }
                        catch (Exception ex) {
                            property.SetValue(propValue, setting);
                            //ex.Data.Add("UserMessage", "Error loading project setting from project database file.");
                        }
                    }
                    if (setting.Name + "_Model" == property.Name) {

                        try {
                            property.SetValue(propValue, setting);

                        }
                        catch (Exception ex) {
                            ex.Data.Add("UserMessage", "Error loading project setting from project database file.");
                        }
                    }
                }
            }

            // -Tables
            foreach (var setting in SettingList) {
                foreach (var prop in projectSettingsClass.GetProperties()) {
                    if (setting.Name == prop.Name
                        && setting.Type == "DataTable"
                        && listOfTablesInDb.Contains(prop.Name))
                    {
                        prop.SetValue(propValue, DaManager.prjDb.GetDataTable(prop.Name));
                    }
                }
            }
        
            AssignCodeSettings();
            GetCableSizesUsedInProject();
            GetExportMappings();
        }

        private static void GetCableSizesUsedInProject()
        {
            EdtProjectSettings.CableSizesUsedInProject = DaManager.prjDb.GetRecords<CableSizeModel>("CableSizesUsedInProject");
        }
        private static void GetExportMappings()
        {
            EdtProjectSettings.ExportMappings = DaManager.prjDb.GetRecords<ExportMappingModel>("ExportMapping");
        }

        private static void AssignCodeSettings()
        {
            if (EdtProjectSettings.Code == "CEC") {
                CableManager.CableSizer = new CecCableSizer();
                CableValidator.CableLengthValidator = new CecCableLengthValidator();
            }
        }

        public static void SaveStringSetting(SettingModel setting)
        {
            //SettingsModel
            DaManager.prjDb.UpdateRecord<SettingModel>(setting, "ProjectSettings");

            //SettingProperty
            Type edtSettingsClass = typeof(EdtProjectSettings); // MyClass is static class with static properties
            foreach (var prop in edtSettingsClass.GetProperties()) {
                if (setting.Name == prop.Name) {
                    prop.SetValue(setting.Value, setting.Value);
                }
            }
        }

        public static void SaveSettingToDb(string settingName, string settingValue)
        {
            SettingModel setting = new SettingModel();
            setting = SettingList.FirstOrDefault(s => s.Name == settingName);
            setting.Value = settingValue;
            DaManager.prjDb.UpdateRecord<SettingModel>(setting, "ProjectSettings");
        }


        public static void SaveSettingTableValueToDb(SettingModel tableSetting)
        {
            DaManager.prjDb.SaveDataTable(tableSetting.TableValue, tableSetting.Name);
        }

        public static void SaveExportMappingToDb(ExportMappingModel mapping)
        {
            DaManager.prjDb.UpdateRecord(mapping, GlobalConfig.ExportMappingTable);
        }
    }
}
