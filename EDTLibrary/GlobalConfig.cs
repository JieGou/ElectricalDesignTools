using EDTLibrary.DataAccess;
using System.Collections.Generic;

namespace EDTLibrary {
    public static class GlobalConfig {

        public static List<IDataConnector> ProjectConnections { get; private set; } = new List<IDataConnector>();
        public static List<IDataConnector> LibraryConnections { get; private set; } = new List<IDataConnector>();

        public static void InitializeConnections(bool sqLite)
        {
            if (sqLite)
            {
                //SQLiteConnector prjDb = new SQLiteConnector(Properties.Settings.Default.ProjectDb);
                //ProjectConnections.Add(prjDb);
                //SQLiteConnector libDb = new SQLiteConnector(Properties.Settings.Default.ProjectDb);
                //LibraryConnections.Add(libDb);
            }

        }

        //Constants
        public const int SigFigs = 1;
        public const string Separator = "-";

        //Default Power Factor and Efficiency
        public const double XfrPf = 0.9;
        public const double XfrEff = 0.95;
        public const double HeaterPf = 0.99;
        public const double HeaterEff = 0.99;
        public const double OtherPf = 0.85;
        public const double OtherEff = 0.85;

        //Settings
            //public static string libraryDb = Properties.Settings.Default.DefaultDataTableLibrary;
            //public static string newProjectTemplateDb = Properties.Settings.Default.NewProjectTemplate;
            //public static string lastOpenedProjectDb = Properties.Settings.Default.LastOpenedProject;
            //public static string currentProjectDb = @"C:\Users\pdeau\OneDrive\Work\Visual Studio Projects\SQLite\EDTProjectTemplate1.0.db";
            //public static string currentProjectDb = Properties.Settings.Default.CurrentProject;

        #region Database Table Quick Names
        public const string loadListTable = "Loads";
        public static string dteqListTable = "DistributionEquipmentList";

        public static string loadTypeTable = "LoadTypes";
        public static string dteqTypeTable = "DistributionEquipmentTypes";

        public static string unitsTable = "Units";
        public static string voltage3PTable = "Voltage3P";

        public static string motorDataTable = "MotorData2";
        public static string loadDataTable = "LoadData";
        #endregion

        #region SQLite Data
        #endregion
    }
}
