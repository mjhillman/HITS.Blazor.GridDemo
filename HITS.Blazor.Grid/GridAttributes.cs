using System;

namespace HITS.Blazor.Grid
{
    /// <summary>
    /// This class describes validation attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GridAttributes : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GridAttributes()
        {
        }

        public bool ShowInGrid { get; set; } = true;

        public bool SortBy { get; set; } = true;

        public bool FilterBy { get; set; } = false;

        public string Align { get; set; } = "left";

        public string FormatString { get; set; }

    }
}
