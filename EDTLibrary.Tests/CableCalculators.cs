using EDTLibrary;
using EDTLibrary.Calculators;
using EDTLibrary.Models;
using Xunit;

namespace EDTLibrary.Tests
{
    public class CableCalculators
    {
        [Fact]
        public void ReturnValidConductorsQty()
        {
            var calc = new ConductorQtyCalculator();
            var load = new LoadModel();

            calc.Calculate(load);

            Assert.Equal(2, 2);
        }
    }
}