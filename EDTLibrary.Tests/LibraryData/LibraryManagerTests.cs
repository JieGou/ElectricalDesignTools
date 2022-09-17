using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDTLibrary.Tests.LibraryData
{
    public class LibraryManagerTests
    {
        [Theory]
        [InlineData(666,50,"HP",0.001)]
        [InlineData(460,50,"HP",0.941)]
        public void GetMotorEfficiency_CorrectAndDefault(double voltage, double size, string unit, double expected)
        {
            DaManager.GettingRecords = true;
            ILoad load = new LoadModel() { Voltage = voltage, Size = size, Unit = unit};
            DataTables.Motors = CreateSampleMotorTable();


            double actual = DataTableSearcher.GetMotorEfficiency(load);

            Assert.Equal(expected, actual);
         
        }
        private DataTable CreateSampleMotorTable()
        {
            DataTable dt = new DataTable("Motors");

            dt.Columns.Add("Voltage",typeof(double));
            dt.Columns.Add("Size", typeof(double));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("RPM",typeof(double));
            dt.Columns.Add("Efficiency", typeof(double));
            dt.Columns.Add("PowerFactor", typeof(double));

            dt.Rows.Add(460, 50, "HP", 1800, 0.941, 0.87);

            return dt;
        }
     
    }


}
