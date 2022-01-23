using EDTLibrary.Models;

namespace EDTLibrary.Calculators
{
    public class ConductorQtyCalculator
    {
        public int Calculate(PowerConsumer load)
        {
            int result = 0;
            if (load.Category == Categories.LOAD3P.ToString()) {
                result = 3;
            }
            else if (load.Category == Categories.DTEQ.ToString()) {
                result = 3;
            }


            return result;
        }
    }
}
