using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
public class LoadManager
{
    public static void SetLoadPd(LoadModel load)
    {
        if (load.Type == LoadTypes.MOTOR.ToString()) {
            load.PdType = EdtSettings.LoadDefaultPdTypeLV_Motor;
        }

        else {
            load.PdType = EdtSettings.LoadDefaultPdTypeLV_NonMotor;
        }
    }

    public static void SetLoadPdSize(LoadModel load)
    {
        //TODO - enum for PdTypes
        if (load.PdType.Contains("MCP") ||
            load.PdType.Contains("FVNR") ||
            load.PdType.Contains("FVR")) {
            load.PdSizeFrame = LibraryManager.GetMcpFrame(load);
            load.PdSizeTrip = LibraryManager.GetBreakerTrip(load);
            load.StarterType = load.PdType;
            load.StarterSize = LibraryManager.GetStarterSize(load);
            //load.PdSizeTrip = Math.Min(load.Fla * 1.25, load.PdSizeFrame);
            //load.PdSizeTrip = Math.Round(load.PdSizeTrip, 0);
        }
        else if (load.PdType == "BKR" ||
                 load.PdType == "VFD" || load.PdType == "VSD" ||
                 load.PdType == "RVS") {
            load.PdSizeFrame = LibraryManager.GetBreakerFrame(load);
            load.PdSizeTrip = LibraryManager.GetBreakerTrip(load);
        }
    }

    public static async Task<LoadModel> AddLoad(object loadToAddObject, ListManager listManager)
    {
        LoadFactory _loadFactory = new LoadFactory(listManager);
        LoadToAddValidator loadToAddValidator = (LoadToAddValidator)loadToAddObject;
        var IsValid = loadToAddValidator.IsValid();
        var errors = loadToAddValidator._errorDict;
        if (IsValid == false) return null;

        LoadModel newLoad = _loadFactory.CreateLoad(loadToAddValidator); //150ms

        IDteq dteqSubscriber = newLoad.FedFrom;
        if (dteqSubscriber != null) {
            //dteqSubscriber.AssignedLoads.Add(newLoad); //load gets added to AssignedLoads inside DistributionManager.UpdateFedFrom();
            newLoad.LoadingCalculated += dteqSubscriber.OnAssignedLoadReCalculated;
            newLoad.PropertyUpdated += DaManager.OnLoadPropertyUpdated;
        }

        //Save to Db
        //newLoad.Id = DaManager.SaveLoadGetId(newLoad);
        if (listManager.LoadList.Count != 0) {
            newLoad.Id = listManager.LoadList.Max(l => l.Id);
            newLoad.Id += 1;
        }
        else {
            newLoad.Id = 1;
        }
        listManager.LoadList.Add(newLoad);
        newLoad.CalculateLoading(); //after load is inserted to get new Id - //150ms


        //Cable
        newLoad.SizePowerCable(); // 51ms
        newLoad.CalculateCableAmps();
        if (listManager.CableList.Count != 0) {
            newLoad.PowerCable.Id = listManager.LoadList.Max(l => l.Id) + 1;
        }
        else {
            newLoad.PowerCable.Id = 1;
        }
        //newLoad.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newLoad.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
        listManager.CableList.Add(newLoad.PowerCable);
        return newLoad;
    }      
    

    public static async Task<int> DeleteLoad(object selectedLoadObject, ListManager listManager)
    {
        try {

            LoadModel selectedLoad = (LoadModel)selectedLoadObject;
            ComponentManager.DeleteComponents(selectedLoad, listManager);
            IDteq dteqToRecalculate = selectedLoad.FedFrom;
            int loadId = selectedLoad.Id;
            selectedLoad.PropertyUpdated -= DaManager.OnLoadPropertyUpdated;
            await CableManager.DeletePowerCableAsync(selectedLoad, listManager); //await
            await DaManager.prjDb.DeleteRecordAsync(GlobalConfig.LoadTable, loadId); //await

            var loadToRemove = listManager.LoadList.FirstOrDefault(load => load.Id == loadId);
            listManager.LoadList.Remove(loadToRemove);

            if (dteqToRecalculate != null) {
                selectedLoad.LoadingCalculated -= dteqToRecalculate.OnAssignedLoadReCalculated;
                dteqToRecalculate.AssignedLoads.Remove(loadToRemove);
                dteqToRecalculate.CalculateLoading();
            }
            return loadId;

        }
        catch (InvalidCastException ex) {
            ex.Data.Add("UserMessage", "Cannot delete Distribution Equipment from Load List");
            throw;
            }
        catch (Exception ex) {
            throw;
        }
        return -1;
        
    }
}
