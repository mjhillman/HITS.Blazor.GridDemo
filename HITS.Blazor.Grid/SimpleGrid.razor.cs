using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HITS.LIB.Extensions;


namespace HITS.Blazor.Grid
{
    /// <summary>
    /// This is the code-behind class for SimpleGrid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class SimpleGrid<T> : ComponentBase, IDisposable
    {
        /// <summary>
        /// the grid parameters object
        /// </summary>
        [Parameter]
        public GridParameters<T> Parameters { get; set; }

        /// <summary>
        /// the method to invoke when the add new button is clicked
        /// </summary>
        [Parameter]
        public EventCallback<string> OnAddNewEvent { get; set; }

        /// <summary>
        /// the method to invoke when the edit button is clicked
        /// </summary>
        [Parameter]
        public EventCallback<string> OnEditEvent { get; set; }

        private List<T> OriginalList { get; set; }

        private List<T> DataPage { get; set; }

        private List<T> FilteredList { get; set; }

        private Dictionary<string, object> ColumnsDictionary { get; set; }

        private Dictionary<string, bool> SortDictionary { get; set; }

        private Dictionary<string, string> FilterDictionary { get; set; }

        private GenericSorter<T> _genericSorter = new GenericSorter<T>();

        private int CurrentPage { get; set; }

        private int TotalPages { get; set; }

        private string SummaryText { get; set; }

        private bool readyToLoad = false;

        private bool disposedValue;

        private bool FilterActive { get; set; } = false;

        /// <summary>
        /// This method returns a bootstrap container or column class name based on the class parameter and desired width.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetDivClassName(string className, int value)
        {
            return $"{className}-{value}";
        }

        /// <summary>
        /// This method loads the initial page after the parameters have been applied.
        /// </summary>
        /// <remarks>if the render-mode is set to ServerPrerendered in _Host.cshtml then this method may fire twice</remarks>
        protected override void OnParametersSet()
        {
            if (Parameters.DataList != null && Parameters.DataList.Count > 0)
            {
                if (!readyToLoad)
                {
                    ColumnsDictionary = new Dictionary<string, object>();
                    FilterDictionary = new Dictionary<string, string>();
                    SortDictionary = new Dictionary<string, bool>();

                    ColumnsDictionary = ConvertModelToDictionary(Parameters.DataList[0]);

                    foreach (var key in ColumnsDictionary.Keys.ToList())
                    {
                        SortDictionary.Add(key, true);
                        FilterDictionary.Add(key, "");
                    }

                    DataPage = new List<T>();
                    OriginalList = new List<T>();
                    OriginalList = Parameters.DataList.GetRange(0, Parameters.DataList.Count);

                    SetPageCount(Parameters.DataList);

                    GetPage("first");

                    readyToLoad = true;
                }
                else
                {
                    GetPage(CurrentPage.ToString());
                }
            }
        }

        /// <summary>
        /// This method determines the maximum number of pages for the list based on the page size parameter.
        /// </summary>
        /// <param name="list"></param>
        private void SetPageCount(List<T> list)
        {
            if (!Parameters.ShowPager)
            {
                TotalPages = 1;
                Parameters.PageSize = list.Count;
            }
            else
            {
                CurrentPage = 1;
                TotalPages = (list.Count + Parameters.PageSize - 1) / Parameters.PageSize;
            }
        }

        /// <summary>
        /// This method converts a model to a dictionary.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Dictionary<string, object> ConvertModelToDictionary(object model)
        {
            return model.GetType().GetProperties(BindingFlags.DeclaredOnly |
                BindingFlags.Public |
                BindingFlags.Instance).ToDictionary
               (
                   propInfo => propInfo.Name,
                   propInfo => propInfo.GetValue(model, null)
               );
        }

        /// <summary>
        /// This method get the ShowInGrid attribute for a given property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>true if the property should be display in the grid.</returns>
        private bool ShowInGrid(string propertyName)
        {
            try
            {
                IEnumerable<object> attributes = typeof(T).GetProperty(propertyName)?.GetCustomAttributes(false);
                foreach (object attributeClass in attributes)
                {
                    if (attributeClass is GridAttributes)
                    {
                        return ((GridAttributes)attributeClass).ShowInGrid;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method get the SortBy attribute for a given property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>true if the filter by sort by icon should be displayed.</returns>
        private bool SortBy(string propertyName)
        {
            try
            {
                IEnumerable<object> attributes = typeof(T).GetProperty(propertyName)?.GetCustomAttributes(false);
                foreach (object attributeClass in attributes)
                {
                    if (attributeClass is GridAttributes)
                    {
                        return ((GridAttributes)attributeClass).SortBy;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method get the FilterBy attribute for a given property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>true if the filter by input box should be displayed.</returns>
        private bool FilterBy(string propertyName)
        {
            try
            {
                IEnumerable<object> attributes = typeof(T).GetProperty(propertyName)?.GetCustomAttributes(false);
                foreach (object attributeClass in attributes)
                {
                    if (attributeClass is GridAttributes)
                    {
                        return ((GridAttributes)attributeClass).FilterBy;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method get the Align attribute for a given property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>left, right or center, defaults to left</returns>
        private string Align(string propertyName)
        {
            try
            {
                IEnumerable<object> attributes = typeof(T).GetProperty(propertyName)?.GetCustomAttributes(false);
                foreach (object attributeClass in attributes)
                {
                    if (attributeClass is GridAttributes)
                    {
                        return ((GridAttributes)attributeClass).Align;
                    }
                }
                return "left";
            }
            catch (Exception)
            {
                return "left";
            }
        }

        /// <summary>
        /// This method returns a formatted property value.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>a formatted property value using the FormatString or the property value</returns>
        private object FormatString(string propertyName, object propertyValue)
        {
            try
            {
                string format = string.Empty;
                IEnumerable<object> attributes = typeof(T).GetProperty(propertyName)?.GetCustomAttributes(false);
                foreach (object attributeClass in attributes)
                {
                    if (attributeClass is GridAttributes)
                    {
                        format = ((GridAttributes)attributeClass).FormatString;
                        if (!string.IsNullOrWhiteSpace(format))
                        {
                            if (propertyValue.ToString().IsDouble())
                            {
                                propertyValue = propertyValue.AsDecimal().ToString(format);
                            }
                            else if (propertyValue.ToString().IsInt())
                            {
                                propertyValue = propertyValue.AsInt().ToString(format);
                            }
                            else if (propertyValue.ToString().IsLong())
                            {
                                propertyValue = propertyValue.AsLong().ToString(format);
                            }
                            else if (propertyValue.ToString().IsSingle())
                            {
                                propertyValue = propertyValue.AsSingle().ToString(format);
                            }
                            else if (propertyValue.ToString().IsDateTime())
                            {
                                propertyValue = propertyValue.AsDateTime().ToString(format);
                            }
                        }
                        else
                        {
                            return propertyValue;
                        }
                    }
                }
                return propertyValue;
            }
            catch (Exception)
            {
                return propertyValue;
            }
        }

        /// <summary>
        /// This method gets the primary key property value for the given model.
        /// </summary>
        /// <param name="model">the model object</param>
        /// <returns>primary key property value</returns>
        private string GetPrimaryKeyValue(T model)
        {
            return typeof(T).GetProperty(Parameters.PrimaryKeyName)?.GetValue(model, null).AsString();
        }

        /// <summary>
        /// This method applies the filter value.
        /// </summary>
        protected void OnFilterInput()
        {
            //start with a new list
            if (FilteredList == null)
            {
                FilteredList = new List<T>();
            }
            else
            {
                FilteredList?.Clear();
            }

            //used when there are more than one filter values
            List<T> removeList = new List<T>();

            bool firstFilter = false;

            //for each filter input
            foreach (KeyValuePair<string, string> entry in FilterDictionary)
            {
                if (!string.IsNullOrWhiteSpace(entry.Value))
                {
                    if (!firstFilter)   //get all the matches for the first filter value
                    {
                        foreach (T item in OriginalList)
                        {
                            string columnValue = typeof(T).GetProperty(entry.Key)?.GetValue(item).AsString().Trim();
                            if (columnValue.Contains(entry.Value, StringComparison.CurrentCultureIgnoreCase))
                            {
                                FilteredList.Add(item);
                            }
                        }
                        firstFilter = FilteredList.Count > 0;
                    }
                    else
                    {
                        foreach (T item in FilteredList)    //for subsequent filter value build a list of items that do not match
                        {
                            string columnValue = typeof(T).GetProperty(entry.Key)?.GetValue(item).AsString().Trim();
                            if (!columnValue.Contains(entry.Value, StringComparison.CurrentCultureIgnoreCase))
                            {
                                removeList.Add(item);
                            }
                        }
                    }
                }
            }

            //remove the items that do not match the filters
            FilteredList = FilteredList.Except(removeList).ToList();

            //set the FilterActive flag
            FilterActive = FilteredList.Count > 0;

            SetPageCount(FilteredList);

            GetPage("first");
        }

        /// <summary>
        /// This method removes list filtering.
        /// </summary>
        private void OnCancelFilerClick()
        {
            FilterActive = false;

            foreach (var key in FilterDictionary.Keys.ToList())
            {
                FilterDictionary[key] = "";
            }

            Parameters.DataList.Clear();
            Parameters.DataList = OriginalList.GetRange(0, OriginalList.Count);

            SetPageCount(OriginalList);

            GetPage("first");
        }

        /// <summary>
        /// This method applies list sorting.
        /// </summary>
        /// <param name="columnName"></param>
        protected void OnSortClick(string columnName)
        {
            bool sortAsc = !SortDictionary[columnName];

            OriginalList = _genericSorter.Sort(OriginalList, columnName, sortAsc == true ? "asc" : "desc").ToList();

            if (FilteredList == null || FilteredList?.Count == 0)
            {
                Parameters.DataList.Clear();
                Parameters.DataList = OriginalList.GetRange(0, OriginalList.Count);
                SetPageCount(OriginalList);
                GetPage("first");
            }
            else
            {
                OnFilterInput();
            }

            SortDictionary[columnName] = sortAsc;
        }

        /// <summary>
        /// This method displays the indicated page.
        /// </summary>
        /// <param name="direction">Direction can be a word like next, previous last or first.  Direction can also be an integer number.</param>
        private void GetPage(string direction)
        {
            if (Parameters.ShowPager)
            {
                if (direction == "next")
                    CurrentPage++;
                else if (direction == "previous")
                    CurrentPage--;
                else if (direction == "first")
                    CurrentPage = 1;
                else if (direction == "last")
                    CurrentPage = TotalPages;
                else
                    CurrentPage = Convert.ToInt32(direction);

                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                if (CurrentPage < 1) CurrentPage = 1;

                if (!FilterActive)
                {
                    Parameters.DataList = OriginalList.Skip((CurrentPage - 1) * Parameters.PageSize).Take(Parameters.PageSize).ToList();
                }
                else
                {
                    Parameters.DataList = FilteredList.Skip((CurrentPage - 1) * Parameters.PageSize).Take(Parameters.PageSize).ToList();
                }
            }
        }

        /// <summary>
        /// This method configures the summary text based on the summary parameters. 
        /// </summary>
        /// <returns>formatted summary text</returns>
        protected string GetSummaryText()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SummaryText)) return SummaryText;

                if (Parameters.SummaryType == GridParameters<T>.SummaryTypeEnum.Sum)
                {
                    SummaryText = $"{Parameters.SummaryTitle} {BuildSummaryList().Sum().ToString(Parameters.SummaryFormat)}";
                }
                else if (Parameters.SummaryType == GridParameters<T>.SummaryTypeEnum.Count)
                {
                    SummaryText = $"{Parameters.SummaryTitle} {BuildSummaryList().Count().ToString(Parameters.SummaryFormat)}";
                }
                else if (Parameters.SummaryType == GridParameters<T>.SummaryTypeEnum.Average)
                {
                    SummaryText = $"{Parameters.SummaryTitle} {BuildSummaryList().Average().ToString(Parameters.SummaryFormat)}";
                }
                return SummaryText;
            }
            catch (Exception ex)
            {
                SummaryText = $"{Parameters.SummaryTitle} {ex.Message}";
                return SummaryText;
            }
        }

        /// <summary>
        /// This method build a list of the summary property.
        /// </summary>
        /// <returns></returns>
        private List<double> BuildSummaryList()
        {
            List<double> list = new List<double>();

            foreach (T item in OriginalList)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(Parameters.SummaryColumnName);
                if (propertyInfo.Name == Parameters.SummaryColumnName)
                {
                    list.Add(propertyInfo.GetValue(item).AsDouble());
                }
            }

            if (list.Count == 0)
            {
                list.Add(new double());
            }

            return list;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    OriginalList?.Clear();
                    OriginalList = null;

                    DataPage?.Clear();
                    DataPage = null;

                    FilteredList?.Clear();
                    FilteredList = null;

                    ColumnsDictionary?.Clear();
                    ColumnsDictionary = null;

                    SortDictionary?.Clear();
                    SortDictionary = null;

                    FilterDictionary?.Clear();
                    FilterDictionary = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SimpleGrid()
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
