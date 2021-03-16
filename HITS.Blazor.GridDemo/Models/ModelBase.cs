using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using HITS.LIB.Extensions;

namespace HITS.Blazor.GridDemo
{
    /// <summary>
    /// This class is inherited by a model POCO to facilitate validation.
    /// </summary>
    public abstract class ModelBase : IDisposable
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ModelBase()
            {

            }

            /// <summary>
            /// This method will convert the model properties to a dictionary.
            /// </summary>
            /// <returns>dictionary of property names and values</returns>
            public Dictionary<string, object> ConvertModelToDictionary()
            {
                return (Dictionary<string, object>)this.AsDictionary();
            }

            /// <summary>
            /// This method validates the model using the validation attributes,
            /// </summary>
            /// <param name="separator">the line separator, usually a HTML break tag or new line character</param>
            /// <returns>string containing one line for each error</returns>
            public List<string> ValidateModel(string separator = "<br/>")
            {
                List<string> validationErrors;

                validationErrors = Validate();

                if (validationErrors?.Count > 0)
                {
                    return validationErrors;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// This method validates model objects that use the Validation attribute class and provides formatting where a format string attribute was supplied.
            /// </summary>
            /// <returns>List of error strings</returns>
            private List<string> Validate()
            {
                List<string> _errors = new List<string>();

                PropertyInfo[] properties = this.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in properties)
                {
                    string value = string.Empty;
                    List<string> errors = null;

                    IEnumerable<ValidationAttributes> attributes = (IEnumerable<ValidationAttributes>)propertyInfo.GetCustomAttributes(typeof(ValidationAttributes));
                    if (attributes.Count() > 0)
                    {
                        ValidationAttributes attribute = (ValidationAttributes)attributes.First();

                        if (!attribute.IsNull())
                        {

                            //get the data type
                            ValidationAttributes.DataType fieldFormat = attribute.FieldFormat;

                            //call the appropriate validation method
                            try
                            {
                                if (fieldFormat == ValidationAttributes.DataType.Numeric)
                                {
                                    errors = NumericValidator(attribute, propertyInfo);
                                }
                                else if (fieldFormat == ValidationAttributes.DataType.Alphanumeric)
                                {
                                    errors = AlphaNumericValidator(attribute, propertyInfo);
                                }
                                else if (fieldFormat == ValidationAttributes.DataType.DateTime)
                                {
                                    errors = DateTimeValidator(attribute, propertyInfo);
                                }
                                else if (fieldFormat == ValidationAttributes.DataType.List)
                                {
                                    errors = ListValidator(attribute, propertyInfo);
                                }
                                else if (fieldFormat == ValidationAttributes.DataType.Dictionary)
                                {
                                    errors = DictionaryValidator(attribute, propertyInfo);
                                }
                                else if (fieldFormat == ValidationAttributes.DataType.Object)
                                {
                                    errors = ObjectValidator(attribute, propertyInfo);
                                }

                                if (!errors.IsNull())
                                {
                                    foreach (string error in errors)
                                    {
                                        _errors.Add(error);
                                    }
                                    errors.Clear();
                                    errors = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"{propertyInfo.Name }: { ex.Message}");
                            }
                        }
                    }

                }

                return _errors.Count == 0 ? null : _errors;
            }

            private List<string> NumericValidator(ValidationAttributes attribute, PropertyInfo propertyInfo, bool allowCurrency = false)
            {
                List<string> errors = new List<string>();

                //get the property name and value
                string propertyName = propertyInfo.Name;
                string propertyValue = propertyInfo.GetValue(this, null).AsString().Trim();

                //min length check
                if (attribute.MinLength > 0 && propertyValue?.Length < attribute.MinLength)
                {
                    errors.Add($"{propertyName} must be at least {attribute.MinLength} characters.");
                }

                //too long check
                if (attribute.FieldLength > 0 && !string.IsNullOrWhiteSpace(propertyValue) &&
                    propertyValue.AsDecimal().ToString(attribute.FormatString).Length > attribute.FieldLength)
                {
                    errors.Add($"{propertyName} must not exceed {attribute.FieldLength} characters.");
                }

                //numeric value check
                if (propertyValue?.Length > 0 && !propertyValue.IsDecimal(allowCurrency))
                {
                    errors.Add($"{propertyName} must be a numeric value.");
                }

                //numeric min value check
                if (propertyValue.IsDecimal() && !attribute.MinNumericValue.IsNull() && propertyValue.AsDecimal() < attribute.MinNumericValue)
                {
                    errors.Add($"{propertyName} value must be equal to or greater than {attribute.MinNumericValue}.");
                }

                //numeric max value check
                if (propertyValue.IsDecimal() && attribute.MaxNumericValue > attribute.MinNumericValue && propertyValue.AsDecimal() > attribute.MaxNumericValue)
                {
                    errors.Add($"{propertyName} value must be equal to or less than {attribute.MaxNumericValue}.");
                }

                return errors;
            }

            private List<string> AlphaNumericValidator(ValidationAttributes attribute, PropertyInfo propertyInfo)
            {
                List<string> errors = new List<string>();

                //get the property name and value
                string propertyName = propertyInfo.Name;
                string propertyValue = propertyInfo.GetValue(this, null).AsString().Trim();

                //null or empty check
                if (attribute.AllowNull == false && string.IsNullOrWhiteSpace(propertyValue))
                {
                    errors.Add($"{propertyName} must not be null or empty.");
                }

                if (attribute.MinNumericValue != 0 || attribute.MaxNumericValue != 0)
                {
                    if (propertyValue.IsDouble(true) || propertyValue.IsLong() || propertyValue.IsFloat(true) || propertyValue.IsInt())
                    {
                        attribute.FieldFormat = ValidationAttributes.DataType.Numeric;
                        errors = NumericValidator(attribute, propertyInfo, true);
                    }
                    attribute.FieldFormat = ValidationAttributes.DataType.Alphanumeric;
                }

                //min length check
                if (attribute.MinLength > 0 && propertyValue?.Length < attribute.MinLength)
                {
                    errors.Add($"{propertyName} must be at least {attribute.MinLength} characters.");
                }

                //max length check
                if (attribute.FieldLength > 0 && !string.IsNullOrWhiteSpace(propertyValue) && propertyValue.Length > attribute.FieldLength)
                {
                    errors.Add($"{propertyName} must not exceed {attribute.FieldLength} characters.");
                }

                if (errors.Count == 0)
                {
                    if (attribute.StringCase == ValidationAttributes.Case.Lower)
                    {
                        propertyInfo.SetValue(this, propertyValue.ToLower());
                    }
                    else if (attribute.StringCase == ValidationAttributes.Case.Upper)
                    {
                        propertyInfo.SetValue(this, propertyValue.ToUpper());
                    }
                    else
                    {
                        propertyInfo.SetValue(this, propertyValue);
                    }
                }

                return errors;
            }

            private List<string> DateTimeValidator(ValidationAttributes attribute, PropertyInfo propertyInfo)
            {
                List<string> errors = new List<string>();

                //get the property name and value
                string propertyName = propertyInfo.Name;
                string propertyValue = propertyInfo.GetValue(this, null).AsString().Trim();

                if (propertyValue.IsNull() || !propertyValue.IsDateTime())
                {
                    errors.Add($"{propertyName} is not a valid date/time string.");
                }

                return errors;
            }

            private List<string> ListValidator(ValidationAttributes attribute, PropertyInfo propertyInfo)
            {
                List<string> errors = new List<string>();

                //get the property name and value
                string propertyName = propertyInfo.Name;
                object propertyValue = propertyInfo.GetValue(this, null);

                if (!propertyValue.IsList())
                {
                    errors.Add($"The {propertyName} is not a valid List object.");
                }
                else
                {
                    if (attribute.MinLength > 0)
                    {
                        if (((ICollection)propertyValue).IsNull() || ((ICollection)propertyValue).Count < attribute.MinLength)
                        {
                            errors.Add($"The {propertyName} list is empty.");
                        }
                    }
                }

                return errors;

            }

            private List<string> DictionaryValidator(ValidationAttributes attribute, PropertyInfo propertyInfo)
            {
                List<string> errors = new List<string>();

                //get the property name and value
                string propertyName = propertyInfo.Name;
                object propertyValue = propertyInfo.GetValue(this, null);

                if (!propertyValue.IsDictionary())
                {
                    errors.Add($"The {propertyName} is not a valid Dictionary object.");
                }
                else
                {
                    if (attribute.MinLength > 0)
                    {
                        if (((ICollection)propertyValue).IsNull() || ((ICollection)propertyValue).Count < attribute.MinLength)
                        {
                            errors.Add($"The {propertyName} dictionary is empty.");
                        }
                    }
                }

                return errors;

            }

            private List<string> ObjectValidator(ValidationAttributes attribute, PropertyInfo propertyInfo)
            {
                List<string> errors = new List<string>();

                //get the property name and value
                string propertyName = propertyInfo.Name;
                object propertyValue = propertyInfo.GetValue(this, null);

                if (!attribute.AllowNull && propertyValue.IsNull())
                {
                    errors.Add($"The {propertyName} object is null.");
                }

                return errors;

            }


            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            /// <summary>
            /// The Dispose Method
            /// </summary>
            /// <param name="disposing"></param>
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
            // ~ModelBase()
            // {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            /// <summary>
            /// This code added to correctly implement the disposable pattern. 
            /// </summary>
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
