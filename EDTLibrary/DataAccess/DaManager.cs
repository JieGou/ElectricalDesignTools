using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EDTLibrary.DataAccess;

public class DaManager {

    private static string _projectFile;
    private static string _libraryFile;

    public static bool IsProjectLoaded { get; private set; }
    public static bool IsLibraryLoaded { get; private set; }


    public static IDaConnector prjDb { get; set; }
    public static IDaConnector libDb { get; set; }

    public static void SetProjectDb(IDaConnector daConnector) {
        prjDb = daConnector;
        _projectFile = daConnector.ConString;
    }

    public static void SetLibraryDb( IDaConnector daConnector) {
        libDb = daConnector;
        _libraryFile = daConnector.ConString;
    }

    public static bool GetLibraryTables()
    {
        Type libraryTablesClass = typeof(DataTables); // MyClass is static class with static properties
        DataTable dt = new DataTable();
        foreach (var prop in libraryTablesClass.GetProperties()) {
            prop.SetValue(dt, libDb.GetDataTable(prop.Name));
        }
        IsLibraryLoaded = true;
        TypeManager.GetTypeTables();
        return IsLibraryLoaded;
    }

    #region Type Tables

    
    #endregion

    public static AreaModel GetArea(int locationId)
    {
        try {
            return prjDb.GetRecordById<AreaModel>(GlobalConfig.AreaTable, locationId);

        }
        catch (Exception) {

            throw;
        }    }

    //Events

