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
    public class ProductGridViewModel : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// The list of products
        /// </summary>
        public List<ProductModel> ProductList { get; set; }

        /// <summary>
        /// The grid parameters/settings
        /// </summary>
        public GridParameters<ProductModel> ProductGridParameters { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProductGridViewModel()
        {
            GetProductData();

            ProductGridParameters = new GridParameters<ProductModel>()
            {
                GridTitle = "Sample Data " + ProductList.Count.ToString("N0") + " records",
                DataList = ProductList,
                BootstrapColumnClass = "col-lg",
                BootstrapContainerClass = "container-fluid",
                ShowEditColumn = true,
                EditButtonText = "Edit",
                PrimaryKeyName = "ProductId",
                ShowFilterRow = true,
                ShowAddNew = true,
                ShowPager = true,
                PageSize = 8,
                SummaryColumnName = nameof(ProductModel.UnitsInStock),
                SummaryFormat = "n0",
                SummaryTitle = "Total Units In Stock:",
                SummaryType = GridParameters<ProductModel>.SummaryTypeEnum.Sum
            };
        }

        /// <summary>
        /// This method retrieves product data from a CSV file and fills the product list.
        /// </summary>
        /// <remarks>Normally this would be a database call but a CSV file is easier to deploy for a demo application.</remarks>
        private void GetProductData()
        {
            ProductList = new List<ProductModel>();

            //read the CSV file into an array of lines
            string[] lines = File.ReadAllLines("Products.csv", Encoding.Latin1);
            
            //for each line convert the data into a model object
            for (int x = 1; x < lines.Length; x++)
            {
                ProductModel model = ProductModel.GetModelFromString(lines[x]);
                if (model != null)
                {
                    //add the model object to the list
                    ProductList.Add(model);
                }
            }

            //sort the list by the id
            ProductList = ProductList.OrderBy(x => x.ProductId).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    ProductList?.Clear();
                    ProductList = null;
                    ProductGridParameters?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProductViewModel()
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
