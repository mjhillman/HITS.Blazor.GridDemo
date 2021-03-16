using HITS.Blazor.Grid;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HITS.Blazor.GridDemo
{
    /// <summary>
    /// This class contains the methods for getting the data required by the grid,
    /// </summary>
    public class SalesGridViewModel : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// The list of products
        /// </summary>
        public List<SalesModel> SalesList { get; set; }

        /// <summary>
        /// The grid parameters/settings
        /// </summary>
        public GridParameters<SalesModel> SalesGridParameters { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SalesGridViewModel()
        {
            GetSalesData();                   //get the sample data from the CSV file
            //GetSalesDataFromDatabase();     //get the sample data from the database

            SalesGridParameters = new GridParameters<SalesModel>()
            {
                GridTitle = "Sample Data " + SalesList.Count.ToString("N0") + " records",
                DataList = SalesList,
                BootstrapColumnClass = "col-lg",
                BootstrapContainerClass = "container-fluid",
                ShowEditColumn = false,
                EditButtonText = "Edit",
                PrimaryKeyName = "OrderId",
                ShowFilterRow = true,
                ShowAddNew = false,
                ShowPager = true,
                PageSize = 1000,
                SummaryColumnName = nameof(SalesModel.TotalProfit),
                SummaryFormat = "n2",
                SummaryTitle = "Total Profit:",
                SummaryType = GridParameters<SalesModel>.SummaryTypeEnum.Sum
            };
        }

        /// <summary>
        /// This method retrieves product data from a CSV file and fills the product list.
        /// </summary>
        /// <remarks>Normally this would be a database call but a CSV file is easier to deploy for a demo application.</remarks>
        private void GetSalesData()
        {
            SalesList = new List<SalesModel>();

            //read the CSV file into an array of lines
            string[] lines = File.ReadAllLines("Sales.csv", Encoding.Latin1);

            //for each line convert the data into a model object
            for (int x = 1; x < lines.Length; x++)
            {
                SalesModel model = SalesModel.GetModelFromString(lines[x]);
                if (model != null)
                {
                    //add the model object to the list
                    SalesList.Add(model);
                }
            }

            //sort the list by the id
            SalesList = SalesList.OrderBy(x => x.OrderId).ToList();
        }

        private void GetSalesDataFromDatabase()
        {
            using (SalesDataMgr mgr = new SalesDataMgr())
            {
                SalesList = mgr.GetSalesData();
            }            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    SalesList?.Clear();
                    SalesList = null;
                    SalesGridParameters?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SalesViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
