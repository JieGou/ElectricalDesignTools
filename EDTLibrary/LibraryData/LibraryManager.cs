using EDTLibrary.Models;
using EDTLibrary.Models.Loads;
using System;
using System.Data;

namespace EDTLibrary.LibraryData
{
    public static class LibraryManager {

        public static double GetMotorEfficiency(ILoad load) {
            double result = GlobalConfig.NoValueDouble;
            if (LibraryTables.Motors != null) {

                DataTable dt = LibraryTables.Motors.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("Voltage") == (double)load.Voltage
                                                             && x.Field<double>("Size") == (double)load.Size
                                                             && x.Field<string>("Unit") == load.Unit
                                                             && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    result = Double.Parse(dtFiltered.Rows[0]["Efficiency"].ToString());
                }
                catch { }
            }

            return result;
        }

        public static double GetMotorPowerFactor(ILoad load) {
            double result = GlobalConfig.NoValueDouble;
            if (LibraryTables.Motors != null) {

                DataTable dt = LibraryTables.Motors.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("Voltage") == (double)load.Voltage
                                                             && x.Field<double>("Size") == (double)load.Size
                                                             && x.Field<string>("Unit") == load.Unit
                                                             && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    result = Double.Parse(dtFiltered.Rows[0]["PowerFactor"].ToString());
                }
                catch { }
            }

            return result;
        }

        public static double GetBreakerFrame(IPowerConsumer load) {
            double result = GlobalConfig.NoValueDouble;
            if (LibraryTables.Breakers != null) {
                DataTable dt = LibraryTables.Breakers.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("FrameAmps") >= (int)load.Fla);

                if (load.Type == DteqTypes.XFR.ToString() || load.Type == LoadTypes.MOTOR.ToString() || load.Type == LoadTypes.TRANSFORMER.ToString()) {
                    filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("FrameAmps") >= (int)load.Fla * 1.25);
                }

                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    dtFiltered = dtFiltered.Select($"FrameAmps = MIN(FrameAmps)").CopyToDataTable();
                    result = Double.Parse(dtFiltered.Rows[0]["FrameAmps"].ToString());
                }
                catch { }
            }
            return result;
        }

        public static double GetBreakerTrip(IPowerConsumer load) {
            double result = GlobalConfig.NoValueDouble;
            if (LibraryTables.Breakers != null) {

                DataTable dt = LibraryTables.Breakers.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("TripAmps") >= (int)load.Fla);

                if (load.Type == DteqTypes.XFR.ToString() || load.Type == LoadTypes.MOTOR.ToString() || load.Type == LoadTypes.TRANSFORMER.ToString()) {
                    filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("TripAmps") >= (int)load.Fla * 1.25);
                }

                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    dtFiltered = dtFiltered.Select($"TripAmps = MIN(TripAmps)").CopyToDataTable();
                    result = Double.Parse(dtFiltered.Rows[0]["TripAmps"].ToString());
                }
                catch { }
            }
            
            return result;
        }

        public static double GetCableVoltageClass(double voltage)
        {
            double result = GlobalConfig.NoValueDouble;

            if (LibraryTables.VoltageTypes != null) {

                DataTable dt = LibraryTables.VoltageTypes.Select($"CableVoltageClass >= {voltage}").CopyToDataTable();
                dt = dt.Select($"CableVoltageClass = MIN(CableVoltageClass)").CopyToDataTable();
                result = Double.Parse(dt.Rows[0]["CableVoltageClass"].ToString());
            }

            return result;
        }
    }
}
