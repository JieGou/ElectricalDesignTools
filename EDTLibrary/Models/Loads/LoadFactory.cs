using EDTLibrary.Managers;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads
{
    public class LoadFactory
    {

        private ListManager _listManager;

        public LoadFactory(ListManager listManager)
        {
            _listManager = listManager;
        }

        public LoadModel CreateLoad(LoadToAddValidator loadToAddValidator)
        {
            LoadModel newLoad = new LoadModel();

            foreach (var dteq in _listManager.DteqList) {  // 85 ms
                if (dteq.Tag == loadToAddValidator.FedFromTag) {

                    newLoad.FedFrom = dteq; 

                    break;
                }
            }
            newLoad.Tag = loadToAddValidator.Tag;
            newLoad.Category = Categories.LOAD3P.ToString();
            newLoad.Type = loadToAddValidator.Type;
            newLoad.Area = _listManager.AreaList.FirstOrDefault(a => a.Tag == loadToAddValidator.AreaTag);

            newLoad.Size = Double.Parse(loadToAddValidator.Size);
            newLoad.Description = loadToAddValidator.Description;
            newLoad.Voltage = Double.Parse(loadToAddValidator.Voltage);
            newLoad.Unit = loadToAddValidator.Unit;
            newLoad.LoadFactor = Double.Parse(loadToAddValidator.LoadFactor);

            if (newLoad.Type == LoadTypes.MOTOR.ToString()) {
                newLoad.PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
            }
            else {
                newLoad.PdType = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
            }

            return newLoad;
        }
    }
}
