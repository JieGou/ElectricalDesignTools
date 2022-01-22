using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace EDTLibrary.DataAccess
{
    public class SQLiteConnector
    {
        public string conString { get; set; }

        public SQLiteConnector(string dbFileName)
        {
            try
            {
                conString = $"DataSource= {dbFileName}; foreign keys=true; PRAGMA foreign_keys = ON;";
            }
            catch (ArgumentException)
            {
            }
        }
        #region GeneralSQLite Calls
        /// <summary>
        ///Maps a SQLite table to a list of Class objects
        /// </summary>
        /// <typeparam name="T">Maps a SQLite table to type T objects </typeparam>
        /// <param name="tableName">SQLIte table name to map</param>
        /// <param name="columnName">Optional column name to apply filter </param>
        /// <param name="filterText">Optional filter to apply to column </param>
        /// <returns>List of Type T with properties from tableName</returns>
        public List<T> GetRecords<T>(string tableName, string columnName = "", string filterText = "") //where T : class, new()
        {

            DynamicParameters dP = new DynamicParameters();
            List<T> output = new List<T>();
            using (SQLiteConnection cnn = new SQLiteConnection(conString)) {

                //returns all columns from table with column filter
                if (columnName != "" && filterText != "") {
                    dP.Add("@filterText", $"%{filterText}%");
                    output = (List<T>)cnn.Query<T>($"SELECT * FROM {tableName} WHERE {columnName} LIKE @filterText", dP);

                    return output;
                }
                //returns entire table
                else {
                    try {
                        output = (List<T>)cnn.Query<T>($"SELECT * FROM {tableName}", dP);
                    }
                    catch { }

                    return output;
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
        /// <param name="propertyList"></param>
        /// <returns></returns>
        public Tuple<bool, string> InsertRecord<T>(T classObject, string tableName, List<string> propertyList) where T : class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(conString)) {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"INSERT INTO {tableName} (");

                //Column Names
                foreach (var prop in props) {
                    sb.Append($"{prop.Name}, ");
                }

                // removes any properties in prop list
                foreach (var prop in propertyList) {
                    sb.Replace($"{prop}, ", "");
                }

                sb.Replace("Id,", "");
                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(") VALUES (");

                //Parameters (@ColumnNames)
                foreach (var prop in props) {
                    sb.Append($"@{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }

                // removes any properties in prop list
                foreach (var prop in propertyList) {
                    sb.Replace($"@{prop}, ", "");
                }
                sb.Replace("@Id,", "");
                sb.Replace(",", "", sb.Length - 2, 2);
                sb.Replace(" ", "", sb.Length - 2, 2);
                sb.Append(")");

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

        public Tuple<bool, string> UpsertRecord<T>(T classObject, string tableName, List<string> propertyList) where T: class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(conString)) {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                void BuildPropAndParamList()
                {                   
                    //Column Names
                    foreach (var prop in props) {
                        sb.Append($"{prop.Name}, ");
                    }

                    // removes any properties in prop list
                    foreach (var prop in propertyList) {
                        sb.Replace($"{prop}, ", "");
                    }

                    sb.Replace("Id,", "");
                    sb.Replace(",", "", sb.Length - 2, 2);
                    sb.Replace(" ", "", sb.Length - 2, 2);
                    sb.Append(") VALUES (");

                    //Parameters (@ColumnNames)
                    foreach (var prop in props) {
                        sb.Append($"@{prop.Name}, ");
                        cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                    }

                    // removes any properties in prop list
                    foreach (var prop in propertyList) {
                        sb.Replace($"@{prop}, ", "");
                    }
                    sb.Replace("@Id,", "");
                    sb.Replace(",", "", sb.Length - 2, 2);
                    sb.Replace(" ", "", sb.Length - 2, 2);
                    sb.Append(")");
                }

                //Build query string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"INSERT INTO {tableName} (");
                BuildPropAndParamList();


                sb.Append($" ON CONFLICT DO UPDATE SET ");

                foreach (var prop in props) {
                    sb.Append($"{prop.Name} = @{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }

                foreach (var prop in propertyList) {
                    sb.Replace($"{prop} = @{prop}, ", "");
                }

                sb.Replace("Id = @Id,", "");
                sb.Replace(", ", "", sb.Length - 2, 2);
                sb.Append(" WHERE Id = @Id");


                try {
                    cnn.Execute("" + sb.ToString(), classObject);
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex) {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Error: \n\n{ex.Message}\n\n\n" +
                        $"Query: \n\n{sb}\n\n\n" +
                        $"Details: \n\n {ex}"); ;
                }
            }
        }

        public Tuple<bool, string> UpdateRecord<T>(T classObject, string tableName) where T : class, new()
        {
            using (IDbConnection cnn = new SQLiteConnection(conString).OpenAndReturn())
            {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                //UPSERT INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"UPDATE {tableName} SET ");

                //Column Names
                foreach (var prop in props)
                {
                    sb.Append($"{prop.Name} = @{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }
                sb.Replace("Id = @Id,", "");
                //TODO - need to update "UpdateRecord" with SaveList
                sb.Replace("AssignedLoads = @AssignedLoads,", "");
                sb.Replace("InLineComponents = @InLineComponents,", "");
                sb.Replace(", ", "", sb.Length - 2, 2);
                sb.Append(" WHERE Id = @Id");

                try
                {
                    cnn.Execute("" + sb.ToString(), classObject);
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex)
                {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Error: \n{ex.Message}\n\n" +
                        $"Query: \n{sb}\n\n" +
                        $"Details: \n\n {ex}"); ;
                }
            }
        }

        public Tuple<bool, string> UpdateSetting(string settingName, string settingValue)
        {
            using (IDbConnection cnn = new SQLiteConnection(conString).OpenAndReturn())
            {
                StringBuilder sb = new StringBuilder();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build query string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"UPDATE ProjectSettings SET Value = @Value");
                cmd.Parameters.AddWithValue($"@Value", settingValue);

                sb.Append(" WHERE Name = @Name");
                cmd.Parameters.AddWithValue($"@Name", settingName);

                try
                {
                    cnn.Execute("" + sb.ToString());
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex)
                {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Error: \n{ex.Message}\n\n" +
                        $"Query: \n{sb}\n\n" +
                        $"Details: \n\n {ex}"); ;
                }
            }
        }

        public void DeleteRecord(string tableName, int id)
        {
            using (SQLiteConnection con = new SQLiteConnection(conString))
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = ($"DELETE FROM {tableName} WHERE Id = @Id");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error thrown from SqLiteDataAccess: DeleteRecord: " + ex);
                }
            }
        }

        public void DeleteAllRecords(string tableName)
        {
            using (SQLiteConnection con = new SQLiteConnection(conString))
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = ($"DELETE FROM {tableName}");
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error thrown from SqLiteDataAccess: DeleteRecord: " + ex);
                }
            }
        }

        public List<string> ColumnToList(string columnName, string tableName)
        {
            using (IDbConnection cnn = new SQLiteConnection(conString))
            {
                var output = cnn.Query<string>($"SELECT {columnName} FROM {tableName}", new DynamicParameters()); //
                return output.ToList();
            }
        }

        public DataTable GetDataTable(string tableName)
        {
            using (IDbConnection cnn = new SQLiteConnection(conString)) {
                DataTable dt = new DataTable();

                try {

                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"SELECT * FROM {tableName}", conString);
                    dataAdapter.Fill(dt);
                    dt.TableName = tableName;
                }
                catch { }
                return dt;
            }
        }

        public void SaveDataTable(DataTable dataTable, string tableName)
        {
            int numRowsUpdated = 0;

            using (IDbConnection cnn = new SQLiteConnection(conString)) {

                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"SELECT * FROM {tableName}", conString);
                SQLiteCommandBuilder scb = new SQLiteCommandBuilder(dataAdapter);
                DataTable dt = new DataTable();
                dataAdapter.Update(dataTable);
            }
        }

        /// <summary>
        /// Puts all the name of each table in the Db into an ArrayList
        /// </summary>
        /// <returns></returns>
        public ArrayList GetListOfTablesNamesInDb()
        {
            ArrayList list = new ArrayList();

            // executes query that select names of all tables in master table of the database
            String query = "SELECT name FROM sqlite_master " +
                    "WHERE type = 'table' " +
                    "ORDER BY 1";
            try{
                DataTable table = QueryToDataTable(query);

                // Return all table names in the ArrayList

                foreach (DataRow row in table.Rows){
                    list.Add(row.ItemArray[0].ToString()); //ItemArray[0] is the name of the table
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
            return list;
        }
            public DataTable QueryToDataTable(string query)
        {
            try {
                DataTable dt = new DataTable();
                using (SQLiteConnection cnn = new SQLiteConnection(conString))
                {
                    cnn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, cnn))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            dt.Load(rdr);//loads into DataTable
                            return dt;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        #endregion

        #region Distribution Equipment


        #endregion

    }
}
