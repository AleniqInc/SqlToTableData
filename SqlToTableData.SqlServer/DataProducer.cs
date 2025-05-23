using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SqlToTableData.SqlServer
{
    /// <summary>
    /// Provides functionality to return SQL SELECT query results as table (2 dimenional array) or JSON
    /// </summary>
    public class DataProducer
    {
        private string _connectionString;

        /// <summary>
        /// Constructor for class that provides functionality to return SQL SELECT query results as raw table (2 dimenional array) or JSON
        /// </summary>
        /// <param name="connectionString">SqlServer database connection string following ADO.NET syntax</param>
        public DataProducer(string connectionString)
        {
            _connectionString = connectionString;
        }


        /// <summary>
        /// Returns SQL query results as a table (2 dimensional array of objects) 
        /// </summary>
        /// <param name="sqlQuery">SQL SELECT query</param>
        /// <returns>2 dimensional array of objects (rows and columns) with actual column types. The first row is always column names.</returns>
        public object[] GetTable(string sqlQuery)
        {
            return SQLExecutor(sqlQuery);
        }


        /// <summary>
        /// Returns SQL query results as a JSON string
        /// </summary>
        /// <param name="sqlQuery">SQL SELECT query</param>
        /// <returns>JSON string representing 2 dimensional array. The first row is always column names.</returns>
        public string GetJson(string sqlQuery)
        {
            object[] result = SQLExecutor(sqlQuery);

            string jsonString = JsonSerializer.Serialize(result);

            return jsonString;
        }


        /// <summary>
        /// Executes SQL query and returns 2-dimensional array
        /// </summary>
        /// <param name="sqlQuery">SQL SELECT query</param>
        /// <returns>2 dimensional array of objects (rows and columns) with actual column types. The first row is always column names.</returns>
        private object[] SQLExecutor(string sqlQuery)
        {
            List<object> data = new List<object>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                SqlCommand command = new(sqlQuery, connection);

                connection.Open();
#if DEBUG
                Console.WriteLine("ServerVersion: {0}", connection.ServerVersion);
                Console.WriteLine("State: {0}", connection.State);
#endif
                using (var reader = command.ExecuteReader())
                {
                    object[] columns = null;
                    bool columnNamesSet = false;
                    int columnCount = 0;

                    if (reader.HasRows)
                    {
                        // Obtain a row from the query result.
                        while (reader.Read())
                        {
                            if (!columnNamesSet)
                            {
                                columnCount = reader.FieldCount;
                                columns = new object[columnCount];
                                for (int i = 0; i < columnCount; i++)
                                {
                                    columns[i] = reader.GetName(i);
                                }
                                columnNamesSet = true;
                                data.Add(columns);
                            }

                            object[] row = new object[columnCount];
                            for (int i = 0; i != columnCount; i++)
                            {
                                object value = reader.GetFieldValue<object>(i);
                                row[i] = value;
#if DEBUG
                                Console.Write("{0}, ", row[i]);
#endif
                            }
#if DEBUG
                            Console.WriteLine();
#endif
                            data.Add(row);
                        }
                    }
                    // Always call the Close method when you have finished using the DataReader object.
                    reader.Close();
                }
            }

            object[] result = data.ToArray();

            return result;

        }

    }
}
