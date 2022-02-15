using EDTLibrary.LibraryData;
using EDTLibrary.TypeTables;
using System;
using System.Data;

namespace EDTLibrary.DataAccess
{
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


        public static bool GetLibraryTables()
        {
            Type libTablesClass = typeof(LibraryTables); // MyClass is static class with static properties
            DataTable dt = new DataTable();
            foreach (var prop in libTablesClass.GetProperties()) {
                prop.SetValue(dt, libDb.GetDataTable(prop.Name));
            }
            IsLibraryLoaded = true;
            GetTypeTables();
            return IsLibraryLoaded;
        }

        #region Type Tables

        public static void GetTypeTables()
        {
            GetCableTypes();
            GetNemaTypes();
            GetVoltageTypes();
            GetAreaClassificationTypes();
        }


        private static void GetVoltageTypes()
        {
            TypeManager.VoltageTypes = libDb.GetRecords<VoltageType>("VoltageTypes");
        }

        private static void GetCableTypes()
        {
            TypeManager.CableTypes = libDb.GetRecords<CableType>("CableTypes");
        }

        private static void GetNemaTypes()
        {
            TypeManager.NemaTypes = libDb.GetRecords<NemaType>("NemaTypes");
        }

        private static void GetAreaClassificationTypes()
        {
            TypeManager.AreaClassifications = libDb.GetRecords<AreaClassificationType>("AreaClassifications");
        }
            #endregion
    }
}
