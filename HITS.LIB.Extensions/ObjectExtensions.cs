using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace HITS.LIB.Extensions
{
    /// <summary>
    /// This class contains Object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// This method checks if an object value is DBNull or Null
        /// </summary>
        /// <param name="value">the object value to test.</param>
        /// <returns>true if value is DBNull or Null; otherwise, false.</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsNull(this object value)
        {
            return value == null || Convert.IsDBNull(value);
        }

        /// <summary>
        /// This method checks if an object is a List
        /// </summary>
        /// <param name="value">the object value to test.</param>
        /// <returns>true if value is a List; otherwise, false.</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsList(this object value)
        {
            return value is IList &&
                   value.GetType().IsGenericType &&
                   value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        /// <summary>
        /// This method checks if an object is a Dictionary
        /// </summary>
        /// <param name="value">the object value to test.</param>
        /// <returns>true if value is a Dictionary; otherwise, false.</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsDictionary(this object value)
        {
            return value is IDictionary &&
                value.GetType().IsGenericType &&
                value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        /// <summary>
        /// This method returns the string value of an object.
        /// </summary>
        /// <param name="value">the object to test.</param>
        /// <returns>the value as a string otherwise; the string.Empty</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static string AsString(this object value)
        {
            return value.IsNull() ? string.Empty : value.ToString();
        }

        /// <summary>
        /// This method returns the string value of an object.
        /// </summary>
        /// <param name="value">the object to test.</param>
        /// <param name="defaultStringValue">the string value to return if the object is empty</param>
        /// <returns>the value as a string otherwise; the defaultStringValue</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static string AsString(this object value, string defaultStringValue)
        {
            return value.IsNull() ? defaultStringValue : value.ToString();
        }

        /// <summary>
        /// This method converts an object to an integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static int AsInt(this object value)
        {
            return value.AsString().AsInt();
        }

        /// <summary>
        /// This method converts an object to a long.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static long AsLong(this object value)
        {
            return value.AsString().AsLong();
        }

        /// <summary>
        /// This method converts an object to a Decimal.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Decimal AsDecimal(this object value)
        {
            return value.AsString().As<Decimal>();
        }

        /// <summary>
        /// This method converts an object to a Double.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Double AsDouble(this object value)
        {
            return value.AsString().As<Double>();
        }

        /// <summary>
        /// This method converts an object to a Single.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Single AsSingle(this object value)
        {
            return value.AsString().As<Single>();
        }

        /// <summary>
        /// This method converts an object to a Float.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static float AsFloat(this object value)
        {
            return value.AsString().AsFloat();
        }

        /// <summary>
        /// This method converts an object to a DateTime.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns the default DateTime.
        /// </remarks>
        public static DateTime AsDateTime(this object value)
        {
            return value.AsString().AsDateTime(new DateTime());
        }

        /// <summary>
        /// This method converts an object to a bool.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns the false.
        /// </remarks>
        public static bool AsBool(this object value)
        {
            return value.AsString().AsBool();
        }

        /// <summary>
        /// This method converts a type of T to type of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]   //hides method from Intellisense only for DLL reference, not project reference.
        public static T As<T>(this object value)
        {
            Type t = typeof(T);

            // Get the type that was made nullable.
            Type valueType = Nullable.GetUnderlyingType(typeof(T));

            if (valueType != null)
            {
                // nullable type.

                if (value == null)
                {
                    // you may want to do something different here.
                    return default(T);
                }
                else
                {
                    // Convert to the value type.
                    object result = Convert.ChangeType(value, valueType);

                    // Cast the value type to the nullable type.
                    return (T)result;
                }
            }
            else
            {
                // Not nullable.
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// This method will convert object properties to a dictionary.
        /// </summary>
        /// <param name="source">the object</param>
        /// <param name="bindingAttr">non default binding properties</param>
        /// <returns>string, object dictionary</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static IDictionary<string, object> AsDictionary<T>(this T source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

        /// <summary>
        /// This method will convert a dictionary of object properties to a new populated object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="source">string, object dictionary</param>
        /// <returns>new object of type T</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
        {
            T newObject = new T();
            Type someObjectType = newObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(newObject, item.Value, null);
            }

            return newObject;
        }

        public static void CopyObject(object source, object destination)
        {
            // Purpose : Use reflection to set property values of objects that share the same property names.
            Type s = source.GetType();
            Type d = destination.GetType();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var objSourceProperties = s.GetProperties(flags);
            var objDestinationProperties = d.GetProperties(flags);

            var propertyNames = objSourceProperties
            .Select(c => c.Name)
            .ToList();

            foreach (var properties in objDestinationProperties.Where(properties => propertyNames.Contains(properties.Name)))
            {

                try
                {
                    PropertyInfo piSource = source.GetType().GetProperty(properties.Name);

                    properties.SetValue(destination, piSource.GetValue(source, null), null);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        /// <summary>
        /// This method will compare the properties of two objects.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="source">the source object</param>
        /// <param name="target">the target object</param>
        /// <returns>true if the object properties are not equal</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsDifferentFrom<T>(this T source, T target)
        {
            if (source != null && target == null) return true;
            if (target != null && source == null) return true;
            IDictionary<string, object> aa = source.AsDictionary();
            IDictionary<string, object> bb = target.AsDictionary();
            if (bb.Count != aa.Count) return true;
            foreach (KeyValuePair<string, object> pair in aa)
            {
                string aaString = pair.Value.AsString();
                string bbString = bb[pair.Key].AsString();
                if (aaString != null)
                {
                    if (!aaString.Equals(bbString, StringComparison.CurrentCulture))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method will convert a POCO object to XML
        /// </summary>
        /// <param name="dataToSerialize">the POCO object</param>
        /// <returns>XML string</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter</remarks> 
        public static string Serialize(this object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }

        /// <summary>
        /// This method get the properties of type T
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <returns>List of type PropertyInfo</returns>
        public static List<PropertyInfo> GetPropertiesForType<T>()
        {
            Dictionary<Type, List<PropertyInfo>> typeDictionary = new Dictionary<Type, List<PropertyInfo>>();

            var type = typeof(T);
            if (!typeDictionary.ContainsKey(typeof(T)))
            {
                typeDictionary.Add(type, type.GetProperties().ToList());
            }

            return typeDictionary[type];
        }

        /// <summary>
        /// This method derives a SQL Update statement from a POCO class
        /// </summary>
        /// <typeparam name="T">the POCO type</typeparam>
        /// <param name="poco">the POCO object</param>
        /// <param name="tableName">the database table name</param>
        /// <param name="whereClause">the SQL where clause</param>
        /// <returns>string SQL statment</returns>
        public static string AsSqlUpdate<T>(this object poco, string tableName, string whereClause)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"UPDATE {tableName}");
            sb.Append("SET ");
            List<PropertyInfo> properties = GetPropertiesForType<T>();

            foreach (PropertyInfo property in properties)
            {
                if (!property.GetValue(poco).IsNull())
                {
                    if (property.PropertyType == typeof(double) ||
                        property.PropertyType == typeof(decimal) ||
                        property.PropertyType == typeof(float) ||
                        property.PropertyType == typeof(int) ||
                        property.PropertyType == typeof(long))
                    {
                        sb.Append($"[{property.Name}] = {property.GetValue(poco).AsString()},");
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        bool bvalue = property.GetValue(poco).AsBool();
                        sb.Append(bvalue ? $"[{property.Name}] = 1, " : $"[{property.Name}] = 0,");
                    }
                    else //string
                    {
                        sb.Append($"[{property.Name}] = '{PrepValue(property.GetValue(poco).AsString())}',");
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine(whereClause.IsEmpty() ? ";" : $"{Environment.NewLine}{whereClause};");

            return sb.ToString();
        }

        /// <summary>
        /// This method derives a SQL Insert statement from a POCO class
        /// </summary>
        /// <typeparam name="T">the POCO type</typeparam>
        /// <param name="poco">the POCO object</param>
        /// <param name="tableName">the database table name</param>
        /// <returns>string SQL statment</returns>
        public static string AsSqlInsert<T>(this object poco, string tableName)
        {
            List<PropertyInfo> properties = GetPropertiesForType<T>();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"INSERT TO {tableName}");
            sb.Append("(");
            foreach (PropertyInfo property in properties)
            {
                if (!property.GetValue(poco).IsNull())
                {
                    sb.Append($"[{property.Name}],");
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine(")");
            sb.AppendLine(" VALUES ");
            sb.Append("(");

            foreach (PropertyInfo property in properties)
            {
                if (!property.GetValue(poco).IsNull())
                {
                    if (property.PropertyType == typeof(double) ||
                        property.PropertyType == typeof(decimal) ||
                        property.PropertyType == typeof(float) ||
                        property.PropertyType == typeof(int) ||
                        property.PropertyType == typeof(long))
                    {
                        sb.Append($"[{property.Name}] = {property.GetValue(poco).AsString()},");
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        bool bvalue = property.GetValue(poco).AsBool();
                        sb.Append(bvalue ? $"[{property.Name}] = 1, " : $"[{property.Name}] = 0,");
                    }
                    else //string
                    {
                        sb.Append($"[{property.Name}] = '{PrepValue(property.GetValue(poco).AsString())}',");
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine($");");
            return sb.ToString();
        }

        private static object PrepValue(object value)
        {
            if (value is string)
            {
                return value?.AsString().Replace("'", "''");
            }
            else
            {
                return value;
            }
        }
    }
}
