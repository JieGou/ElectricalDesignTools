using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;

namespace EDTLibrary.Models.DistributionEquipment;

[AddINotifyPropertyChangedInterface]
public class DteqHeatLossCalculator
{
    public DistributionEquipment Dteq { get; set; }
    public void CalculateHeatLoss(DistributionEquipment dteq)
    {
        Dteq = dteq;

        try {
            if (dteq.Type == DteqTypes.XFR.ToString()) {
                EfficiencyHeatLoss = dteq.ConnectedKva * 1000 * dteq.PowerFactor * (1 - 0.98); 
            }


            MainBreakerHeatLoss = TypeManager.GetBreaker(dteq.Fla).HeatLoss;
            LoadBreakersHeatLoss = 0;
            LoadStartersHeatLoss = 0;
            LoadDrivesHeatLoss = 0;

            var loadHeatLossCalculator = new LoadHeatLossCalculator();
            foreach (var load in dteq.AssignedLoads) {
                loadHeatLossCalculator.CalculateHeatLoss(load);
                LoadBreakersHeatLoss += loadHeatLossCalculator.BreakerHeatLoss;
                LoadStartersHeatLoss += loadHeatLossCalculator.StarterHeatLoss;
                LoadDrivesHeatLoss += loadHeatLossCalculator.DriveHeatLoss;
            }

            TotalHeatLoss = EfficiencyHeatLoss + MainBreakerHeatLoss + LoadBreakersHeatLoss + LoadStartersHeatLoss + LoadDrivesHeatLoss;
        }
        catch (Exception) {

            throw;
        }
    }
    public double TotalHeatLoss { get; set; }
    public double EfficiencyHeatLoss { get; set; }
    public double MainBreakerHeatLoss { get; set; }
    public double LoadBreakersHeatLoss { get; set; }
    public double LoadStartersHeatLoss { get; set; }
    public double LoadDrivesHeatLoss { get; set; }


    public double CableHeatLoss { get; set; }

    // Bus      Watts / 20" section
    // 3000	    220
    // 2000	    110
    // 1600	    80
    // 1200	    60
    // 800	    40
    // 600	    30

}


