using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.Cables;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Data;
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
        _projectFile = daConnector.ConString;
    }

    public static bool GetLibraryTables()
    {
        Type libraryTablesClass = typeof(DataTables); // MyClass is static class with static properties
        DataTable dt = new DataTable();
        foreach (var prop in libraryTablesClass.GetProperties()) {
            prop.SetValue(dt, libDb.GetDataTable(prop.Name));
        }
        IsLibraryLoaded = true;
        GetTypeTables();
        return IsLibraryLoaded;
    }

    #region Type Tables

    public static void GetTypeTables()
    {
        GetCableTypes();
        GetNemaTypes();
        GetVoltageTypes();
        GetAreaClassificationTypes();
        GetCECCableSizingRules();
        GetCecAmpacities();
        GetOcpdTypes();
    }

    private static void GetOcpdTypes()
    {
        TypeManager.OcpdTypes = libDb.GetRecords<OcpdType>("OcpdTypes");
    }

    private static void GetCECCableSizingRules()
    {
        TypeManager.CecCableSizingRules = libDb.GetRecords<CecCableSizingRule>("CecCableSizingRules");
    }
    private static void GetCecAmpacities()
    {
        LibraryManager.CecCableAmpacities = libDb.GetRecords<CecCableAmpacityModel>("CecCableAmpacities");
    }

    private static void GetVoltageTypes()
    {
        TypeManager.VoltageTypes = libDb.GetRecords<VoltageType>("VoltageTypes");
    }

    private static void GetCableTypes()
    {
        TypeManager.CableTypes = libDb.GetRecords<CableTypeModel>(GlobalConfig.PowerCableTypes);
    }

    private static void GetNemaTypes()
    {
        TypeManager.NemaTypes = libDb.GetRecords<NemaType>("NemaTypes");
    }

    private static void GetAreaClassificationTypes()
    {
        TypeManager.AreaClassifications = libDb.GetRecords<AreaClassificationType>("AreaClassifications");
    }
    #endregion

    public static AreaModel GetArea(int locationId)
    {
        return prjDb.GetRecordById<AreaModel>(GlobalConfig.AreaTable, locationId);
    }

    //Events

    public static void OnDteqPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords==false) {
            DaManager.UpsertDteq((IDteq)source);

        }
    } 
    public static void OnLoadPropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            prjDb.UpsertRecord<LoadModel>((LoadModel)source, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
        }
    }
    public static void OnPowerCablePropertyUpdated(object source, EventArgs e)
    {
        if (GlobalConfig.GettingRecords == false) {
            prjDb.UpsertRecord<PowerCableModel>((PowerCableModel)source, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
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

    public static int SaveLoadGetId(LoadModel load)
    {
        int id = prjDb.InsertRecordGetId(load, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
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
    public static void UpsertDteq(IDteq iDteq)
    {
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
    }

    public static void UpsertLoad(LoadModel load)
    {
        try {
            if (GlobalConfig.Importing == true) return;
            if (load == GlobalConfig.DteqDeleted) { return; }

            prjDb.UpsertRecord(load, GlobalConfig.LoadTable, SaveLists.LoadSaveList);
        }
        catch (Exception ex) {
            throw ex;
        }
     
    }

    public static void UpsertCable(PowerCableModel cable)
    {
        prjDb.UpsertRecord(cable, GlobalConfig.PowerCableTable, SaveLists.PowerCableSaveList);
    }

    
}
