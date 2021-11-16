using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteLibrary
{
    public class SQLiteDapperConnector
    {
        public string conString { get; set; }
        public SQLiteConnection cnn = new SQLiteConnection();

        public SQLiteDapperConnector(string dbFileName) {
            try {
                conString = $"DataSource= {dbFileName}; foreign keys=true; PRAGMA foreign_keys = ON;";
            }
            catch (ArgumentException e) {
            }
        }
        /// <summary>
        ///Maps a SQLite table to a list of Class objects
        /// </summary>
        /// <typeparam name="T">Maps a SQLite table to type T objects </typeparam>
        /// <param name="tableName">SQLIte table name to map</param>
        /// <param name="columnName">Optional column name to apply filter to w""</param>
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
                    return output.ToList();
                }
                //returns entire table
                else {
                    output = (List<T>)cnn.Query<T>($"SELECT * FROM {tableName}", dP);
                    return output.ToList();
                }
            }
        }

        //Builds an SQL command string with Dynamic parameters to insert an obejct as a record. 
        //Takes in the object instance and Database tablename
        public Tuple<bool, string> InsertRecord<T>(T classObject, string tableName) where T : class, new() {
            using (IDbConnection cnn = new SQLiteConnection(conString)) {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build command string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"INSERT INTO {tableName} (");

                //Column Names
                foreach (var prop in props) {
                    sb.Append($"{prop.Name},");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }
                sb.Replace("Id,", "");
                sb.Remove(sb.Length - 1, 1);
                sb.Append(") VALUES (");

                //Parameters (@ColumnNames)
                foreach (var prop in props) {
                    sb.Append($"@{prop.Name},");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }
                sb.Replace("@Id,", "");
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");

                try {
                    cnn.Execute(sb.ToString(), classObject);
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex) {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Exception thrown: \n\n{ex}");
                }
            }
        }

        public Tuple<bool, string> UpdateRecord<T>(T classObject, string tableName) where T : class, new() {
            using (IDbConnection cnn = new SQLiteConnection(conString).OpenAndReturn()) {
                StringBuilder sb = new StringBuilder();
                var props = classObject.GetType().GetProperties();
                SQLiteCommand cmd = new SQLiteCommand();

                //Build command string: 
                //INSER INTO tableName (Col1, Col2,..) VALUES (@Col1, @Col2,..)
                sb.Append($"UPDATE {tableName} SET ");

                //Column Names
                foreach (var prop in props) {
                    sb.Append($"{prop.Name} = @{prop.Name}, ");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(classObject));
                }
                sb.Replace("Id = @Id,", "");
                sb.Replace("AssignedLoads = @AssignedLoads,", "");
                sb.Replace("InLineComponents = @InLineComponents,", "");
                sb.Remove(sb.Length - 2, 2); //, and last space
                sb.Append(" WHERE Id = @Id");

                try {
                    cnn.Execute("" + sb.ToString(), classObject);
                    return new Tuple<bool, string>(true, "");
                }
                catch (Exception ex) {
                    //throw new Exception("Could not add record");
                    return new Tuple<bool, string>(false, $"Error: \n\n" +
                        $"{ex.Message}\n\n" +
                        $"Details: \n\n {ex}");
                }
            }
        }

        public void DeleteRecord(string tableName, int id) {
            using (SQLiteConnection con = new SQLiteConnection(conString)) {
                try {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = ($"DELETE FROM {tableName} WHERE Id = @Id");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) {
                    throw new Exception("Error thrown from SqLiteDataAccess: DeleteRecord: " + ex);
                }
            }
        }

        public List<string> ColumnToList(string columnName, string tableName) {
            using (IDbConnection cnn = new SQLiteConnection(conString)) {
                var output = cnn.Query<string>($"SELECT {columnName} FROM {tableName}", new DynamicParameters()); //
                return output.ToList();
            }
        }

        public DataTable GetDataTable(string tablename) {
            using (IDbConnection cnn = new SQLiteConnection(conString)) {
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter($"SELECT * FROM {tablename}", conString);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                dt.TableName = tablename;
                return dt;
            }
        }
    }
}
