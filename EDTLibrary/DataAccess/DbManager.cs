using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.DataAccess {
    public class DbManager {

        public static SQLiteConnector prjDb { get; set; }
        public static SQLiteConnector libDb { get; set; }

        public static void SetProjectDb(string conString) {
            prjDb = new SQLiteConnector(conString);
        }

        public static void SetLibraryDb(string conString) {
            libDb = new SQLiteConnector(conString);
        }

    }
}
