using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeModels;
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

    public static bool Importing { get; set; } = false;
    public static bool GettingRecords { get; set; } = true;
    public static bool AddingEquipment { get; set; } = true;

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

    #region General
    public static void DeleteAllEquipmentRecords()
    {
        //Delete records
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.DteqTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.XfrTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.SwgTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.MccTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.LoadTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.CableTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.ComponentTable);
        DaManager.prjDb.DeleteAllRecords(GlobalConfig.LcsTable);
    }

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

    //Upsert Dteq
    public static async Task UpsertDteqAsync(IDteq iDteq)
    {
        try {
            // removed await to test speed
            await Task.Run(() => {
                if (GlobalConfig.Importing == true) return;
                if (iDteq == GlobalConfig.DteqDeleted) { return; }

                if (iDteq.GetType() == typeof(DteqModel)) {
                    var model = (DteqModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.DteqTable, SaveLists.DteqNoSaveList);
                }
                else if (iDteq.GetType() == typeof(XfrModel)) {
                    var model = (XfrModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.XfrTable, SaveLists.DteqNoSaveList);
                }
                else if (iDteq.GetType() == typeof(SwgModel)) {
                    var model = (SwgModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.SwgTable, SaveLists.DteqNoSaveList);
                }
                else if (iDteq.GetType() == typeof(MccModel)) {
                    var model = (MccModel)iDteq;
                    prjDb.UpsertRecord(model, GlobalConfig.MccTable, SaveLists.DteqNoSaveList);
                }
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }
    public static void OnLoadPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            DaManager.UpsertLoadAsync((LoadModel)source);
        }
    }

    private static async Task UpsertLoadAsync(LoadModel load)
    {
        try {
            //removed await to test speed
            await Task.Run(() => {
                if (GlobalConfig.Importing == true) return;
                prjDb.UpsertRecord(load, GlobalConfig.LoadTable, SaveLists.LoadNoSaveList);
            });
        }

        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }
    public static void OnComponentPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            DaManager.UpsertComponent((ComponentModel)source);
        }
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

            prjDb.UpsertRecord(drive, GlobalConfig.DriveTable, SaveLists.CompNoSaveList);
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw ;
        }
    }

    public static void OnLcsPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            DaManager.UpsertLcs((LocalControlStationModel)source);
        }
    }

    public static void OnAreaPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            prjDb.UpsertRecord<AreaModel>((AreaModel)source, GlobalConfig.AreaTable, SaveLists.AreaNoSaveList);
        }
    }

    public static void OnPowerCablePropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            prjDb.UpsertRecord<CableModel>((CableModel)source, GlobalConfig.CableTable, SaveLists.PowerCableNoSaveList);
        }
    }

    //Save Get Id
    public static int SaveDteqGetId(IDteq iDteq)
    {
        int id = 0;
        if (iDteq.GetType() == typeof(DteqModel)) {
           var model = (DteqModel)iDteq;
           id = prjDb.InsertRecordGetId(model, GlobalConfig.DteqTable, SaveLists.DteqNoSaveList);
        }
        else if (iDteq.GetType() == typeof(XfrModel)) {
            var model = (XfrModel)iDteq;
            id = prjDb.InsertRecordGetId(model, GlobalConfig.XfrTable, SaveLists.DteqNoSaveList);
        }
        else if (iDteq.GetType() == typeof(SwgModel)) {
            var model = (SwgModel)iDteq;
            id = prjDb.InsertRecordGetId(model, GlobalConfig.SwgTable, SaveLists.DteqNoSaveList);
        }
        else if (iDteq.GetType() == typeof(MccModel)) {
            var model = (MccModel)iDteq;
            id = prjDb.InsertRecordGetId(model, GlobalConfig.MccTable, SaveLists.DteqNoSaveList);
        }
        return id;
    }

    internal static void OnDisconnectPropertyUpdated(object source, EventArgs e)
    {
        if (DaManager.GettingRecords == false) {
            DaManager.UpsertDisconnect((DisconnectModel)source);
        }
    }

    private static void UpsertDisconnect(DisconnectModel disconnect)
    {
        try {
            if (GlobalConfig.Importing == true) return;

            prjDb.UpsertRecord(disconnect, GlobalConfig.DisconnectTable, SaveLists.CompNoSaveList);
        }
        catch (Exception ex) {
            Debug.Print(ex.ToString());
            throw;
        }
    }

    public static int SaveLoadGetId(LoadModel load)
    {
        int id = prjDb.InsertRecordGetId(load, GlobalConfig.LoadTable, SaveLists.LoadNoSaveList);
        return id;
    }
    public static int SavePowerCableGetId(ICable cable)
    {
        int id = 0;
        if (cable.GetType() == typeof(CableModel)) {
            var cableModel = (CableModel)cable;
            id= prjDb.InsertRecordGetId(cableModel, GlobalConfig.CableTable, SaveLists.PowerCableNoSaveList);
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

    

    public static async Task UpsertLoadAsycn(LoadModel load)
    {

        try {
            await Task.Run(() => {
                if (GlobalConfig.Importing == true) return;
                prjDb.UpsertRecord(load, GlobalConfig.LoadTable, SaveLists.LoadNoSaveList);
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

            prjDb.UpsertRecord(component, GlobalConfig.ComponentTable, SaveLists.CompNoSaveList);
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

            prjDb.UpsertRecord(lcs, GlobalConfig.LcsTable, SaveLists.LcsNoSaveList);
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
        prjDb.UpsertRecord(cable, GlobalConfig.CableTable, SaveLists.PowerCableNoSaveList);
    }

    public static void UpsertArea(AreaModel area)
    {
        DaManager.prjDb.UpsertRecord<AreaModel>(area, GlobalConfig.AreaTable, SaveLists.AreaNoSaveList);
    }
}
