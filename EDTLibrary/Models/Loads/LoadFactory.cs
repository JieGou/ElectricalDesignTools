using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Calculations;
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
            newLoad.CalculationFlags.CanUpdateFedFrom = false;

            newLoad.Voltage = Double.Parse(loadToAddValidator.Voltage);
            newLoad.VoltageType = loadToAddValidator.VoltageType;
            newLoad.VoltageTypeId = newLoad.VoltageType.Id;

            //Checks if dteq CanAdd
            foreach (var dteq in _listManager.DteqList) {  // 85 ms
                if (dteq.Tag == loadToAddValidator.FedFromTag) {
                    if (dteq.CanAdd(newLoad)) {
                        newLoad.FedFrom = dteq;
                        break;
                    }
                    else 
                    {
                        return null; 
                    }
                }
            }
            newLoad.Tag = loadToAddValidator.Tag;
            newLoad.Description = loadToAddValidator.Description;
            newLoad.Category = Categories.LOAD3P.ToString();
            newLoad.Type = loadToAddValidator.Type;

            newLoad.Voltage = Double.Parse(loadToAddValidator.Voltage);
            newLoad.VoltageType = loadToAddValidator.VoltageType;
            newLoad.VoltageTypeId = newLoad.VoltageType.Id;

            newLoad.Area = _listManager.AreaList.FirstOrDefault(a => a.Tag == loadToAddValidator.AreaTag);
            
            newLoad.Size = Double.Parse(loadToAddValidator.Size);
            

            newLoad.Unit = loadToAddValidator.Unit;
            newLoad.LoadFactor = Double.Parse(loadToAddValidator.LoadFactor);

            //if (newLoad.Type == LoadTypes.MOTOR.ToString()) {
            //    newLoad.PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
            //}
            //else {
            //    newLoad.PdType = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
            //}

            newLoad.SequenceNumber = loadToAddValidator.SequenceNumber;
            newLoad.PanelSide = loadToAddValidator.PanelSide;
            newLoad.CircuitNumber = loadToAddValidator.CircuitNumber;
            newLoad.CalculationFlags = new CalculationFlags();
            return newLoad;
        }

        public LoadModel CreateLoad(ILoad loadToCopy, string newTag)
        {
            LoadModel newLoad = new LoadModel();
            newLoad.CalculationFlags.CanUpdateFedFrom = false;

            newLoad.Voltage = loadToCopy.Voltage;
            newLoad.VoltageType = loadToCopy.VoltageType;
            newLoad.VoltageTypeId = newLoad.VoltageType.Id;

            //Checks if dteq CanAdd
            foreach (var dteq in _listManager.DteqList) {  // 85 ms
                if (dteq.Tag == loadToCopy.FedFromTag) {
                    if (dteq.CanAdd(newLoad)) {
                        newLoad.FedFrom = dteq;
                        break;
                    }
                    else {
                        return null;
                    }
                }
            }
            newLoad.Tag = newTag;
            newLoad.Description = loadToCopy.Description;
            newLoad.Category = Categories.LOAD3P.ToString();
            newLoad.Type = loadToCopy.Type;

            newLoad.Voltage = loadToCopy.Voltage;
            newLoad.VoltageType = loadToCopy.VoltageType;
            newLoad.VoltageTypeId = newLoad.VoltageType.Id;

            newLoad.Area = _listManager.AreaList.FirstOrDefault(a => a.Tag == loadToCopy.Area.Tag);

            newLoad.Size = loadToCopy.Size;


            newLoad.Unit = loadToCopy.Unit;
            newLoad.LoadFactor = loadToCopy.LoadFactor;

            //if (newLoad.Type == LoadTypes.MOTOR.ToString()) {
            //    newLoad.PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
            //}
            //else {
            //    newLoad.PdType = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
            //}

            newLoad.SequenceNumber = loadToCopy.SequenceNumber;
            newLoad.PanelSide = loadToCopy.PanelSide;
            newLoad.CircuitNumber = loadToCopy.CircuitNumber;
            newLoad.CalculationFlags = new CalculationFlags();
            return newLoad;
        }
    }
}
