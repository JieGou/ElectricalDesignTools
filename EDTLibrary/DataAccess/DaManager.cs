﻿using EdtLibrary.Models.AdditionalProperties;
using EdtLibrary.Models.TypeProperties;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Components.ProtectionDevices;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Raceways;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace EDTLibrary.DataAccess;

public class DaManager {

    private static string _projectFile;
    private static string _libraryFile;

    public static bool IsProjectLoaded { get; private set; }
    public static bool IsLibraryLoaded { get; private set; }

    public static bool Importing { get; set; } = false;
    public static bool GettingRecords { get; set; } = true;
    public static bool DeletingLoad { get; set; } = false;

    public static IDaConnector PrjDb { get; set; }
    public static IDaConnector LibDb { get; set; }

    public static void SetProjectDb(IDaConnector daConnector) {
        PrjDb = daConnector;
        _projectFile = daConnector.ConString;
    }

    public static void SetLibraryDb( IDaConnector daConnector) {
        LibDb = daConnector;
        _libraryFile = daConnector.ConString;
    }

    public static bool GetLibraryTables()
    {
        Type libraryTablesClass = typeof(DataTables); // MyClass is static class with static properties
        DataTable dt = new DataTable();
        foreach (var prop in libraryTablesClass.GetProperties()) {
            prop.SetValue(dt, LibDb.GetDataTable(prop.Name));
        }
        IsLibraryLoaded = true;
        TypeManager.GetTypeTables();
        return IsLibraryLoaded;
    }

