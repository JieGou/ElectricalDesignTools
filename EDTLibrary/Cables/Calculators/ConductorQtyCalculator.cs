using EDTLibrary.Models;

namespace EDTLibrary.Calculators
{
    public class ConductorQtyCalculator
    {
        public int Calculate(IHasLoading load)
        {
            int result = 0;
            if (load.Category == Categories.LOAD3P.ToString()) {
                result = 3;
            }
            else if (load.Category == Categories.DIST.ToString()) {
                result = 3;
            }


            return result;
        }
    }
}
