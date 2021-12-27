using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EDTLibrary.LibraryData {
    public static class LibraryManager {

        public static double GetMotorEfficiency(LoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Motors.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("Voltage") == (int)load.Voltage
                                                         && x.Field<double>("Size") == (double)load.Size
                                                         && x.Field<string>("Unit") == load.Unit
                                                         && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

            try {
                dtFiltered = filteredRows.CopyToDataTable();
                result = Double.Parse(dtFiltered.Rows[0]["Efficiency"].ToString());
            }
            catch { }
            return result;
        }

        public static double GetMotorPowerFactor(LoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Motors.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("Voltage") == (int)load.Voltage
                                                         && x.Field<double>("Size") == (double)load.Size
                                                         && x.Field<string>("Unit") == load.Unit
                                                         && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

            try {
                dtFiltered = filteredRows.CopyToDataTable();
                result = Double.Parse(dtFiltered.Rows[0]["PowerFactor"].ToString());
            }
            catch { }
            return result;
        }

        public static double GetBreakerFrame(ILoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Breakers.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("FrameAmps") >= (int)load.Fla * 1.25);

            try {
                dtFiltered = filteredRows.CopyToDataTable();
                dtFiltered = dtFiltered.Select($"FrameAmps = MIN(FrameAmps)").CopyToDataTable();
                result = Double.Parse(dtFiltered.Rows[0]["FrameAmps"].ToString());
            }
            catch { }
            return result;
        }

        public static double GetBreakerTrip(ILoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Breakers.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("TripAmps") >= (int)load.Fla * 1.25);

            try {
                dtFiltered = filteredRows.CopyToDataTable();
                dtFiltered = dtFiltered.Select($"TripAmps = MIN(TripAmps)").CopyToDataTable();
                result = Double.Parse(dtFiltered.Rows[0]["TripAmps"].ToString());
            }
            catch { }
            return result;
        }

    }
}
