using HITS.LIB.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HITS.LIB.SqlServer
{
    /// <summary>
    /// This class contains data access methods for the SQL Client
    /// </summary>
    /// <remarks>Be sure to wraps call to this class in a try-catch block and handle the errors appropriately.</remarks>
    public class SqlSync : SqlBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SqlSync()
        {

        }

        /// <summary>
        /// This method will retrieve data from the database as a DataSet.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">optional: query timeout in seconds</param>
        /// <param name="captureLastSqlStatement">optional: set to true to save the last executed SQL statement in the LastSqlStatment property</param>
        /// <returns>DataSet object</returns>
        public DataSet GetDataSet(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            DataSet ds = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = sql.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameters)
                        {
                            cmd.Parameters.Add(GetParameter(kvp.Key, PrepValue(kvp.Value)));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        ds = new DataSet();
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// This method will retrieve data from the database as a DataTable.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <param name="captureLastSqlStatement">optional: set to true to save the last executed SQL statement in the LastSqlStatment property</param>
        /// <returns>DataTable object</returns>
        public DataTable GetDataTable(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            DataTable dt = null; ;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = sql.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameters)
                        {
                            cmd.Parameters.Add(GetParameter(kvp.Key, PrepValue(kvp.Value)));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        dt = new DataTable();
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// This method will retrieve data from the database as a List.
        /// </summary>
        /// <typeparam name="T">your data type that corresponds to the database data</typeparam>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <returns>List</returns>
        public List<T> GetDataTableAsList<T>(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT) where T : new()
        {
            DataTable dt = null;

            try
            {
                dt = GetDataTable(connectionString, sql, parameters, timeout);
                return dt == null ? new List<T>() : dt.Copy().ToList<T>();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dt?.Dispose();
            }
        }

        /// <summary>
        /// This method will retrieve data from the database as a typed record.
        /// </summary>
        /// <typeparam name="T">your data type that corresponds to the database data</typeparam>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <param name="captureLastSqlStatement">optional: set to true to save the last executed SQL statement in the LastSqlStatment property</param>
        /// <returns>List</returns>
        public T GetDataRecord<T>(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false) where T : new()
        {
            DataTable dt = null;
            try
            {
                if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);
                dt = GetDataTable(connectionString, sql, parameters, timeout);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Copy().ToList<T>()[0];
                }
                return new T();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dt?.Dispose();
            }
        }

        /// <summary>
        /// This method will execute a SQL Stored Procedure.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <param name="captureLastSqlStatement">optional: set to true to save the last executed SQL statement in the LastSqlStatment property</param>
        /// <returns>the number of rows affected</returns>
        public int ExecuteNonQuery(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.CommandTimeout = timeout;
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameters)
                        {
                            cmd.Parameters.Add(GetParameter(kvp.Key, PrepValue(kvp.Value)));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    connection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// This method will execute a stored procedure that returns a single value.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout">query timeout in seconds</param>
        /// <param name="captureLastSqlStatement">optional: set to true to save the last executed SQL statement in the LastSqlStatment property</param>
        /// <returns>an object value</returns>
        public object ExecuteScalar(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = sql.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameters)
                        {
                            cmd.Parameters.Add(GetParameter(kvp.Key, PrepValue(kvp.Value)));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    connection.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

    }
}