    public static void OnDteqPropertyUpdated(object source, EventArgs e)
    {

        try {
            if (GlobalConfig.GettingRecords == false) {
                IDteq dteq = (IDteq)source;
                if (dteq.Tag == GlobalConfig.Deleted ) return;
                DaManager.UpsertDteqAsync((IDteq)source);
            }
        }
        catch (Exception) {
            throw;
        }
    } 
    public static void OnLoadPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            prjDb.UpsertRecord<LoadModel>((LoadModel)source, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
        }
    }
    public static void OnComponentPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            DaManager.UpsertComponent((ComponentModel)source);
        }
    }
    public static void OnDrivePropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            DaManager.UpsertDrive((DriveModel)source);
        }
    }

    private static void UpsertDrive(DriveModel drive)
    {
        try {
            if (GlobalConfig.Importing == true) return;

            prjDb.UpsertRecord(drive, GlobalConfig.DriveTable, SaveLists.CompSaveList);
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ;
        }
    }

    public static void OnLcsPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            DaManager.UpsertLcs((LocalControlStationModel)source);
        }
    }

    public static void OnAreaPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            prjDb.UpsertRecord<AreaModel>((AreaModel)source, GlobalConfig.AreaTable, SaveLists.AreaSaveList);
        }
    }

    public static void OnPowerCablePropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            prjDb.UpsertRecord<CableModel>((CableModel)source, GlobalConfig.CableTable, SaveLists.PowerCableSaveList);
        }
    }

    //Save Get Id
    public static int SaveDteqGetId(IDteq iDteq)
    {
        int id = 0;
        if (iDteq.GetType() == typeof(DteqModel)) {
           var model = (DteqModel)iDteq;
           id = prjDb.InsertRecordGetId(model, GlobalConfig.DteqTable, SaveLists.DteqSaveList);
        }
        else if (iDteq.GetType() == typeof(XfrModel)) {
            var model = (XfrModel)iDteq;
            id = prjDb.InsertRecordGetId(model, GlobalConfig.XfrTable, SaveLists.DteqSaveList);
        }
        else if (iDteq.GetType() == typeof(SwgModel)) {
            var model = (SwgModel)iDteq;
            id = prjDb.InsertRecordGetId(model, GlobalConfig.SwgTable, SaveLists.DteqSaveList);
        }
        else if (iDteq.GetType() == typeof(MccModel)) {
            var model = (MccModel)iDteq;
            id = prjDb.InsertRecordGetId(model, GlobalConfig.MccTable, SaveLists.DteqSaveList);
        }
        return id;
    }

    internal static void OnDisconnectPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            DaManager.UpsertDisconnect((DisconnectModel)source);
        }
    }

    private static void UpsertDisconnect(DisconnectModel disconnect)
    {
        try {
            if (GlobalConfig.Importing == true) return;

            prjDb.UpsertRecord(disconnect, GlobalConfig.DisconnectTable, SaveLists.CompSaveList);
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    public static int SaveLoadGetId(LoadModel load)
    {
        int id = prjDb.InsertRecordGetId(load, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
        return id;
    }
    public static int SavePowerCableGetId(ICable cable)
    {
        int id = 0;
        if (cable.GetType() == typeof(CableModel)) {
            var cableModel = (CableModel)cable;
            id= prjDb.InsertRecordGetId(cableModel, GlobalConfig.CableTable, SaveLists.PowerCableSaveList);
        }
        return id;
    }

    //Delete Dteq
    public static void DeleteDteq(IDteq iDteq)
    {
        if (iDteq.GetType() == typeof(DteqModel)) {
            var model = (DteqModel)iDteq;
            prjDb.DeleteRecord(GlobalConfig.DteqTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(XfrModel)) {
            var model = (XfrModel)iDteq;
            prjDb.DeleteRecord(GlobalConfig.XfrTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(SwgModel)) {
            var model = (SwgModel)iDteq;
            prjDb.DeleteRecord(GlobalConfig.SwgTable, model.Id);
        }
        else if (iDteq.GetType() == typeof(MccModel)) {
            var model = (MccModel)iDteq;
            prjDb.DeleteRecord(GlobalConfig.MccTable, model.Id);
        }
    }

    //Upsert Dteq
    public static async Task UpsertDteqAsync(IDteq iDteq)
    {
        try {

            await Task.Run(() => {
                if (GlobalConfig.Importing == true) return;
                if (iDteq == GlobalConfig.DteqDeleted) { return; }

                if (iDteq.GetType() == typeof(DteqModel)) {
                    var model = (DteqModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.DteqTable, SaveLists.DteqSaveList);
                }
                else if (iDteq.GetType() == typeof(XfrModel)) {
                    var model = (XfrModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.XfrTable, SaveLists.DteqSaveList);
                }
                else if (iDteq.GetType() == typeof(SwgModel)) {
                    var model = (SwgModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.SwgTable, SaveLists.DteqSaveList);
                }
                else if (iDteq.GetType() == typeof(MccModel)) {
                    var model = (MccModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.MccTable, SaveLists.DteqSaveList);
                }
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    public static async Task UpsertLoadAsycn(LoadModel load)
    {

        try {
            await Task.Run(() => {
                if (GlobalConfig.Importing == true) return;
                prjDb.UpsertRecord(load, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
            });

        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ex;

        }
    }

    public static void UpsertComponent(ComponentModel component)
    {
        try {
            if (GlobalConfig.Importing == true) return;

            prjDb.UpsertRecord(component, GlobalConfig.ComponentTable, SaveLists.CompSaveList);
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }

    }

    public static void DeleteComponent(ComponentModel component)
    {
        if (component == null ) {
            return;
        }
        prjDb.DeleteRecord(GlobalConfig.ComponentTable, component.Id);
    }

    public static void UpsertLcs(LocalControlStationModel lcs)
    {
        try {
            if (GlobalConfig.Importing == true) return;

            prjDb.UpsertRecord(lcs, GlobalConfig.LcsTable, SaveLists.LcsSaveList);
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
        prjDb.DeleteRecord(GlobalConfig.LcsTable, lcs.Id);
    }



    public static void UpsertCable(CableModel cable)
    {
        prjDb.UpsertRecord(cable, GlobalConfig.CableTable, SaveLists.PowerCableSaveList);
    }

    public static void UpsertArea(AreaModel area)
    {
        DaManager.prjDb.UpsertRecord<AreaModel>(area, GlobalConfig.AreaTable, SaveLists.AreaSaveList);
    }
}
