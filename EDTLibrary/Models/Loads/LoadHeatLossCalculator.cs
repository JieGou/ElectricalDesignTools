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
                else if (load.ProtectionDevice.Type.Contains("V")) {
                    DriveHeatLoss = GetVfdHeatLoss(load);
                }
            }

            if (load.StandAloneStarterBool == true) {
                DriveHeatLoss = GetVfdHeatLoss(load);
            }
        }
        catch (Exception ex) {

            throw;
        }
    }

    private double GetVfdHeatLoss(IPowerConsumer load)
    {
        if (load.Unit == Units.kW.ToString()) {
            return TypeManager.GetVfdHeatSize(load.Size / .746, load.VoltageType.Voltage).HeatLoss;
        }
        else {
            return TypeManager.GetVfdHeatSize(load.Size, load.VoltageType.Voltage).HeatLoss;
        }
    }

    public double BreakerHeatLoss { get; set; }
    public double StarterHeatLoss { get; set; }
    public double DriveHeatLoss { get; set; }
    public double CableHeatLoss { get; set; }


}


