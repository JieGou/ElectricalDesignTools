using EDTLibrary.DataAccess;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EDTLibrary {
    public static class GlobalConfig {


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
        public const double NoValueDouble = 0.001;

        //Default Power Factor and Efficiency
        public const double DefaultTransformerPowerFactor = 0.9;
        public const double DefaultTransformerEfficiency = 0.95;

        public const double DefaultHeaterPowerFactor = 0.99;
        public const double DefaultHeaterEfficiency = 0.99;

        //TODO - figure out motor RPM
        public const double DefaultMotorRpm = 1800;

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
        public static string dteqListTable = "DistributionEquipment";
        public const string locationTable = "Locations";
        
        #endregion

        #region SQLite Data
        #endregion
    }
}
