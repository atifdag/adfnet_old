using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ADF.Net.Core.Helpers
{
    public static class TypeHelper
    {

        public static byte ToByte(this object obj, byte defaultValue = default)
        {

            if (obj == null)
            {
                return defaultValue;
            }

            return !byte.TryParse(obj.ToString(), out var result) ? defaultValue : result;

        }

        public static short ToShort(this object obj, short defaultValue = default)
        {

            if (obj == null)
            {

                return defaultValue;

            }

            return !short.TryParse(obj.ToString(), out var result) ? defaultValue : result;

        }


        public static int ToInt(this object obj, int defaultValue = default)
        {

            if (obj == null)
            {

                return defaultValue;

            }

            return !int.TryParse(obj.ToString(), out var result) ? defaultValue : result;

        }


        public static double ToDouble(this object obj, double defaultValue = default)
        {

            if (obj == null)
            {

                return defaultValue;

            }

            return !double.TryParse(obj.ToString(), out var result) ? defaultValue : result;

        }


        public static decimal ToDecimal(this object obj, decimal defaultValue = default)
        {

            if (obj == null)
            {

                return defaultValue;

            }

            return !decimal.TryParse(obj.ToString(), out var result) ? defaultValue : result;

        }


        public static DateTime ToDateTime(this object obj, DateTime defaultValue = default)
        {

            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
            {

                return defaultValue;

            }

            return !DateTime.TryParse(obj.ToString(), out var result) ? defaultValue : result;

        }


        public static bool IsNumeric(this string input)
        {

            return int.TryParse(input, out var result);

        }


        public static bool ToBoolean(this string str)
        {

            string[] trueStrings = { "1", "y", "yes", "true", "evet", "on" };

            string[] falseStrings = { "0", "n", "no", "false", "hayır", "hayir", "off" };

            if (trueStrings.Contains(str, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            if (falseStrings.Contains(str, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new InvalidCastException("Yalnızca şu ifadeler dönüştürülür: " + string.Join(", ", trueStrings) + " ve " + string.Join(", ", falseStrings));

        }


        public static T DeserializeFromString<T>(this string data)
        {

            var bytes = Convert.FromBase64String(data);

            using var stream = new MemoryStream(bytes);

            var formatter = new BinaryFormatter();

            stream.Seek(0, SeekOrigin.Begin);

            return (T)formatter.Deserialize(stream);

        }

        public static string SerializeToString<T>(this T data)
        {

            using var stream = new MemoryStream();

            var formatter = new BinaryFormatter();

            formatter.Serialize(stream, data);

            stream.Flush();

            stream.Position = 0;

            return Convert.ToBase64String(stream.ToArray());

        }

        public static T ToEnum<T>(this string value)
        {

            return (T)Enum.Parse(typeof(T), value, true);

        }



    }
}