﻿using EdtLibrary.Managers;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Selectors;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Managers;
public class LoadManager
{

    public static async Task<LoadModel> AddLoad(object loadToAddObject, ListManager listManager, bool append = true)
    {
        GlobalConfig.Importing = true;
        DaManager.Importing = true;

        LoadModel newLoad = new LoadModel();
        LoadFactory _loadFactory = new LoadFactory(listManager);

        //create
        if (loadToAddObject is LoadToAddValidator) {
            var loadToAddValidator = (LoadToAddValidator)loadToAddObject;
            var errors = loadToAddValidator._errorDict;
            loadToAddValidator.IsValid();
            //Auto-Tag
            if (bool.Parse(TagSettings.AutoTagEquipment)) {
                if (errors.ContainsKey("Type")) {
                    // do nothing since other data is required
                }
                else if (errors.ContainsKey("Tag") || loadToAddValidator.Tag == GlobalConfig.EmptyTag || string.IsNullOrWhiteSpace(loadToAddValidator.Tag)) {
                    loadToAddValidator.Tag = TagManager.AssignEqTag(new DummyLoad { Type = loadToAddValidator.Type }, listManager);
                }
            }

            var IsValid = loadToAddValidator.IsValid();
            if (IsValid == false) return null;

            //CreateLoad checks if the Dteq has enough space to add the load
            newLoad = _loadFactory.CreateLoad(loadToAddValidator); //150ms

        }

        //copy
        else if(loadToAddObject is ILoad){
            var loadToAdd = (ILoad)loadToAddObject;
            newLoad = _loadFactory.CreateLoad_CopyFromExisting(
                        loadToAdd, 
                        TagManager.AssignEqTag(new DummyLoad { Type = loadToAdd.Type }, listManager)
                        );
        }

        if (newLoad == null) return null; 
      
        EfficiencyAndPowerFactorSelector.SetEfficiencyAndPowerFactor(newLoad);

        IDteq dteqSubscriber = newLoad.FedFrom;
        if (dteqSubscriber != null) {

            if (append == true) {
                dteqSubscriber.AddNewLoad(newLoad); //load gets added to AssignedLoads inside DistributionManager.UpdateFedFrom
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
        ProtectionDeviceManager.AddProtectionDevice(newLoad, listManager);
        newLoad.FedFrom.SetLoadProtectionDevice(newLoad);
        newLoad.CalculateLoading(); //after load is inserted to get new Id - //150ms


        //Cable
        newLoad.PowerCable.Type = CableManager.CableSizer.GetDefaultCableType(newLoad);
        newLoad.PowerCable.TypeModel = CableManager.CableSizer.GetDefaultCableTypeModel(newLoad);
        newLoad.SizePowerCable(); // 51ms
        newLoad.PowerCable.LoadId = newLoad.Id;
        newLoad.PowerCable.LoadType = newLoad.GetType().ToString();

        newLoad.PowerCable.SetTypeProperties();
        newLoad.PowerCable.CreateTag();

        //Get Id
        //newLoad.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newLoad.PowerCable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);

        if (listManager.CableList.Count != 0) {
            newLoad.PowerCable.Id = listManager.CableList.Max(l => l.Id) + 1;
        }
        else {
            newLoad.PowerCable.Id = 1;
        }

        //ProtectionDeviceManager.AddProtectionDevice(newLoad, listManager);

        //Save to Db
        DaManager.PrjDb.UpsertRecord(newLoad.PowerCable, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);

        newLoad.CalculateCableAmps();
        listManager.CableList.Add(newLoad.PowerCable);
        DaManager.PrjDb.UpsertRecord(newLoad, GlobalConfig.LoadTable, NoSaveLists.LoadNoSaveList);

        listManager.LoadList.Add(newLoad);

        Debug.Print("Load List COUNT: " + listManager.LoadList.Count.ToString() + " - " + newLoad.Tag);
        
        DaManager.Importing = false;
        GlobalConfig.Importing = false;

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

            DaManager.DeletingLoad = true;

            LoadModel loadToDelete = (LoadModel)loadToDeleteObject;
            ComponentManager.DeleteComponents(loadToDelete, listManager);
            ComponentManager.DeleteLcs(loadToDelete, listManager);
            ProtectionDeviceManager.DeleteProtectionDevices(loadToDelete, listManager);
            IDteq dteqToRecalculate = loadToDelete.FedFrom;
            int loadId = loadToDelete.Id;
            await CableManager.DeletePowerCableAsync(loadToDelete, listManager); //await
            await CableManager.DeleteLoadComponentsCablesAsync(loadToDelete, listManager); //await
            await DaManager.PrjDb.DeleteRecordAsync(GlobalConfig.LoadTable, loadId); //await

            var loadToRemove = listManager.LoadList.FirstOrDefault(load => load.Id == loadId);
            listManager.LoadList.Remove(loadToRemove);

            loadToDelete.PropertyUpdated -= DaManager.OnLoadPropertyUpdated;
           
            if (dteqToRecalculate != null) {
                loadToDelete.LoadingCalculated -= dteqToRecalculate.OnAssignedLoadReCalculated;

                dteqToRecalculate.AssignedLoads.Remove(loadToRemove);
                dteqToRecalculate.CalculateLoading();
            }

            DaManager.DeletingLoad = false;
            OnLoadDeleted();

            return loadId;

        }
        catch (InvalidCastException ex) {
            ex.Data.Add("UserMessage", "Cannot delete Distribution Equipment from Load List");
            throw;
        }
        catch (Exception ex) {
            throw;
        }
        finally {
            DaManager.DeletingLoad = false;
        }
        return -1;

    }



    public static event EventHandler LoadDeleted;
    public static void OnLoadDeleted()
    {
        if (LoadDeleted != null) {
            LoadDeleted(nameof(FedFromManager), EventArgs.Empty);
        }
    }
}
