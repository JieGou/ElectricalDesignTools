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

            TotalHeatLoss = MainBreakerHeatLoss + LoadBreakersHeatLoss + LoadStartersHeatLoss;
        }
        catch (Exception) {

            throw;
        }
    }
    public double TotalHeatLoss { get; set; }
    public double MainBreakerHeatLoss { get; set; }
    public double LoadBreakersHeatLoss { get; set; }
    public double LoadStartersHeatLoss { get; set; }
    public double LoadDrivesHeatLoss { get; set; }


    public double CableHeatLoss { get; set; }

}


