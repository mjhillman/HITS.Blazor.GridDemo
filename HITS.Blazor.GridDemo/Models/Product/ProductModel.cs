using HITS.Blazor.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HITS.LIB.Extensions;

namespace HITS.Blazor.GridDemo
{
    /// <summary>
    /// This class describes the product data and contains the attributes for the grid.
    /// </summary>
    public class ProductModel : ModelBase
    {
        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Numeric,  AllowNull = false)]
        public long ProductId { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Alphanumeric, AllowNull = false,  FieldLength = 40)]
        [DataType(DataType.Text)]
        public string Product { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Alphanumeric, AllowNull = false, FieldLength = 40)]
        public string Supplier { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Alphanumeric, AllowNull = false, MinLength = 1, FieldLength = 40)]
        public string Category { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right")]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Alphanumeric, AllowNull = false)]
        public string UnitQty { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "c")]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Numeric, MaxNumericValue = 999999,  AllowNull = false)]
        public double UnitPrice { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "n0")]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Numeric, MinNumericValue = 0, MaxNumericValue = 9999, AllowNull = false)]
        public int UnitsInStock { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "n0")]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Numeric, MinNumericValue = 0, MaxNumericValue = 9999, AllowNull = false)]
        public int ReorderLevel { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "center")]
        [ValidationAttributes(FieldFormat = ValidationAttributes.DataType.Object, AllowNull = false)]
        public bool Discontinued { get; set; }

        public ProductModel Clone()
        {
            return (ProductModel)this.MemberwiseClone();
        }

        public void CopyTo(ProductModel model)
        {
            PropertyInfo[] propInfo = this.GetType().GetProperties();
            foreach (PropertyInfo item in propInfo)
            {
                model.GetType().GetProperty(item.Name).SetValue(model, item.GetValue(this, null), null);
            }
        }

        /// <summary>
        /// This method converts one line of the CSV file to a ProductModel
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns>ProductModel object</returns>
        /// <remarks>This is where you would do any data formatting.</remarks>
        public static ProductModel GetModelFromString(string csvString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csvString))
                {
                    string[] values = csvString.Split(',');
                    ProductModel model = new ProductModel();
                    model.ProductId = values[0].AsLong();
                    model.Product = values[1];
                    model.Supplier = values[2];
                    model.Category = values[3];
                    model.UnitQty = values[4];
                    model.UnitPrice = values[5].AsDouble();    
                    model.UnitsInStock = values[6].AsInt();
                    model.ReorderLevel = values[7].AsInt();
                    model.Discontinued = values[8].ToString() == "0" ? false : true;
                    return model;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}