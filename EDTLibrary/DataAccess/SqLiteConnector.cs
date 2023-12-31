﻿using Dapper;
using EDTLibrary.ErrorManagement;
using EDTLibrary.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace EDTLibrary.DataAccess
{
    public class SQLiteConnector : IDaConnector
    {
        public string ConString { get; set; }

        public SQLiteConnector(string dbFileName)
        {
            try {
                ConString = $"DataSource= {dbFileName}; foreign keys=true; PRAGMA foreign_keys = ON;";
            }
            catch (Exception ex) {
                throw;
            }
        }

        #region GenericSQLite Calls
        /// <summary>
        ///Maps a SQLite table to a list of Class objects
        /// </summary>
        /// <typeparam name="T">Maps a SQLite table to type T objects </typeparam>
        /// <param name="tableName">SQLIte table name to map</param>
        /// <param name="columnName">Optional column name to apply filter </param>
        /// <param name="filterText">Optional filter to apply to column </param>
        /// <returns>List of Type T with properties from tableName</returns>
        /// 

        public T GetRecordById<T>(string tableName, int id)
        {
            DynamicParameters dP = new DynamicParameters();
            dP.Add("@Id", $"{id}");

            using (SQLiteConnection cnn = new SQLiteConnection(ConString)) {
                return cnn.QuerySingleOrDefault<T>($"SELECT * FROM {tableName} WHERE Id = @Id", dP);
            }
        }

        public ObservableCollection<T> GetRecords<T>(string tableName, string columnName = "", string filterText = "") //where T : class, new()
        {

            DynamicParameters dP = new DynamicParameters();
            List<T> queryResult = new List<T>();
            ObservableCollection<T> output = new ObservableCollection<T>();

            using (SQLiteConnection cnn = new SQLiteConnection(ConString)) {

                //returns all columns from table with column filter
                if (columnName != "" && filterText != "") {
                    dP.Add("@filterText", $"%{filterText}%");
                    queryResult = (List<T>)cnn.Query<T>($"SELECT * FROM {tableName} WHERE {columnName} LIKE @filterText", dP);
                    output = new ObservableCollection<T>(queryResult);
                    return output;
                }
                //returns entire table
                else {
                    try {
                        queryResult = (List<T>)cnn.Query<T>($"SELECT * FROM {tableName}", dP);
                        output = new ObservableCollection<T>(queryResult);
                        return output;
                    }
                    catch (Exception ex) {
                        ex.Data.Add("UserMessage", $"Error getting records from: {tableName}");
                        ErrorHelper.SendExeptionMessage(ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Builds an SQL command string with Dynamic parameters to insert an obejct as a record. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classObject"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>

        /// <summary>
        /// Inserts a record from an object with a list of properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classObject"></param>
        /// <param name="tableName"></param>
        /// <param name="propertiesToIgnore"></param>
        /// <returns></returns>
        public Tuple<bool, string> InsertRecord<T>(T classObject, string tableName, List<string> propertiesToIgnore) where T : class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConString)) {
                StringBuilder sb = new StringBuilder();
                var objectProperties = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"INSERT INTO {tableName} (");

                //Column Names
                foreach (var prop in objectProperties) {
                    sb.Append($"{prop.Name}, ");
                }

                // removes any properties in prop list
                string input;
                string pattern;
                string replace = "";
                foreach (var prop in propertiesToIgnore) {
                    input = sb.ToString();
                    pattern = $@"\b{prop}, ";
                    replace = "";
                    sb.Clear();
                    sb.Append(Regex.Replace(input, pattern, ""));

                    //sb.Replace($"{prop}, ", "");
                }

                input = sb.ToString();
                pattern = @"\bId,";
                replace = "";
                sb.Clear();
                sb.Append(Regex.Replace(input, pattern, ""));

                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(") VALUES (");

                //Parameters (@ColumnNames)
                foreach (var prop in objectProperties) {
                    sb.Append($"@{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }

                // removes any properties in prop list



                foreach (var prop in propertiesToIgnore) {
                    input = sb.ToString();
                    pattern = $@"(@{prop}, )";
                    replace = "";
                    sb.Clear();
                    sb.Append(Regex.Replace(input, pattern, ""));

                    //sb.Replace($"@{prop}, ", "");
                }
                sb.Replace("@Id,", "");
                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(")");
                sb.Replace("( ", "(");

                //cnn.Execute(sb.ToString() + ";", classObject);
                //return new Tuple<bool, string>(true, "");

                try {
                    cnn.Execute(sb.ToString() + ";", classObject);
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex) {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Error: \n{ex.Message}\n\n" +
                        $"Query: \n{sb}\n\n" +
                        $"Details: \n\n {ex}"); ;
                }
            }
        }
        public int InsertRecordGetId<T>(T classObject, string tableName, List<string> propertiesToIgnore) where T : class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConString)) {
                SQLiteCommand cmd = new SQLiteCommand();
                StringBuilder sb = new StringBuilder();
                var objectProperties = classObject.GetType().GetProperties();


                //Build query string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"INSERT INTO {tableName} (");

                //Column Names
                foreach (var prop in objectProperties) {
                    sb.Append($"{prop.Name}, ");
                }

                // removes properties to Ignore
                string input;
                string pattern;
                string replace = "";
                foreach (var prop in propertiesToIgnore) {
                    input = sb.ToString();
                    pattern = $@"\b{prop}, ";
                    replace = "";
                    sb.Clear();
                    sb.Append(Regex.Replace(input, pattern, ""));

                    //sb.Replace($"{prop}, ", "");
                }

                input = sb.ToString();
                pattern = @"\bId,";
                replace = "";
                sb.Clear();
                sb.Append(Regex.Replace(input, pattern, ""));

                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(") VALUES (");

                //Parameters (@ColumnNames)
                foreach (var prop in objectProperties) {
                    sb.Append($"@{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }

                // removes properties to ignore
                foreach (var prop in propertiesToIgnore) {
                    input = sb.ToString();
                    pattern = $@"(@{prop}, )";
                    replace = "";
                    sb.Clear();
                    sb.Append(Regex.Replace(input, pattern, ""));

                    //sb.Replace($"@{prop}, ", "");
                }
                sb.Replace("@Id,", "");
                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(")");

                SQLiteConnection conn = new SQLiteConnection(ConString);
                SQLiteCommand cmdId = new SQLiteCommand();

                try {
                    cnn.Execute(sb.ToString() + ";", classObject);
                    object sqlId;
                    sqlId = cnn.ExecuteScalar($"SELECT MAX(rowid) FROM {tableName}");
                    int objectId = Convert.ToInt32(sqlId);

                    return objectId;
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", "SQL Query Error\nQuery:\n" + sb.ToString());
                    ErrorHelper.SendExeptionMessage(ex);
                    throw;
                }
            }
        }

        public void UpsertRecord<T>(T classObject, string tableName, List<string> propertiesToIgnore, [CallerMemberName] string callerMethod = "") where T : class, new()
        {
            //if (DaManager.Importing==true) {
            //    return;
            //}
            using (IDbConnection cnn = new SQLiteConnection(ConString)) {
                StringBuilder sb = new StringBuilder();
                var objectProperties = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                int id = -1;
                //Build query string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"INSERT INTO {tableName} (");
                //Column Names
                foreach (var prop in objectProperties) {
                    sb.Append($"{prop.Name}, ");
                }

                //File.WriteAllTextAsync("C:\\Users\\pdeau\\OneDrive\\Desktop\\WriteText.txt", sb.ToString().Replace(",",Environment.NewLine));

                // removes properties to ignore
                string tag = "";
                string input;
                string pattern;
                string replace = "";
                foreach (var prop in propertiesToIgnore) {
                    input = sb.ToString();
                    pattern = $@"\b{prop}, ";
                    replace = "";
                    sb.Clear();
                    sb.Append(Regex.Replace(input, pattern, ""));

                }

               

                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(") VALUES (");

                //Parameters (@ColumnNames)
                foreach (var prop in objectProperties) {
                    sb.Append($"@{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                    if (prop.Name == "Id") {
                        id = (int)prop.GetValue(classObject);
                    }
                    //if (prop.Name == "Tag") {
                    //    tag = (string)prop.GetValue(classObject);
                    //}
                }

                // removes properties to ignore with @
                foreach (var prop in propertiesToIgnore) {
                    input = sb.ToString();
                    pattern = $@"(@{prop}, )";
                    replace = "";
                    sb.Clear();
                    sb.Append(Regex.Replace(input, pattern, ""));

                    //sb.Replace($"@{prop}, ", "");
                }
                //sb.Replace("@Id,", "");
                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(") ");


                sb.Append($"ON CONFLICT(Id) DO UPDATE SET ");

                foreach (var prop in objectProperties) {
                    sb.Append($"{prop.Name} = @{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }

                // removes properties to ignore
                foreach (var prop in propertiesToIgnore) {

                    sb.Replace($"{prop} = @{prop}, ", "");
                }

                sb.Replace("Id = @Id,", "");
                sb.Replace("  ", " ");

                sb.Replace(", ", "", sb.Length - 2, 2);
                sb.Append(" WHERE Id = @Id");

                //if (tag=="CDP-01") {
                //    tag = tag;
                //}

                try {
                    cnn.Execute("" + sb.ToString(), classObject);
                    var debug = tag;
                    ErrorHelper.Log($"SqLiteConnector.Upsert - Success Tag: {tag},      Caller: {callerMethod}");


                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", "SQL Query Error\nQuery:\n" + sb.ToString());
                    var debugHelper = classObject;

                    //readonyl database error

                    string message = "Error";

                 

                    if (ex.Message.Contains("readonly")) {
                        message = $"The project file may be saved in a folder that does not have write " +
                                        "priveliges enabled, like 'Program Files'. Move the file to another " +
                                        "location and reopen the project.\n\n\n" +
                                        $"Error Details: {ex.Message}";

                        ErrorHelper.NotifyUserError(message);

                    }
                    else {
                        //ErrorHelper.ShowErrorMessage(ex);
                    }
                    ErrorHelper.Log($"SqLiteConnector.Upsert - Failure Tag: {tag},      Caller: {callerMethod}");
                    ErrorHelper.SendExeptionMessage(ex);

                    //throw;
                }
            }
        }

        public void UpdateRecordSaveList<T>(T classObject, string tableName, List<string> propertiesToSave, [CallerMemberName] string callerMethod = "") where T : class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConString).OpenAndReturn()) {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                //UPSERT INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"UPDATE {tableName} SET ");

                //Column Names
                foreach (var prop in propertiesToSave) {
                    var objectProperty = props.FirstOrDefault(p => p.Name == prop);
                    sb.Append($"{prop} = @{prop}, ");
                    cmd.Parameters.AddWithValue($"@{prop}", objectProperty.GetValue(classObject));
                }
                sb.Replace(", ", "", sb.Length - 2, 2);
                sb.Append(" WHERE Id = @Id");

                var query = sb.ToString();
                try {
                    cnn.Execute("" + sb.ToString(), classObject);
                    return;
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", "SQL Query Error\nQuery:\n" + sb.ToString());
                    var debugHelper = classObject;

                    //readonyl database error

                    string message = "Error";

                    if (ex.Message.Contains("readonly")) {
                        message = $"The project file may be saved in a folder that does not have write " +
                                        "priveliges enabled, like 'Program Files'. Move the file to another " +
                                        "location and reopen the project.\n\n\n" +
                                        $"Error Details: {ex.Message}";

                        ErrorHelper.NotifyUserError(message);
                    }
                    else {
                        //ErrorHelper.ShowErrorMessage(ex);
                    }
                    ErrorHelper.Log($"SqLiteConnector.Upsert - Failure Tag: none,    Caller: {callerMethod}");
                    ErrorHelper.SendExeptionMessage(ex);
                }
            }
        }

        public Tuple<bool, string> UpdateRecord<T>(T classObject, string tableName) where T : class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConString).OpenAndReturn()) {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                //UPSERT INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"UPDATE {tableName} SET ");

                //Column Names
                foreach (var prop in props) {
                    sb.Append($"{prop.Name} = @{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }
                sb.Replace("Id = @Id,", "");
                //TODO - need to update "UpdateRecord" with SaveList
                sb.Replace("AssignedLoads = @AssignedLoads,", "");
                sb.Replace("InLineComponents = @InLineComponents,", "");
                sb.Replace(", ", "", sb.Length - 2, 2);
                sb.Append(" WHERE Id = @Id");

                try {
                    cnn.Execute("" + sb.ToString(), classObject);
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex) {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Error: \n{ex.Message}\n\n" +
                        $"Query: \n{sb}\n\n" +
                        $"Details: \n\n {ex}"); ;
                }
            }
        }


        public void UpdateSetting(string settingName, string settingValue)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConString).OpenAndReturn()) {
                StringBuilder sb = new StringBuilder();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                sb.Append($"UPDATE ProjectSettings SET Value = @Value");
                cmd.Parameters.AddWithValue($"@Value", settingValue);

                sb.Append(" WHERE Name = @Name");
                cmd.Parameters.AddWithValue($"@Name", settingName);

                try {
                    cnn.Execute("" + sb.ToString());
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", "SQL Query Error\nQuery:\n" + sb.ToString());
                    ErrorHelper.SendExeptionMessage(ex);
                    throw;
                }
            }
        }
        public void DeleteRecord(string tableName, int id)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConString)) {
                try {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = ($"DELETE FROM {tableName} WHERE Id = @Id");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", $"Error deleting Id: {id}    From: {tableName}");
                    throw;
                }
            }
        }
        public async Task DeleteRecordAsync(string tableName, int id)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConString)) {
                try {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = ($"DELETE FROM {tableName} WHERE Id = @Id");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", $"Error deleting Id: {id}    From: {tableName}");
                    throw;
                }
            }
        }

        public void DeleteAllRecords(string tableName)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConString)) {
                try {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = ($"DELETE FROM {tableName}");
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", $"Error deleting All    From: {tableName}");
                    throw;
                }
            }
        }


        public DataTable GetDataTable(string tableName)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConString)) {
                DataTable dt = new DataTable();

                try {

                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"SELECT * FROM {tableName}", ConString);
                    if (dataAdapter == null) return dt;
                    dataAdapter.Fill(dt);
                    dt.TableName = tableName;
                    return dt;
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", $"Error getting DataTable: {tableName}");
                    throw;
                }
            }
        }

        public void SaveDataTable(DataTable dataTable, string tableName)
        {
            int numRowsUpdated = 0;

            using (IDbConnection cnn = new SQLiteConnection(ConString)) {

                try {
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"SELECT * FROM {tableName}", ConString);
                    SQLiteCommandBuilder scb = new SQLiteCommandBuilder(dataAdapter);
                    DataTable dt = new DataTable();
                    dataAdapter.Update(dataTable);
                }
                catch (Exception ex) {
                    ex.Data.Add("UserMessage", $"Error saving DataTable: {dataTable}    To Db table:{tableName}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Puts all the name of each table in the Db into an ArrayList
        /// </summary>
        /// <returns></returns>
        public ArrayList GetListOfTablesNamesInDb()
        {
            ArrayList arrayList = new ArrayList();

            // executes query that select names of all tables in master table of the database
            String query = "SELECT name FROM sqlite_master " +
                    "WHERE type = 'table' " +
                    "ORDER BY 1";
            try {
                DataTable table = QueryToDataTable(query);

                // Return all table names in the ArrayList

                foreach (DataRow row in table.Rows) {
                    arrayList.Add(row.ItemArray[0].ToString()); //ItemArray[0] is the name of the table
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return arrayList;
        }
        public DataTable QueryToDataTable(string query)
        {
            try {
                DataTable dt = new DataTable();
                using (SQLiteConnection cnn = new SQLiteConnection(ConString)) {
                    cnn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn)) {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader()) {
                            dt.Load(rdr);//loads into DataTable
                            return dt;
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        #endregion
    }
}
