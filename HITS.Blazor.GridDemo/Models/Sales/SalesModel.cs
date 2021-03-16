using HITS.Blazor.Grid;
using HITS.LIB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HITS.Blazor.GridDemo
{
    public class SalesModel: ModelBase
    {
        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        public long OrderId { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        public string Country { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        public string Region { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        public string SalesChannel { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = false)]
        public string OrderPriority { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, FormatString = "d")]
        public DateTime OrderDate { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, FormatString = "d")]
        public DateTime ShipDate { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true)]
        public string ItemType { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "n0")]
        public long UnitsSold { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "c")]
        public double UnitPrice { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "c")]
        public double UnitCost { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "c")]
        public double TotalRevenue { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "c")]
        public double TotalCost { get; set; }

        [GridAttributes(FilterBy = true, SortBy = true, ShowInGrid = true, Align = "right", FormatString = "c")]
        public double TotalProfit { get; set; }

        public SalesModel Clone()
        {
            return (SalesModel)this.MemberwiseClone();
        }

        public void CopyTo(SalesModel model)
        {
            PropertyInfo[] propInfo = this.GetType().GetProperties();
            foreach (PropertyInfo item in propInfo)
            {
                model.GetType().GetProperty(item.Name).SetValue(model, item.GetValue(this, null), null);
            }
        }

        /// <summary>
        /// This method converts one line of the CSV file to a SalesModel
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns>SalesModel object</returns>
        /// <remarks>This is where you would do any data formatting.</remarks>
        public static SalesModel GetModelFromString(string csvString)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(csvString))
                {
                    string[] values = csvString.Split(',');
                    SalesModel model = new SalesModel();
                    model.OrderId = values[6].AsLong();
                    model.Country = values[1];
                    model.ItemType = values[2];
                    model.OrderDate = values[5].AsDateTime();
                    model.OrderPriority = values[4];
                    model.Region = values[0];
                    model.SalesChannel = values[3];
                    model.ShipDate = values[7].AsDateTime();
                    model.TotalCost = values[12].AsDouble();
                    model.TotalProfit = values[13].AsDouble();
                    model.TotalRevenue = values[11].AsDouble();
                    model.UnitCost = values[10].AsDouble();
                    model.UnitPrice = values[9].AsDouble();
                    model.UnitsSold = values[8].AsLong();
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
