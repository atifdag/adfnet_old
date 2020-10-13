using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Adfnet.Core.Helpers
{
    public static class StringHelper
    {
        public static string ClearForHtml(this string str)
        {
            return Regex.Replace(str, @"<[^>]+>|&nbsp;", "").Trim();
        }


        /// <summary>
        /// Enum olarak verilen parametrenin Description değerini döndürür.
        /// </summary>
        /// <param name="e">Enum</param>
        /// <returns>string</returns>
        public static string GetEnumDescription(this Enum e)
        {
            return e.GetType().GetMember(e.ToString()).FirstOrDefault()?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        /// <summary>
        /// string olarak verilen parametrenin değerini SEO'ya (Arama Motoru Optimazsyonu) uygun olarak dönüştürerek geri döndürür.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string ToStringForSeo(this string inputString)
        {
            try
            {
                inputString = inputString.Replace("Ç", "c");
                inputString = inputString.Replace("ç", "c");
                inputString = inputString.Replace("Ğ", "g");
                inputString = inputString.Replace("ğ", "g");
                inputString = inputString.Replace("I", "i");
                inputString = inputString.Replace("ı", "i");
                inputString = inputString.Replace("İ", "i");
                inputString = inputString.Replace("i", "i");
                inputString = inputString.Replace("Ö", "o");
                inputString = inputString.Replace("ö", "o");
                inputString = inputString.Replace("Ş", "s");
                inputString = inputString.Replace("ş", "s");
                inputString = inputString.Replace("Ü", "u");
                inputString = inputString.Replace("ü", "u");
                inputString = inputString.Trim().ToLower();
                inputString = Regex.Replace(inputString, @"\s+", "-");
                inputString = Regex.Replace(inputString, @"[^A-Za-z0-9_-]", "");
                return inputString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        /// <summary>
        /// string olarak verilen parametrenin değerindeki kelimelerin ilk harflerini büyük harfe çevirir.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="orjnaliKullan">false olması durumunda "Ve, İle" bağlaçları küçük yapılır.</param>
        /// <returns></returns>
        public static string ToTitleCase(this string str, bool orjnaliKullan = false)
        {
            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());

            if (orjnaliKullan)
            {
                return result;
            }

            result = result.Replace(" Ve ", " ve ");
            result = result.Replace(" İle ", " ile ");
            return result;
        }

        public static string ListToString(this List<string> list, string delimeter)
        {
            return String.Join(delimeter, list.ToArray());

        }


        public static string TemplateParser(this string templateText, string regExString, string value)
        {
            var regExToken = new Regex(regExString, RegexOptions.IgnoreCase);
            return regExToken.Replace(templateText, value);
        }
    }
}
