using ACS_Reception.Domain.Abstractions;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace ACS_Reception.SerializerLib
{
    public partial class Serializer : ISerializer
    {
        public async Task SerializeJson<T>(T obj, string fileName)
        {
            string result = await Task.Run(() => SerializeToStringJson<T>(obj));

            using var stream = new StreamWriter(fileName, false);
            await stream.WriteAsync(result);
            stream.Close();
        }

        public async Task<T> DeserializeJson<T>(string fileName)
        {
            using var stream = new StreamReader(fileName);
            string data = await stream.ReadToEndAsync();

            var result = await Task.FromResult(DeserializeFromStringJson<T>(data)!);
            stream.Close();

            return result;
        }

        private string SerializeToStringJson<T>(T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException("Cannot serialize null object");
            }

            var type = obj.GetType();
            if (type == typeof(string))
            {
                return $"\"{EscapeJson((string)(object)obj)}\"";
            }
            if (type == typeof(bool))
            {
                return obj.ToString()!.ToLower();
            }
            if (type.IsPrimitive || type == typeof(decimal))
            {
                return obj.ToString()!.Replace(',', '.');
            }
            if (type.IsValueType)
            {
                return $"\"{obj}\"";
            }


            if (obj is IEnumerable enumerable && type != typeof(string))
            {
                var sb = new StringBuilder();
                sb.Append('[');

                bool first = true;
                foreach (var item in enumerable)
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }

                    sb.Append(SerializeToStringJson(item));
                    first = false;
                }

                sb.Append(']');
                return sb.ToString();
            }

            if (type.IsClass || type.IsInterface)
            {
                var sb = new StringBuilder();
                sb.Append('{');

                bool first = true;
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    if (!property.CanRead)
                    {
                        continue;
                    }

                    var value = property.GetValue(obj);
                    if (!first)
                    {
                        sb.Append(',');
                    }

                    sb.Append($"\"{property.Name}\":");
                    sb.Append(SerializeToStringJson(value));
                    first = false;
                }

                sb.Append('}');
                return sb.ToString();
            }

            throw new NotSupportedException($"Type {type.FullName} is not supported");
        }

        public T? DeserializeFromStringJson<T>(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return default;
            }

            int index = 0;
            object? parsedValue = ParseValueJson(data, ref index);
            return ConvertToTargetType<T>(parsedValue);
        }

        private T ConvertToTargetType<T>(object? parsedValue)
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

            if (parsedValue is Dictionary<string, object?> jsonObj)
            {
                if (targetType.IsClass || targetType.IsInterface)
                {
                    var instance = Activator.CreateInstance<T>();
                    var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite)
                        .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

                    foreach (var kvp in jsonObj)
                    {
                        if (properties.TryGetValue(kvp.Key, out var property))
                        {
                            try
                            {
                                object? propertyValue = ConvertToTargetType(property.PropertyType, kvp.Value);
                                property.SetValue(instance, propertyValue);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning: Could not convert JSON value for property '{property.Name}' of type {property.PropertyType.FullName}. Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Warning: JSON key '{kvp.Key}' has no matching writable property in type {targetType.FullName}");
                        }
                    }

                    return instance;
                }
            }

            if (parsedValue is List<object?> jsonArray)
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = targetType.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(targetType)!;

                    foreach (var item in jsonArray)
                    {
                        try
                        {
                            object? convertedItem = ConvertToTargetType(itemType, item);
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
                    object? convertedToUnderlying = ConvertToTargetType(nullableUnderlyingType, parsedValue);
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

        private object? ConvertToTargetType(Type targetType, object? parsedValue)
        {
            try
            {
                MethodInfo? convertMethod = typeof(Serializer)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == nameof(ConvertToTargetType)
                    && m.IsGenericMethodDefinition && m.GetParameters().Length == 1)
                    ?? throw new InvalidOperationException("Internal serializer error: ConvertToTargetType generic method not found.");

                MethodInfo? genericConvertMethod = convertMethod.MakeGenericMethod(targetType);
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

        private object? ParseValueJson(string json, ref int index)
        {
            SkipWhitespace(json, ref index);
            if (index >= json.Length)
            {
                throw new FormatException("Enexpected end in JSON string.");
            }

            char currentChar = json[index];
            switch (currentChar)
            {
                case '{':
                    return ParseObjectJson(json, ref index);
                case '[':
                    return ParseArrayJson(json, ref index);
                case '"':
                    return ParseStringJson(json, ref index);
                case 't':
                    return ParseLiteralJson(json, ref index, "true");
                case 'f':
                    return ParseLiteralJson(json, ref index, "false");
                case 'n':
                    return ParseLiteralJson(json, ref index, "null");
                case '-':
                case var _
                    when char.IsDigit(currentChar):
                        return ParseNumberJson(json, ref index);
                default:
                    throw new InvalidDataException($"Enexpected symbol during value parsing '{currentChar}' at position {index}");
            }
        }

        private string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        private object ParseNumberJson(string json, ref int index)
        {
            int startIndex = index;
            while (index < json.Length)
            {
                char currentChar = json[index];
                if (char.IsDigit(currentChar) || currentChar == '-' || currentChar == '.')
                {
                    ++index;
                }
                else
                {
                    break;
                }
            }

            string numString = json[startIndex..index];
            if (decimal.TryParse(numString, CultureInfo.InvariantCulture, out decimal decimalValue))
            {
                return decimalValue;
            }

            if (double.TryParse(numString, CultureInfo.InvariantCulture, out double doubleValue))
            {
                return doubleValue;
            }

            if (int.TryParse(numString, CultureInfo.InvariantCulture, out int intValue))
            {
                return intValue;
            }

            throw new FormatException($"Invalid number format: '{numString}' at position {startIndex}");
        }

        private List<object?> ParseArrayJson(string json, ref int index)
        {
            if (json[index] != '[')
            {
                throw new FormatException($"Expected '[' at position {index}");
            }
            ++index;

            SkipWhitespace(json, ref index);
            var list = new List<object?>();
            if (index < json.Length && json[index] == ']')
            {
                ++index;
                return list;
            }

            while (index < json.Length)
            {
                SkipWhitespace(json, ref index);
                object? value = ParseValueJson(json, ref index);
                list.Add(value);

                SkipWhitespace(json, ref index);
                char separator = json[index];
                if (separator == ',')
                {
                    index++;
                }
                else if (separator == ']')
                {
                    index++;
                    return list;
                }
                else
                {
                    throw new FormatException($"Expected ',' or ']' after array element at position {index}");
                }
            }

            throw new FormatException("Enexpected string end during array parsing");
        }

        private Dictionary<string, object?> ParseObjectJson(string json, ref int index)
        {
            if (json[index] != '{')
            {
                throw new FormatException($"Expected '{{' at position {index}");
            }
            ++index;

            SkipWhitespace(json, ref index);
            var objDict = new Dictionary<string, object?>();
            if (index < json.Length && json[index] == '}')
            {
                ++index;
                return objDict;
            }

            while (index < json.Length)
            {
                SkipWhitespace(json, ref index);
                if (index >= json.Length || json[index] != '\"')
                {
                    if (json[index] == '}')
                    {
                        ++index;
                        return objDict;
                    }

                    throw new FormatException($"Expected key (string in \"\") or '}}' at position {index}");
                }

                string key = ParseStringJson(json, ref index);
                SkipWhitespace(json, ref index);
                if (index >= json.Length || json[index] != ':')
                {
                    throw new FormatException($"Expected ':' after key '{key}' at position {index}");
                }

                ++index;
                object? value = ParseValueJson(json, ref index);
                objDict[key] = value;

                SkipWhitespace(json, ref index);
                if (json[index] == ',')
                {
                    ++index;
                }
                else if (json[index] == '}')
                {
                    ++index;
                    return objDict;
                }
                else
                {
                    throw new FormatException($"Expected '}}' after key '{key}' at position '{index}'");
                }
            }

            throw new FormatException("Unexpected string end during parsing");
        }

        private string ParseStringJson(string json, ref int index)
        {
            if (json[index] != '\"')
            {
                throw new FormatException($"Expected '\"' at position {index}");
            }

            ++index;
            var sb = new StringBuilder();
            bool escaped = false;
            while (index < json.Length)
            {
                char currChar = json[index++];
                if (escaped)
                {
                    switch (currChar)
                    {
                        case '"':
                            sb.Append('"');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case '/':
                            sb.Append('/');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        default:
                            sb.Append(currChar);
                            break;
                    }

                    escaped = false;
                }
                else if (currChar == '\\')
                {
                    escaped = true;
                }
                else if (currChar == '"')
                {
                    return sb.ToString();
                }
                else
                {
                    sb.Append(currChar);
                }
            }

            throw new FormatException("Unexpected string end during parsing (missing closing \").");
        }

        private object? ParseLiteralJson(string json, ref int index, string literal)
        {
            if (index + literal.Length > json.Length || json.Substring(index, literal.Length) != literal)
            {
                throw new FormatException($"Expected literal '{literal}' at position {index}");
            }

            index += literal.Length;
            return literal switch
            {
                "true" => true,
                "false" => false,
                "null" => null,
                _ => throw new InvalidOperationException("Unknown literal"),
            };
        }

        private void SkipWhitespace(string str, ref int index)
        {
            while (index < str.Length && char.IsWhiteSpace(str[index]))
            {
                ++index;
            }
        }
    }
}
