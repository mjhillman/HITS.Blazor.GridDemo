using HITS.LIB.SqlServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HITS.Blazor.GridDemo
{
    public class SalesDataMgr : SqlSync
    {
        private string _connectionString = string.Empty;

        public SalesDataMgr()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "SalesData.mdf");
            _connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path};Integrated Security=True;Connect Timeout=30";
        }

        public int InsertSalesData(SalesModel model)
        {
            Dictionary<string, object> parameters = model.ConvertModelToDictionary();
            return ExecuteNonQuery(_connectionString, "InsertSalesData", parameters);            
        }

        public List<SalesModel> GetSalesData()
        {
            return GetDataTableAsList<SalesModel>(_connectionString, "GetSalesData");
        }

        protected override void Dispose(bool disposing)
        {
            _connectionString = string.Empty;
            base.Dispose(disposing);
        }
    }
}
