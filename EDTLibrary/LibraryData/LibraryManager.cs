using EDTLibrary.LibraryData.Cables;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace EDTLibrary.LibraryData
{
    public static class LibraryManager {
        public static ObservableCollection<CecCableAmpacityModel> CecCableAmpacities { get; set; }
        public static double GetMotorEfficiency(ILoad load) {
            double result = GlobalConfig.NoValueDouble;
            if (DataTables.Motors != null) {

                DataTable dt = DataTables.Motors.Copy();
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
            if (DataTables.Motors != null) {

                DataTable dt = DataTables.Motors.Copy();
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
            if (DataTables.Breakers != null) {
                DataTable dt = DataTables.Breakers.Copy();
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
            if (DataTables.Breakers != null) {

                DataTable dt = DataTables.Breakers.Copy();
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

        public static double GetMcpFrame(IPowerConsumer load)
        {
            double result = GlobalConfig.NoValueDouble;
            if (DataTables.MCPs != null) {
                DataTable dt = DataTables.MCPs.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<double>("Voltage") == (double)load.Voltage
                                                             && x.Field<double>("HP") == (double)load.Size) ;


                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    result = Double.Parse(dtFiltered.Rows[0]["Size"].ToString());
                }
                catch { }
            }
            return result;
        }

        public static double GetStarterSize(IPowerConsumer load)
        {
            double result = GlobalConfig.NoValueDouble;
            if (DataTables.Starters != null) {
                DataTable dt = DataTables.Starters.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<string>("Unit") == load.Unit
                                                             && x.Field<double>("HP") <= (double)load.Size);


                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    dtFiltered = dtFiltered.Select($"Size = Max(Size)").CopyToDataTable();
                    result = Double.Parse(dtFiltered.Rows[0]["Size"].ToString());
                }
                catch { }
            }
            return result;
        }


        public static double GetCableVoltageClass(double voltage)
        {
            double result = GlobalConfig.NoValueDouble;
            try {
                if (DataTables.VoltageTypes != null) {

                    DataTable dt = DataTables.VoltageTypes.Select($"CableVoltageClass >= {voltage}").CopyToDataTable();
                    dt = dt.Select($"CableVoltageClass = MIN(CableVoltageClass)").CopyToDataTable();
                    result = Double.Parse(dt.Rows[0]["CableVoltageClass"].ToString());
                }
                return result;
            }
            catch (InvalidOperationException ex) {
                return voltage;
            }
            catch (Exception ex) {
                throw;
            }
        }
        public static double GetCableDerating_CecTable5A(IPowerCable cable, double ambientTemp)
        {
            Debug.WriteLine("LibraryManager_GetCableDerating_CecTable5A");

            double result = .99999;
            if (DataTables.Motors != null) {

                DataTable dt = DataTables.CEC_Table5A.Copy();
                DataTable dtFiltered;

                var filteredRows = dt.AsEnumerable().Where(x => x.Field<long>("AmbientTemp") == (long)ambientTemp);

                try {
                    dtFiltered = filteredRows.CopyToDataTable();
                    if (cable.TypeModel != null) {
                        string col = "CableTemp" + cable.TypeModel.TemperatureRating.ToString();
                        result = Double.Parse(dtFiltered.Rows[0][col].ToString());
                    }
                }
                catch (InvalidCastException ex) {
                    //throw;
                }
            }
            return result;
        }
    }
}
