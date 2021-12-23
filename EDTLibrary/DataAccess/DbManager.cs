using EDTLibrary.TypeTables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EDTLibrary.DataAccess {
    public class DbManager {


        private static string _projectFile;
        private static string _libraryFile;

        public static bool IsProjectLoaded { get; private set; }
        public static bool IsLibraryLoaded { get; private set; }


        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }

        public static void SetProjectDb(string conString) {
            prjDb = new SQLiteConnector(conString);
            _projectFile = conString;
        }

        public static void SetLibraryDb(string conString) {
            libDb = new SQLiteConnector(conString);
            _libraryFile = conString;
        }


        public static bool LoadLibraryTables()
        {
            Type libTablesClass = typeof(LibraryTables); // MyClass is static class with static properties
            DataTable dt = new DataTable();
            foreach (var prop in libTablesClass.GetProperties()) {
                prop.SetValue(dt, libDb.GetDataTable(prop.Name));
            }
            IsLibraryLoaded = true;
            return IsLibraryLoaded;
        }

        #region Type Tables

        public static void LoadTypeTables()
        {
            LoadCableTypes();
            LoadNemaTypes();
            LoadVoltageTypes();
        }


        private static void LoadVoltageTypes()
        {
            libDb.GetRecords<VoltageType>("VoltageTypes");
        }

        private static void LoadCableTypes()
        {
            libDb.GetRecords<CableTypes>("CableTypes");
        }

        private static void LoadNemaTypes()
        {
            libDb.GetRecords<NemaTypes>("NemaTypes");
        }

            #endregion
    }
}
