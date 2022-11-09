﻿using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
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
            load.PdSizeFrame = DataTableSearcher.GetMcpFrame(load);
            load.PdSizeTrip = DataTableSearcher.GetBreakerTrip(load);
            load.StarterType = load.PdType;
            load.StarterSize = DataTableSearcher.GetStarterSize(load);
            //load.PdSizeTrip = Math.Min(load.Fla * 1.25, load.PdSizeFrame);
            //load.PdSizeTrip = Math.Round(load.PdSizeTrip, 0);
        }
        else if (load.PdType == "BKR" ||
                 load.PdType == "VFD" || load.PdType == "VSD" ||
                 load.PdType == "RVS") {
            load.PdSizeFrame = DataTableSearcher.GetBreakerFrame(load);
            load.PdSizeTrip = DataTableSearcher.GetBreakerTrip(load);
        }
    }

    
    public static async Task<LoadModel> AddLoad(object loadToAddObject, ListManager listManager, bool append = true)
    {
        LoadFactory _loadFactory = new LoadFactory(listManager);
        var loadToAddValidator = (LoadToAddValidator)loadToAddObject;
        var IsValid = loadToAddValidator.IsValid();
        var errors = loadToAddValidator._errorDict;
        if (IsValid == false) return null;
        
        //CreateLoad checks if the Dteq has enough space to add the load
        
        LoadModel newLoad = _loadFactory.CreateLoad(loadToAddValidator); //150ms
        if (newLoad == null) return null; 
      
        IDteq dteqSubscriber = newLoad.FedFrom;
        if (dteqSubscriber != null) {

            if (append == true) {
                dteqSubscriber.AdddNewLoad(newLoad); //load gets added to AssignedLoads inside DistributionManager.UpdateFedFrom
                                                         //which is fired inside loadFactory when setting fedfrom
                                                         //but this checks if it is already added;
            }

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
        newLoad.PowerCable.Type = CableManager.CableSizer.GetDefaultCableType(newLoad);
        newLoad.SizePowerCable(); // 51ms
        newLoad.PowerCable.LoadId = newLoad.Id;
        newLoad.PowerCable.LoadType = newLoad.GetType().ToString();

        newLoad.PowerCable.SetTypeProperties();


        //Get Id
        //newLoad.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newLoad.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);

        if (listManager.CableList.Count != 0) {
            newLoad.PowerCable.Id = listManager.LoadList.Max(l => l.Id) + 1;
        }
        else {
            newLoad.PowerCable.Id = 1;
        }

        //Save to Db
        DaManager.prjDb.UpsertRecord(newLoad.PowerCable, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);

        newLoad.CalculateCableAmps();
        listManager.CableList.Add(newLoad.PowerCable);
        return newLoad;
    }

    /// <summary>
    /// Deletes a LoadModel and all associated models; components, cables, etc. and unsubscrbes from events.
    /// </summary>
    /// <param name="loadToDeleteObject"></param>
    /// <param name="listManager"></param>
    /// <returns></returns>
    public static async Task<int> DeleteLoadAsync(object loadToDeleteObject, ListManager listManager)
    {
        try {

            LoadModel loadToDelete = (LoadModel)loadToDeleteObject;
            ComponentManager.DeleteComponents(loadToDelete, listManager);
            IDteq dteqToRecalculate = loadToDelete.FedFrom;
            int loadId = loadToDelete.Id;
            await CableManager.DeletePowerCableAsync(loadToDelete, listManager); //await
            await CableManager.DeleteLoadComponentsCablesAsync(loadToDelete, listManager); //await
            await DaManager.prjDb.DeleteRecordAsync(GlobalConfig.LoadTable, loadId); //await

            var loadToRemove = listManager.LoadList.FirstOrDefault(load => load.Id == loadId);
            listManager.LoadList.Remove(loadToRemove);

            loadToDelete.PropertyUpdated -= DaManager.OnLoadPropertyUpdated;
            if (dteqToRecalculate != null) {
                loadToDelete.LoadingCalculated -= dteqToRecalculate.OnAssignedLoadReCalculated;

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