    #region General
    public static void DeleteAllModelRecords()
    {
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.XfrTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.SwgTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.MccTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.DpnTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.SplitterTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.LoadTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.LoadCircuitTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.CableTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.ComponentTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.ProtectionDeviceTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.LcsTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.RacewayTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.RacewayRouteSegmentsTable);

        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.BreakerPropsTable);
        DaManager.PrjDb.DeleteAllRecords(GlobalConfig.DisconnectPropsTable);
    }
    #endregion

    public static AreaModel GetArea(int locationId)
    {
        try {
            return PrjDb.GetRecordById<AreaModel>(GlobalConfig.AreaTable, locationId);

        }
        catch (Exception) {

            throw;
        }    }

    //Events

    public static void OnDteqPropertyUpdated(object source, EventArgs e)
    {

        try {
            if (DaManager.GettingRecords == false) {
                IDteq dteq = (IDteq)source;
                if (dteq.Tag == GlobalConfig.Deleted ) return;
                DaManager.UpsertDteqAsync(dteq);
            } 
        }
        catch (Exception) {
            throw;
        }
    }
    public static async Task UpsertDteqAsync(IDteq iDteq)
    {
        try {
            await Task.Run(() => {
                UpsertDteq(iDteq);
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }
    public static void UpsertDteq(IDteq iDteq)
    {
        try {
            if (GlobalConfig.Importing == true) return;
            if (iDteq == GlobalConfig.DteqDeleted) { return; }

            if (iDteq.GetType() == typeof(DteqModel)) {
                var model = (DteqModel)iDteq;
                PrjDb.UpsertRecord(model, GlobalConfig.DteqTable, NoSaveLists.DteqNoSaveList);
            }
            else if (iDteq.GetType() == typeof(XfrModel)) {
                var model = (XfrModel)iDteq;
                PrjDb.UpsertRecord(model, GlobalConfig.XfrTable, NoSaveLists.DteqNoSaveList);
            }
            else if (iDteq.GetType() == typeof(SwgModel)) {
                var model = (SwgModel)iDteq;
                PrjDb.UpsertRecord(model, GlobalConfig.SwgTable, NoSaveLists.DteqNoSaveList);
            }
            else if (iDteq.GetType() == typeof(MccModel)) {
                var model = (MccModel)iDteq;
                PrjDb.UpsertRecord(model, GlobalConfig.MccTable, NoSaveLists.DteqNoSaveList);
            }


            else if (iDteq.GetType() == typeof(DpnModel)) {
                var model = (DpnModel)iDteq;
                PrjDb.UpsertRecord(model, GlobalConfig.DpnTable, NoSaveLists.DteqNoSaveList);
            }

            else if (iDteq.GetType() == typeof(SplitterModel)) {
                var model = (SplitterModel)iDteq;
                PrjDb.UpsertRecord(model, GlobalConfig.SplitterTable, NoSaveLists.DteqNoSaveList);
            }
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    public static void OnLoadPropertyUpdated(object source, EventArgs e)
    {

        if (DaManager.GettingRecords == true) return;
        UpsertLoadAsync((LoadModel)source);
    }
    public static async Task UpsertLoadAsync(LoadModel load)
    {

        try {
            if (GlobalConfig.Importing == true) return;
            await Task.Run(() => {
                PrjDb.UpsertRecord(load, GlobalConfig.LoadTable, NoSaveLists.LoadNoSaveList);
            });

        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ex;

        }
    }

    public static void OnLoadCircuitPropertyUpdated(object sender, EventArgs e)
    {
        if (DaManager.GettingRecords == true) return;
        UpsertLoadCircuitAsync((LoadCircuit)sender);
    }
    public static async Task UpsertLoadCircuitAsync(LoadCircuit load)
    {

        try {
            await Task.Run(() => {
                if (GlobalConfig.Importing == true) return;
                PrjDb.UpsertRecord(load, GlobalConfig.LoadCircuitTable, NoSaveLists.LoadNoSaveList);
            });

        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ex;

        }
    }


    public static void OnComponentPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords) return;
        DaManager.UpsertComponentAsync((ComponentModel)source);
        
    }
    public static void OnDrivePropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            DaManager.UpsertDrive((DriveModel)source);
        }
    }

    private static void UpsertDrive(DriveModel drive)
    {
        try {
            if (GlobalConfig.Importing == true) return;

            PrjDb.UpsertRecord(drive, GlobalConfig.DriveTable, NoSaveLists.CompNoSaveList);
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ;
        }
    }

    public static void OnLcsPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            DaManager.UpsertLcsAsync((LocalControlStationModel)source);
        }
    }

    public static void OnAreaPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            PrjDb.UpsertRecord<AreaModel>((AreaModel)source, GlobalConfig.AreaTable, NoSaveLists.AreaNoSaveList);
        }
    }

    public static void OnPowerCablePropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            //prjDb.UpsertRecord<CableModel>((CableModel)source, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);
            PrjDb.UpsertRecord<CableModel>((CableModel)source, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);
        }
    }

    public static async Task SaveAllPowerCablesAsync(ListManager listManager)
    {
        await Task.Run(() => {
            foreach (var cable in listManager.CableList) {
                PrjDb.UpsertRecord<CableModel>((CableModel)cable, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);
            }
        });
    }

    public static void OnRacewayPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            PrjDb.UpsertRecord<RacewayModel>((RacewayModel)source, GlobalConfig.RacewayTable, NoSaveLists.RacewayNoSaveList);
        }
    }

    public static void OnProtectioneDevicePropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            PrjDb.UpsertRecord<ProtectionDeviceModel>((ProtectionDeviceModel)source, GlobalConfig.ProtectionDeviceTable, NoSaveLists.ProtectionDeviceNoSaveList);
        }
    }

    //Save Dteq Get Id
    public static int SaveDteqGetId(IDteq iDteq)
    {
        int id = 0;
        if (iDteq.GetType() == typeof(DteqModel)) {
           var model = (DteqModel)iDteq;
           id = PrjDb.InsertRecordGetId(model, GlobalConfig.DteqTable, NoSaveLists.DteqNoSaveList);
        }
        else if (iDteq.GetType() == typeof(XfrModel)) {
            var model = (XfrModel)iDteq;
            id = PrjDb.InsertRecordGetId(model, GlobalConfig.XfrTable, NoSaveLists.DteqNoSaveList);
        }
        else if (iDteq.GetType() == typeof(SwgModel)) {
            var model = (SwgModel)iDteq;
            id = PrjDb.InsertRecordGetId(model, GlobalConfig.SwgTable, NoSaveLists.DteqNoSaveList);
        }
        else if (iDteq.GetType() == typeof(MccModel)) {
            var model = (MccModel)iDteq;
            id = PrjDb.InsertRecordGetId(model, GlobalConfig.MccTable, NoSaveLists.DteqNoSaveList);
        }
        return id;
    }

    

    

    public static int SaveLoadGetId(LoadModel load)
    {
        int id = PrjDb.InsertRecordGetId(load, GlobalConfig.LoadTable, NoSaveLists.LoadNoSaveList);
        return id;
    }
    public static int SavePowerCableGetId(ICable cable)
    {
        int id = 0;
        if (cable.GetType() == typeof(CableModel)) {
            var cableModel = (CableModel)cable;
            id= PrjDb.InsertRecordGetId(cableModel, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);
        }
        return id;
    }

    //Delete Dteq
    public static void DeleteDteq(IDteq iDteq)
    {
        if (iDteq.GetType() == typeof(DteqModel)) {
            var model = (DteqModel)iDteq;
            PrjDb.DeleteRecord(GlobalConfig.DteqTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(XfrModel)) {
            var model = (XfrModel)iDteq;
            PrjDb.DeleteRecord(GlobalConfig.XfrTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(SwgModel)) {
            var model = (SwgModel)iDteq;
            PrjDb.DeleteRecord(GlobalConfig.SwgTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(MccModel)) {
            var model = (MccModel)iDteq;
            PrjDb.DeleteRecord(GlobalConfig.MccTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(DpnModel)) {
            var model = (DpnModel)iDteq;
            PrjDb.DeleteRecord(GlobalConfig.DpnTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(SplitterModel)) {
            var model = (SplitterModel)iDteq;
            PrjDb.DeleteRecord(GlobalConfig.SplitterTable, model.Id);
        }
    }


   
    public static async Task UpsertComponentAsync(ComponentModelBase component)
    {
     
        //commented out too allow inserting new PD's
        //if (GlobalConfig.Importing == true) return;
        try {
            // removed await to test speed
            await Task.Run(() => {

                if (component.GetType() == typeof(ComponentModel)) {
                    var model = (ComponentModel)component;
                    PrjDb.UpsertRecord(model, GlobalConfig.ComponentTable, NoSaveLists.CompNoSaveList);
                }
                else if (component.GetType() == typeof(ProtectionDeviceModel)) {
                    var model = (ProtectionDeviceModel)component;
                    PrjDb.UpsertRecord(model, GlobalConfig.ProtectionDeviceTable, NoSaveLists.ProtectionDeviceNoSaveList);
                }
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }

    }

    public static void DeleteComponent(ComponentModelBase component)
    {
        if (component == null ) return;
        
        if (component.GetType() == typeof(ComponentModel)) {
            var model = (ComponentModel)component;
            PrjDb.DeleteRecord(GlobalConfig.ComponentTable, model.Id);
        }
        else if (component.GetType() == typeof(ProtectionDeviceModel)) {
            var model = (ProtectionDeviceModel)component;
            PrjDb.DeleteRecord(GlobalConfig.ProtectionDeviceTable, model.Id);
        }
    }
    public static void DeleteProtectionDevice(ProtectionDeviceModel pd)
    {
        if (pd == null) {
            return;
        }
        PrjDb.DeleteRecord(GlobalConfig.ProtectionDeviceTable, pd.Id);
    }

    public static async Task UpsertLcsAsync(LocalControlStationModel lcs)
    {
        try {
            if (GlobalConfig.Importing == true) return;
            await Task.Run(() => {
                PrjDb.UpsertRecord(lcs, GlobalConfig.LcsTable, NoSaveLists.LcsNoSaveList);
            });
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ;
        }
    }
    public static void DeleteLcs(LocalControlStationModel lcs)
    {
        if (lcs == null) {
            return;
        }
        PrjDb.DeleteRecord(GlobalConfig.LcsTable, lcs.Id);
    }

    public static async Task UpsertCableAsync(CableModel cable)
    {

        try {
            if (GlobalConfig.Importing == true) return;
            await Task.Run(() => {
                PrjDb.UpsertRecord(cable, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);
            });
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    public static async Task UpsertAreaAsync(AreaModel area)
    {
        try {
            if (GlobalConfig.Importing == true) return;
            await Task.Run(() => {
                PrjDb.UpsertRecord<AreaModel>(area, GlobalConfig.AreaTable, NoSaveLists.AreaNoSaveList);
            });
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    internal static void DeleteLoadCircuit(LoadCircuit loadCircuit)
    {
        DaManager.PrjDb.DeleteRecord(GlobalConfig.LoadCircuitTable, loadCircuit.Id);
    }
    internal static void DeleteRaceway(RacewayModel raceway)
    {
        DaManager.PrjDb.DeleteRecord(GlobalConfig.RacewayTable, raceway.Id);
    }

    internal static void DeleteRacewaySegment(RacewayRouteSegment racewayRouteSegment)
    {
        DaManager.PrjDb.DeleteRecord(GlobalConfig.RacewayRouteSegmentsTable, racewayRouteSegment.Id);
    }




    public static void OnTypeModelPropertyUpdated(object source, EventArgs e)
    {

        try {

            if (DaManager.GettingRecords) return;

            var propModel = (PropertyModelBase)source;
            DaManager.UpsertPropModelAsync(propModel);
            
        }
        catch (Exception) {
            throw;
        }
    }
    public static async Task UpsertPropModelAsync(PropertyModelBase propModel)
    {
        try {
            await Task.Run(() => {
                UpsertPropModel(propModel);
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }
    public static void UpsertPropModel(PropertyModelBase propModel)
    {
        try {
            //if (GlobalConfig.Importing == true) return;

            if (propModel.GetType() == typeof(BreakerPropModel)) {
                var model = (BreakerPropModel)propModel;
                PrjDb.UpsertRecord(model, GlobalConfig.BreakerPropsTable, NoSaveLists.PropModelNoSaveList);
            }
            else if (propModel.GetType() == typeof(DisconnectPropModel)) {
                var model = (DisconnectPropModel)propModel;
                PrjDb.UpsertRecord(model, GlobalConfig.DisconnectPropsTable, NoSaveLists.PropModelNoSaveList);
            }
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    public static async Task DeletePropModelAsync(PropertyModelBase propModel)
    {
        try {
            await Task.Run(() => {
                DeletePropModel(propModel);
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }
    public static void DeletePropModel(PropertyModelBase propModel)
    {
        try {
            if (GlobalConfig.Importing == true) return;
            if (propModel == GlobalConfig.DteqDeleted) { return; }

            if (propModel.GetType() == typeof(BreakerPropModel)) {
                var model = (BreakerPropModel)propModel;
                PrjDb.DeleteRecord(GlobalConfig.BreakerPropsTable, propModel.Id);
            }
            else if (propModel.GetType() == typeof(DisconnectPropModel)) {
                var model = (DisconnectPropModel)propModel;
                PrjDb.DeleteRecord(GlobalConfig.DisconnectPropsTable, propModel.Id);
            }
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }









}

