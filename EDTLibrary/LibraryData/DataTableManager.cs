using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EDTLibrary.LibraryData {
    public static class DataTableManager {




        public static double GetMotorEfficiency(LoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Motors.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("Voltage") == (int)load.Voltage
                                                                      && x.Field<double>("Size") == (double)load.Size
                                                                      && x.Field<string>("Unit") == load.Unit
                                                                      && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

            dtFiltered = filteredRows.CopyToDataTable();

            result = Double.Parse(dtFiltered.Rows[0]["Efficiency"].ToString());
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

            dtFiltered = filteredRows.CopyToDataTable();

            result = Double.Parse(dtFiltered.Rows[0]["PowerFactor"].ToString());
            return result;
        }

        public static double GetBreakerFrame(ILoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Breakers.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("FrameAmps") >= (int)load.Fla * 1.25);

            dtFiltered = filteredRows.CopyToDataTable();
            dtFiltered = dtFiltered.Select($"FrameAmps = MIN(FrameAmps)").CopyToDataTable();

            result = Double.Parse(dtFiltered.Rows[0]["FrameAmps"].ToString());
            return result;
        }

        public static double GetBreakerTrip(ILoadModel load) {
            double result = GlobalConfig.NoValueDouble;
            DataTable dt = LibraryTables.Breakers.Copy();
            DataTable dtFiltered;

            var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("TripAmps") >= (int)load.Fla * 1.25);

            dtFiltered = filteredRows.CopyToDataTable();
            dtFiltered = dtFiltered.Select($"TripAmps = MIN(TripAmps)").CopyToDataTable();

            result = Double.Parse(dtFiltered.Rows[0]["TripAmps"].ToString());
            return result;
        }

    }
}
