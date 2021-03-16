using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace HITS.LIB.Extensions
{
    /// <summary>
    /// This class contains an extension to convert a DataTable to a List of T
    /// </summary>
    public static class DataTableExtensions
    {
        private static Dictionary<Type, List<PropertyInfo>> typeDictionary = new Dictionary<Type, List<PropertyInfo>>();

        /// <summary>
        /// This method converts a DataTable to an IList of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<PropertyInfo> properties = ObjectExtensions.GetPropertiesForType<T>();
            List<T> result = new List<T>();

            if (table != null && table.Rows?.Count > 0)
            {
                foreach (var row in table.Rows)
                {
                    var item = CreateItemFromRow<T>((DataRow)row, properties);
                    result.Add(item);
                }
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, List<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (property.PropertyType == typeof(double))
                    {
                        property.SetValue(item, row[property.Name].AsDouble(), null);
                    }
                    else if (property.PropertyType == typeof(decimal))
                    {
                        property.SetValue(item, row[property.Name].AsDecimal(), null);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        property.SetValue(item, row[property.Name].AsFloat(), null);
                    }
                    else if (property.PropertyType == typeof(long))
                    {
                        property.SetValue(item, row[property.Name].AsLong(), null);
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(item, row[property.Name].AsInt(), null);
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(item, row[property.Name].AsBool(), null);
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(item, row[property.Name].AsDateTime(), null);
                    }
                    else //string
                    {
                        property.SetValue(item, row[property.Name].AsString(), null);
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// This method converts a DataTable to a List of type string having comma separated values.
        /// </summary>
        /// <param name="table">the data table</param>
        /// <returns>List of type string</returns>
        public static List<string> ToLines(this DataTable table)
        {
            if (table.IsEmpty()) return null;
            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string line = string.Empty;
                foreach (DataColumn column in table.Columns)
                {
                    line += $"{row[column.ColumnName].AsString()},";
                }
                list.Add(line);
            }
            return list;
        }

        /// <summary>
        /// This method will determine if a DataTable contains and data.
        /// </summary>
        /// <param name="dt">DataTable object</param>
        /// <returns></returns>
        public static bool IsEmpty(this DataTable dt)
        {
            if (dt == null) return true;
            if (dt.Rows == null) return true;
            if (dt.Rows.Count == 0) return true;
            return false;
        }

        /// <summary>
        /// This method will determine if a DataSet contains and data.
        /// </summary>
        /// <param name="ds">DataSet object</param>
        /// <returns></returns>
        public static bool IsEmpty(this DataSet ds)
        {
            if (ds == null) return true;
            if (ds.Tables == null) return true;
            if (ds.Tables.Count == 0) return true;
            if (ds.Tables[0].Rows == null) return true;
            if (ds.Tables[0].Rows.Count == 0) return true;
            return false;
        }

        /// <summary>
        /// This method converts a DataTable to an IList of type T without knowing the type. 
        /// </summary>
        /// <typeparam name="T">could be object</typeparam>
        /// <param name="dt">DataTable</param>
        /// <param name="isFirstRowColumnsHeader">determines if the first row will be skipped and not included in the list</param>
        /// <returns>IList<typeparamref name="T"/></returns>
        /// <remarks> 
        /// usage...
        /// dynamic dynamicList = datatable.ToList[object](false)
        /// </remarks>
        public static IList<T> ToList<T>(this DataTable dt, bool isFirstRowColumnsHeader = false) where T : new()
        {
            var results = new List<T>();

            if (dt != null && dt.Rows.Count > 0)
            {
                var columns = dt.Columns.Cast<DataColumn>().ToList();
                var rows = dt.Rows.Cast<DataRow>().ToList();
                var headerNames = columns.Select(col => col.ColumnName).ToList();
                //
                // Find properties name or columns name
                if (isFirstRowColumnsHeader)
                {
                    for (var i = 0; i < headerNames.Count; i++)
                    {
                        if (rows[0][i] != DBNull.Value && !string.IsNullOrEmpty(rows[0][i].ToString()))
                            headerNames[i] = rows[0][i].ToString();
                    }

                    //
                    // remove first row because that is header
                    rows.RemoveAt(0);
                }

                // Create dynamic or anonymous object for `T type
                if (typeof(T) == typeof(System.Dynamic.ExpandoObject) ||
                    typeof(T) == typeof(System.Dynamic.DynamicObject) ||
                    typeof(T) == typeof(System.Object))
                {
                    var dynamicDt = new List<dynamic>();
                    foreach (var row in rows)
                    {
                        dynamic dyn = new ExpandoObject();
                        dynamicDt.Add(dyn);
                        for (var i = 0; i < columns.Count; i++)
                        {
                            var dic = (IDictionary<string, object>)dyn;
                            dic[headerNames[i]] = row[columns[i]];
                        }
                    }
                    return (dynamic)dynamicDt;
                }
                else // other types of `T
                {
                    var properties = typeof(T).GetProperties();
                    if (columns.Any() && properties.Any())
                    {
                        foreach (var row in rows)
                        {
                            var entity = new T();
                            for (var i = 0; i < columns.Count; i++)
                            {
                                if (!row.IsNull(columns[i]))
                                {
                                    typeof(T).GetProperty(headerNames[i])? // ? -> maybe the property by name `headerNames[i]` is not exist in entity then get null!
                                        .SetValue(entity, row[columns[i]] == DBNull.Value ? null : row[columns[i]]);
                                }
                            }
                            results.Add(entity);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Returns a dynamic DataRow instance that can be accessed
        /// with the field name as a property
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>taTab
        public static dynamic DynamicRow(this DataTable dt, int index)
        {
            var row = dt.Rows[index];
            return new DynamicDataRow(row);
        }

        /// <summary>
        /// Returns a dynamic list of rows so you can reference them with
        /// row.fieldName
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<dynamic> DynamicRows(this DataTable dt)
        {
            List<dynamic> drows = new List<dynamic>();

            foreach (DataRow row in dt.Rows)
                drows.Add(new DynamicDataRow(row));

            return drows;
        }
    }

    /// <summary>
    /// This class provides an easy way to turn a DataRow 
    /// into a Dynamic object that supports direct property
    /// access to the DataRow fields.
    /// 
    /// The class also automatically fixes up DbNull values
    /// (null into .NET and DbNUll to DataRow)
    /// </summary>
    /// <remarks>
    /// https://weblog.west-wind.com/posts/2011/Nov/24/Creating-a-Dynamic-DataRow-for-easier-DataRow-Syntax
    /// </remarks>
    public class DynamicDataRow : DynamicObject
    {
        /// <summary>
        /// Instance of object passed in
        /// </summary>
        DataRow DataRow;

        /// <summary>
        /// Pass in a DataRow to work off
        /// </summary>
        /// <param name="instance"></param>
        public DynamicDataRow(DataRow dataRow)
        {
            DataRow = dataRow;
        }

        /// <summary>
        /// Returns a value from a DataRow items array.
        /// If the field doesn't exist null is returned.
        /// DbNull values are turned into .NET nulls.
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            try
            {
                result = DataRow[binder.Name];

                if (result == DBNull.Value)
                    result = null;

                return true;
            }
            catch { }

            result = null;
            return false;
        }


        /// <summary>
        /// Property setter implementation tries to retrieve value from instance 
        /// first then into this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                if (value == null)
                    value = DBNull.Value;

                DataRow[binder.Name] = value;
                return true;
            }
            catch { }

            return false;
        }
    }
}
