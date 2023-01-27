using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EDTLibrary.DataAccess;

public interface IDaConnector
{
    string ConString { get; set; }

    void DeleteAllRecords(string tableName);
    void DeleteRecord(string tableName, int id);
    Task DeleteRecordAsync(string tableName, int id);
    DataTable GetDataTable(string tableName);
    ArrayList GetListOfTablesNamesInDb();
    T GetRecordById<T>(string tableName, int id);
    ObservableCollection<T> GetRecords<T>(string tableName, string columnName = "", string filterText = "");
    Tuple<bool, string> InsertRecord<T>(T classObject, string tableName, List<string> propertiesToIgnore) where T : class, new();
    int InsertRecordGetId<T>(T classObject, string tableName, List<string> propertiesToIgnore) where T : class, new();
    DataTable QueryToDataTable(string query);
    void SaveDataTable(DataTable dataTable, string tableName);
    Tuple<bool, string> UpdateRecord<T>(T classObject, string tableName) where T : class, new();
    void UpdateSetting(string settingName, string settingValue);
    void UpsertRecord<T>(T classObject, string tableName, List<string> propertiesToIgnore, [CallerMemberName] string callerMethod = "") where T : class, new();

    void UpdateRecordSaveList<T>(T classObject, string tableName, List<string> propertiesToSave, [CallerMemberName] string callerMethod = "") where T : class, new();
}