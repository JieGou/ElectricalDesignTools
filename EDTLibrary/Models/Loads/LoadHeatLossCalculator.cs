using EDTLibrary.LibraryData;
using PropertyChanged;
using System;

namespace EDTLibrary.Models.Loads;

[AddINotifyPropertyChangedInterface]
public class LoadHeatLossCalculator
{
    public IPowerConsumer Load { get; set; }
    public void CalculateHeatLoss(IPowerConsumer load)
    {
        Load = load;

        BreakerHeatLoss = 0;
        StarterHeatLoss = 0;
        DriveHeatLoss = 0;


        try {
            BreakerHeatLoss = TypeManager.GetBreaker(load.Fla).HeatLoss;

            if (load.ProtectionDevice.Type != null) {
                if (load.ProtectionDevice.Type.Contains("MCP")) {
                    StarterHeatLoss = TypeManager.GetStarter(load.Size, load.Unit).HeatLossWatts;
                }
            }

            if (load.StandAloneStarterBool == true) {
                if (load.Unit == Units.kW.ToString()) {
                    DriveHeatLoss = TypeManager.GetVfdHeatSize(load.Size / .746, load.VoltageType.Voltage).HeatLoss;
                }
                else {
                    DriveHeatLoss = TypeManager.GetVfdHeatSize(load.Size, load.VoltageType.Voltage).HeatLoss;
                }
            }
        }
        catch (Exception ex) {

            throw;
        }
    }

    public double BreakerHeatLoss { get; set; }
    public double StarterHeatLoss { get; set; }
    public double DriveHeatLoss { get; set; }
    public double CableHeatLoss { get; set; }


}


