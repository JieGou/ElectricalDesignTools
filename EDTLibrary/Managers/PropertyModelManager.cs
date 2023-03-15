using EdtLibrary.Models.AdditionalProperties;
using EdtLibrary.Models.TypeProperties;
using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
internal class PropertyModelManager
{
    public static PropertyModelBase CreateNewPropModel(string type, ComponentModelBase owner)
    {
        PropertyModelBase propModel = new BreakerPropModel();

        if (type == "Breaker") {
            propModel = new BreakerPropModel();
            if (ScenarioManager.ListManager.BreakerPropModels.Count == 0) {
                propModel.Id = 1;
            }
            else {
                propModel.Id = ScenarioManager.ListManager.BreakerPropModels.Max(p => p.Id) + 1;
            }
            ScenarioManager.ListManager.BreakerPropModels.Add((BreakerPropModel)propModel);
        }


        else if (type == "Disconnect" || type == "FDS") {
            propModel = new DisconnectPropModel();
            if (ScenarioManager.ListManager.DisconnectPropModels.Count == 0) {
                propModel.Id = 1;
            }
            else {
                propModel.Id = ScenarioManager.ListManager.DisconnectPropModels.Max(p => p.Id) + 1;
            }
            ScenarioManager.ListManager.DisconnectPropModels.Add((DisconnectPropModel)propModel);

        }

        propModel.PropertyUpdated += DaManager.OnTypeModelPropertyUpdated;
        propModel.Owner = owner;
        propModel.OnPropertyUpdated();

        return propModel;
    }

    public static void DeletePropModel(PropertyModelBase propModel)
    {
        if (propModel == null) return;
        propModel.PropertyUpdated -= DaManager.OnTypeModelPropertyUpdated;
        DaManager.DeletePropModelAsync(propModel);
    }
}
