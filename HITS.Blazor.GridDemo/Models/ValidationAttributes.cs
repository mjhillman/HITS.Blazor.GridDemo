using System;

namespace HITS.Blazor.GridDemo
{
    /// <summary>
    /// This class describes validation attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidationAttributes : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationAttributes()
        {
        }

        public enum DataType { Alphanumeric, Numeric, DateTime, List, Dictionary, Object }
        public enum Case { Default, Upper, Lower }

        /// <summary>
        /// The maximum and required field length
        /// </summary>
        public int FieldLength { get; set; }

        /// <summary>
        /// AlphaNumeric, Numeric, DateTime, Date
        /// </summary>
        public DataType FieldFormat { get; set; }

        /// <summary>
        /// Minimum Field Length
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Minimum numeric value
        /// </summary>
        public long MinNumericValue { get; set; }

        /// <summary>
        /// Maximum numeric value
        /// </summary>
        public long MaxNumericValue { get; set; }

        /// <summary>
        /// Format String
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// Case Specifier
        /// </summary>
        public Case StringCase { get; set; }

        /// <summary>
        /// Allow null
        /// </summary>
        public bool AllowNull { get; set; }

    }
}
