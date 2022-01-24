using EDTLibrary.Models;

namespace EDTLibrary.Calculators
{
    public class CableDeratingCalculator
    {
        public double Calculate(IPowerConsumer load)
        {
            double result = 1;

            if (load.FedFrom == "UTILITY" || load.Category== Categories.DTEQ.ToString()) {
                return 1;
            }

            int loadCount = ListManager.dteqDict[load.FedFrom].LoadCount;

            if (loadCount * 3 >= 43) {
                result = 0.5;
            }
            else if (loadCount * 3 >= 25) {
                result = 0.6;
            }
            else if (loadCount * 3 >= 7) {
                result = 0.7;
            }
            else if (loadCount * 3 >= 4) {
                result = 0.8;
            }
            else {
                result = 1;
            }
            return result;
        }
    }
}
