using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDTLibrary.Tests.LibraryData
{
    public class LibraryManagerTests
    {
        [Theory]
        [InlineData(480,50,"HP")]
        public void GetMotorEfficiency_ReturnsCorrectEfficiency(double voltage, double size, string unit, double rpm=1800)
        {
            ILoad ld = new LoadModel() { Voltage = voltage, Size = size, Unit = unit};

            double expected = 0.941;
            double actual = LibraryManager.GetMotorEfficiency(ld);

            Assert.Equal(expected, actual);
         
        }
    }
}
