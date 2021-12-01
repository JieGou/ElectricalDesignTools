using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EDTLibrary.LibraryData {
    public static class DataTableManager {
        public static double GetMotorEfficiency(LoadModel load) {
            double eff = GlobalConfig.NoValueDouble;
            DataTable motorSelected;
            DataTable motors = DataTables.Motors.Copy();

            var motorsFilteredResults = motors.AsEnumerable().Where(x => x.Field<double>("Voltage") == (int)load.Voltage
                                                                      && x.Field<double>("Size") == (double)load.Size
                                                                      && x.Field<string>("Unit") == load.Unit
                                                                      && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

            motorSelected = motorsFilteredResults.CopyToDataTable();

            eff = Double.Parse(motorSelected.Rows[0]["Efficiency"].ToString());
            return eff;

        }

        public static double GetMotorPowerFactor(LoadModel load) {
            double pf = GlobalConfig.NoValueDouble;
            DataTable motorSelected;
            DataTable motors = DataTables.Motors.Copy();

            var motorFilteredResults = motors.AsEnumerable().Where(x => x.Field<double>("Voltage") == (int)load.Voltage
                                                          && x.Field<double>("Size") == (double)load.Size
                                                          && x.Field<string>("Unit") == load.Unit
                                                          && x.Field<double>("RPM") == GlobalConfig.DefaultMotorRpm);

            motorSelected = motorFilteredResults.CopyToDataTable();

            pf = Double.Parse(motorSelected.Rows[0]["PowerFactor"].ToString());
            return pf;
        }

        public static double GetBreakerFrame(LoadModel load) {
            double pdFrame = GlobalConfig.NoValueDouble;
            DataTable pdFrameSelected;
            DataTable pdFrames = DataTables.Breakers.Copy();

            var pdFrameFilteredResults = pdFrames.AsEnumerable().Where(x => x.Field<double>("FrameAmps") >= (int)load.Fla * 1.25);

            pdFrameSelected = pdFrameFilteredResults.CopyToDataTable();
            pdFrameSelected = pdFrameSelected.Select($"FrameAmps = MIN(FrameAmps)").CopyToDataTable();

            pdFrame = Double.Parse(pdFrameSelected.Rows[0]["FrameAmps"].ToString());
            return pdFrame;
        }

        public static double GetBreakerTrip(LoadModel load) {
            double pdTrip = GlobalConfig.NoValueDouble;
            DataTable pdTripSelected;
            DataTable pdTrips = DataTables.Breakers.Copy();

            var pdFrameFilteredResults = pdTrips.AsEnumerable().Where(x => x.Field<double>("TripAmps") >= (int)load.Fla * 1.25);

            pdTripSelected = pdFrameFilteredResults.CopyToDataTable();
            pdTripSelected = pdTripSelected.Select($"TripAmps = MIN(TripAmps)").CopyToDataTable();

            pdTrip = Double.Parse(pdTripSelected.Rows[0]["TripAmps"].ToString());
            return pdTrip;
        }

    }
}
