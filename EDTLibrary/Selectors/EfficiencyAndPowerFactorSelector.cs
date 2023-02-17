using EDTLibrary.LibraryData;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Selectors;
public class EfficiencyAndPowerFactorSelector
{
    public static void SetEfficiencyAndPowerFactor(LoadModel load)
    {
        //PowerFactor and Efficiency
        if (load.Type == LoadTypes.HEATER.ToString()) {
            load.Unit = Units.kW.ToString();
            load.Efficiency = double.Parse(EdtProjectSettings.LoadDefaultEfficiency_Heater);
            load.PowerFactor = double.Parse(EdtProjectSettings.LoadDefaultPowerFactor_Heater);
        }

        else if (load.Type == LoadTypes.MOTOR.ToString()) {
            load.Efficiency = DataTableSearcher.GetMotorEfficiency(load);
            load.PowerFactor = DataTableSearcher.GetMotorPowerFactor(load);
        }
        else if (load.Type == LoadTypes.HEATER.ToString()) {
            load.Efficiency = double.Parse(EdtProjectSettings.LoadDefaultEfficiency_Heater);
            load.PowerFactor = double.Parse(EdtProjectSettings.LoadDefaultPowerFactor_Heater);
        }
        else if (load.Type == LoadTypes.PANEL.ToString()) {
            load.Efficiency = double.Parse(EdtProjectSettings.LoadDefaultEfficiency_Panel);
            load.PowerFactor = double.Parse(EdtProjectSettings.LoadDefaultPowerFactor_Panel);
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            load.Efficiency = double.Parse(EdtProjectSettings.LoadDefaultEfficiency_Other);
            load.PowerFactor = double.Parse(EdtProjectSettings.LoadDefaultPowerFactor_Other);
        }
        else {
            load.Efficiency = double.Parse(EdtProjectSettings.LoadDefaultEfficiency_Other);
            load.PowerFactor = double.Parse(EdtProjectSettings.LoadDefaultPowerFactor_Other);
        }

        if (load.Efficiency > 1)
            load.Efficiency /= 100;
        if (load.PowerFactor > 1)
            load.PowerFactor /= 100;

        load.Efficiency = Math.Round(load.Efficiency, 3);
        load.PowerFactor = Math.Round(load.PowerFactor, 2);
    }
}
