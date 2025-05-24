using ACS_Reception.Domain.Abstractions;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace ACS_Reception.SerializerLib
{
    public partial class Serializer : ISerializer
    {
        public async Task SerializeXml<T>(T obj, string fileName)
        {
            string result = await Task.Run(() => SerializeToStringXml<T>(obj));

            using var stream = new StreamWriter(fileName, false);
            await stream.WriteAsync(result);
            stream.Close();
        }

        public async Task<T> DeserializeXml<T>(string fileName)
        {
            using var stream = new StreamReader(fileName);
            string data = await stream.ReadToEndAsync();

            var result = await Task.FromResult(DeserializeFromStringXml<T>(data)!);
            stream.Close();

            return result;
        }

        private string SerializeToStringXml<T>(T obj)
        {
            var sb = new StringBuilder();
            //sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
            if (obj == null)
            {
                throw new NotSupportedException("Root object cannot be null");
            }

            string rootName = GetElementName(typeof(T));
            SerializeValue(sb, obj, rootName);
            return sb.ToString();
        }

        private void SerializeValue(StringBuilder sb, object? obj, string elementName)
        {
            if (string.IsNullOrWhiteSpace(elementName) || elementName.Contains(" ") ||
                elementName.StartsWith("xml", StringComparison.OrdinalIgnoreCase)
                || elementName.Contains('<') || elementName.Contains('>') || elementName.Contains('&'))
            {
                elementName = "InvalidElementName";
                Console.WriteLine($"Warning: Invalid element name '{elementName}', using '{elementName}'");
            }

            if (obj == null)
            {
                sb.Append($"<{elementName}/>");
                return;
            }

            var type = obj.GetType();

            if (type.IsPrimitive || type == typeof(string)
                                 || type == typeof(decimal) || type.IsValueType)
            {
                sb.Append($"<{elementName}>");
                string stringValue = obj.ToString() ?? "";

                sb.Append(EscapeXml(stringValue));
                sb.Append($"</{elementName}>");
                return;
            }

            if (obj is IEnumerable enumerable)
            {
                string itemElementName = "Item";
                Type? itemType = null;

                if (type.IsGenericType)
                {
                    itemType = type.GetGenericArguments().FirstOrDefault();
                }

                if (itemType == null)
                {
                    var firstItem = enumerable.Cast<object?>().FirstOrDefault();
                    if (firstItem != null)
                    {
                        itemType = firstItem.GetType();
                    }
                }

                if (itemType != null)
                {
                    itemElementName = GetElementName(itemType);
                }


                sb.Append($"<{elementName}>");
                foreach (var item in enumerable)
                {
                    SerializeValue(sb, item, itemElementName);
                }

                sb.Append($"</{elementName}>");
                return;
            }

            if (type.IsClass || type.IsInterface)
            {
                sb.Append($"<{elementName}>");

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    if (!property.CanRead) continue;

                    var value = property.GetValue(obj);

                    SerializeValue(sb, value, property.Name);
                }

                sb.Append($"</{elementName}>");
                return;
            }

            throw new NotSupportedException($"Serialization of type {type.FullName} is not supported");
        }

        private string GetElementName(Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
            {
                name = name[..name.IndexOf('`')];
            }

            name = Regex.Replace(name, @"[^a-zA-Z0-9_.-]", "");
            if (!char.IsLetter(name.FirstOrDefault()) && name.FirstOrDefault() != '_')
            {
                name = "_" + name;
            }

            if (string.IsNullOrWhiteSpace(name)) name = "Object";

            return name;
        }

        private string EscapeXml(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("&", "&")
                .Replace("<", "<")
                .Replace(">", ">")
                .Replace("\"", "\"")
                .Replace("'", "'");
        }

        public T? DeserializeFromStringXml<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return default;
            }

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.MoveToContent();

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                return ParseXmlList<T>(reader);
            }

            object? parsedValue = ParseXmlElement(reader);
            return ConvertToTargetTypeXml<T>(parsedValue);
        }

        private T ParseXmlList<T>(XmlReader reader)
        {
            Type itemType = typeof(T).GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance<T>()!;

            if (reader.NodeType == XmlNodeType.Element)
            {
                reader.ReadStartElement();
            }

            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    object? item = ParseXmlElement(reader);
                    object? convertedItem = ConvertToTargetTypeXml(itemType, item);
                    list.Add(convertedItem);
                }
                else
                {
                    reader.Read();
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                reader.ReadEndElement();
            }

            return (T)list;
        }

        private object? ParseXmlElement(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                return null;
            }

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return null;
            }

            string elementName = reader.LocalName;
            reader.Read();

            if (reader.NodeType == XmlNodeType.Text)
            {
                string value = reader.Value;
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.Read();
                }
                return ParseXmlValue(value);
            }

            var dict = new Dictionary<string, object?>();
            var currentList = new List<object?>();
            string? currentElementName = null;

            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string name = reader.LocalName;
                    object? value = ParseXmlElement(reader);

                    if (currentElementName == name)
                    {
                        currentList.Add(value);
                        if (!dict.ContainsKey(name))
                        {
                            dict.Add(name, currentList);
                        }
                    }
                    else
                    {
                        currentElementName = name;
                        currentList = [value];

                        if (!dict.ContainsKey(name))
                        {
                            dict.Add(name, value);
                        }
                    }
                }
                else
                {
                    reader.Read();
                }
            }

            reader.Read();

            if (dict.Count == 1 && dict.Values.First() is List<object?> list)
            {
                return list;
            }

            return dict.Count > 0 ? dict : null;
        }

        private object? ParseXmlValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (bool.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
            {
                if (decimalValue == Math.Floor(decimalValue))
                {
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                    {
                        return intValue;
                    }
                    if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue))
                    {
                        return longValue;
                    }
                }
                return decimalValue;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
            {
                return dateValue;
            }

            if (Guid.TryParse(value, out Guid guidValue))
            {
                return guidValue;
            }

            return value;
        }

        private T ConvertToTargetTypeXml<T>(object? parsedValue)
        {
            if (parsedValue == null)
            {
                return default!;
            }

            var targetType = typeof(T);
            var parsedType = parsedValue.GetType();

            if (targetType.IsAssignableFrom(parsedType))
            {
                return (T)parsedValue;
            }

            if (parsedValue is Dictionary<string, object?> xmlObj)
            {
                if (targetType.IsClass || targetType.IsInterface)
                {
                    var instance = Activator.CreateInstance<T>();
                    var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite)
                        .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

                    foreach (var kvp in xmlObj)
                    {
                        if (properties.TryGetValue(kvp.Key, out var property))
                        {
                            try
                            {
                                object? propertyValue = ConvertToTargetTypeXml(property.PropertyType, kvp.Value);
                                property.SetValue(instance, propertyValue);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning: Could not convert XML value for property '{property.Name}' of type {property.PropertyType.FullName}. Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Warning: XML element '{kvp.Key}' has no matching writable property in type {targetType.FullName}");
                        }
                    }

                    return instance;
                }
            }

            if (parsedValue is List<object?> xmlArray)
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = targetType.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(targetType)!;

                    foreach (var item in xmlArray)
                    {
                        try
                        {
                            object? convertedItem = ConvertToTargetTypeXml(itemType, item);
                            list.Add(convertedItem);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Warning: Could not convert item in array to type {itemType.FullName}. Error: {ex.Message}");
                        }
                    }

                    return (T)list;
                }

                Console.WriteLine($"Warning: Deserialization to collection type {targetType.FullName} is not fully supported. Only List<TItem> is explicitly handled.");
            }

            try
            {
                Type nullableUnderlyingType = Nullable.GetUnderlyingType(targetType)!;
                if (nullableUnderlyingType != null)
                {
                    object? convertedToUnderlying = ConvertToTargetTypeXml(nullableUnderlyingType, parsedValue);
                    return (T)convertedToUnderlying!;
                }

                if (targetType.IsEnum && parsedValue is string enumString)
                {
                    return (T)Enum.Parse(targetType, enumString, ignoreCase: true);
                }

                if (targetType == typeof(Guid) && parsedValue is string guidString)
                {
                    return (T)(object)Guid.Parse(guidString);
                }

                if (targetType == typeof(DateTime) && parsedValue is string dateTimeString)
                {
                    return (T)(object)DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);
                }

                return (T)Convert.ChangeType(parsedValue, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Failed to parse value '{parsedValue}' (type {parsedType.FullName}) to target type {targetType.FullName}", ex);
            }
        }

        private object? ConvertToTargetTypeXml(Type targetType, object? parsedValue)
        {
            try
            {
                MethodInfo convertMethod = typeof(Serializer)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(m => m.Name == nameof(ConvertToTargetTypeXml)
                        && m.IsGenericMethodDefinition
                        && m.GetParameters().Length == 1);

                MethodInfo genericConvertMethod = convertMethod.MakeGenericMethod(targetType);
                return genericConvertMethod.Invoke(this, [parsedValue]);
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to perform recursive conversion to type {targetType.FullName}", ex);
            }
        }
    }
}
