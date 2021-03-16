using HITS.LIB.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HITS.LIB.SqlServer
{
    /// <summary>
    /// This class contains data access methods for the SQL Client
    /// </summary>
    /// <remarks>Be sure to wraps call to this class in a try-catch block and handle the errors appropriately.</remarks>
    public class SqlAsync : SqlBase
    {
        /// <summary>
        /// Constructor
        /// </summary>

        public SqlAsync()
        {
        }

        /// <summary>
        /// This method will retrieve data from the database as a DataSet.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout"></param>
        /// <param name="captureLastSqlStatement"></param>
        /// <returns>DataSet object</returns>
        internal protected async Task<DataSet> GetDataSetAsync(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
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
                            cmd.Parameters.AddWithValue(kvp.Key, PrepValue(kvp.Value));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        ds = new DataSet();
                        await Task.Run(() => { da.Fill(ds); }).ConfigureAwait(false);
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
        /// <param name="timeout"></param>
        /// <param name="captureLastSqlStatement"></param>
        /// <returns>DataTable object</returns>
        internal protected async Task<DataTable> GetDataTableAsync(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            DataTable dt = null;

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
                            cmd.Parameters.AddWithValue(kvp.Key, PrepValue(kvp.Value));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        dt = new DataTable();
                        await Task.Run(() => { da.Fill(dt); }).ConfigureAwait(false);
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
        /// <param name="timeout"></param>
        /// <param name="captureLastSqlStatement"></param>
        /// <returns>List</returns>
        internal protected async Task<List<T>> GetDataTableAsListAsync<T>(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false) where T : new()
        {
            DataTable dt = await GetDataTableAsync(connectionString, sql, parameters, timeout, captureLastSqlStatement).ConfigureAwait(false);
            return dt.ToList<T>();
        }

        /// <summary>
        /// This method will execute a SQL statement.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout"></param>
        /// <param name="captureLastSqlStatement"></param>
        /// <returns>numer of rows affected</returns>
        internal protected async Task<int> ExecuteNonQueryAsync(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            int rowsAffected = 0;

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
                            cmd.Parameters.AddWithValue(kvp.Key, PrepValue(kvp.Value));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    await connection.OpenAsync();
                    rowsAffected = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            return rowsAffected;
        }

        /// <summary>
        /// This method will execute a scalar statement that returns a single value.
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="sql">the stored procedure name or SQL statement</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="parameters">optional: the parameter dictionary</param>
        /// <param name="timeout"></param>
        /// <returns>an object value</returns>
        internal protected async Task<object> ExecuteScalarAsync(string connectionString, string sql, IDictionary<string, object> parameters = null, int timeout = QUERY_TIMEOUT, bool captureLastSqlStatement = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = sql.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> kvp in parameters)
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, PrepValue(kvp.Value));
                        }
                    }

                    if (captureLastSqlStatement) CaptureSqlStatement(sql, parameters);

                    await connection.OpenAsync();

                    return await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
        }
    }
}