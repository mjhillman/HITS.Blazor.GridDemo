using HITS.LIB.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HITS.LIB.SqlServer
{
    /// <summary>
    /// This class in inherited by SQLDataAccess and SQLDataAccessAsync
    /// </summary>
    public abstract class SqlBase : IDisposable
    {
        /// <summary>
        /// Last SQL Statement Executed
        /// </summary>
        public string LastSqlStatement { get; set; }

        internal const int QUERY_TIMEOUT = 30;

        public static object PrepValue(object value)
        {
            if (value is string)
            {
                return value?.AsString().Replace("'", "''");
            }
            else
            {
                return value;
            }
        }

        internal void CaptureSqlStatement(string spName, IDictionary<string, object> parameters)
        {
            try
            {
                string message = $"EXEC {spName} ";
                foreach (KeyValuePair<string, object> p in parameters)
                {
                    message += $@"{p.Key}='{p.Value.AsString()}',";
                }
                LastSqlStatement = message.TrimEnd(',') + ";";                
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// This method tests the connection to the database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>true on success</returns>
        public string ConnectionTest(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT GETDATE();", connection) { CommandType = CommandType.Text })
                    {
                        connection.Open();
                        object dateTime = cmd.ExecuteScalar();
                    }
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        internal SqlParameter GetParameter(string name, object value)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = name;

            if (value.GetType() == typeof(double))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Decimal;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(decimal))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Decimal;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(float))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Float;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(Byte[]))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Binary;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(Int64))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                sqlParameter.Value = value.AsLong();
            }
            else if (value.GetType() == typeof(Int32))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Int;
                sqlParameter.Value = value.AsLong();
            }
            else if (value.GetType() == typeof(long))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Int;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(int))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Int;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(bool))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(DateTime))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.DateTime;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(DateTimeOffset))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.DateTimeOffset;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(Guid))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;
                sqlParameter.Value = value;
            }
            else if (value.GetType() == typeof(Char))
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.Char;
                sqlParameter.Value = value.AsString();
            }
            else
            {
                sqlParameter.SqlDbType = System.Data.SqlDbType.NVarChar;
                sqlParameter.Value = value.AsString();
            }

            return sqlParameter;

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DataAccessBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
