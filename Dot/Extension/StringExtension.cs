using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Dot.Util;

namespace Dot.Extension
{
    /// <summary>
    /// 字符串相关
    /// </summary>
    public static partial class StringExtension
    {
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string ConcatWith(this string value, params object[] args)
        {
            return string.Concat(value, string.Concat(args));
        }

        public static string Reverse(this string value)
        {
            Ensure.NotNull(value, "value");

            if (value.Length == 0) return string.Empty;
            if (value.Length == 1) return value;

            var chars = value.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public static string Left(this string value, int length)
        {
            Ensure.NotNull(value, "value");
            Ensure.Greater(length, 0, "length");

            return length > value.Length 
                 ? value 
                 : value.Substring(0, length);
        }

        public static string Right(this string value, int length)
        {
            Ensure.NotNull(value, "value");
            Ensure.Greater(length, 0, "length");

            return length > value.Length 
                 ? value 
                 : value.Substring(value.Length - length);
        }

        public static string Between(this string value, int begin, int end)
        {
            Ensure.NotNull(value, "value");
            Ensure.GreaterOrEqual(begin, 0, "begin");
            Ensure.Greater(end, begin, "end");

            return end > value.Length - 1
                 ? value.Substring(begin)
                 : value.Substring(begin, end - begin);
        }

        public static string ToHashText(this string plainText, string hashName = "", Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (string.IsNullOrEmpty(hashName))
                hashName = "MD5";

            using (var algorithm = HashAlgorithm.Create(hashName))
            {
                Ensure.True(algorithm != null, "can not create hash algorithm by hash name = {0}".FormatWith(hashName));

                var plainBytes = encoding.GetBytes(plainText);
                var hashBytes = algorithm.ComputeHash(plainBytes);
                var hashText = BitConverter.ToString(hashBytes).Replace("-", "");

                return hashText;
            }
        }

        public static string UrlEncode(this string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public static string UrlDecode(this string url)
        {
            return HttpUtility.UrlDecode(url);
        }
    }

    /// <summary>
    /// 日期相关
    /// </summary>
    public static partial class StringExtension
    {
        public static DateTime ToDateTime(this string value, string format = "yyyy-MM-dd HH:mm:ss")
        {
            Ensure.NotNullOrEmpty(value, "value");

            return DateTime.ParseExact(value, format, null);
        }

        public static DateTime ToDate(this string value)
        {
            return value.ToDateTime("yyyy-MM-dd");
        }

        public static string ChangeDateTimeFormat(this string value, string oldFormat, string newFormat)
        {
            return value.ToDateTime(oldFormat).ToString(newFormat);
        }
    }

    /// <summary>
    /// 字典相关
    /// </summary>
    public static partial class StringExtension
    {
        public static Dictionary<string, string> ToDictionary(this string source, char columnSeparator, char rowSeparator)
        {
            Func<string, string> keySelector = key => key;
            Func<string, string> valueSelector = value => value;

            return source.ToDictionary<string, string>(columnSeparator, rowSeparator, keySelector, valueSelector);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this string source, char columnSeparator, char rowSeparator, 
            Func<string, TKey> keySelector, Func<string, TValue> valueSelector)
        {
            Ensure.NotNullOrEmpty(source, "source");
            Ensure.NotNull(keySelector, "keySelector");
            Ensure.NotNull(valueSelector, "valueSelector");

            var map = new Dictionary<TKey, TValue>();
            var rows = source.Split(true, rowSeparator);
            foreach (var row in rows)
            {
                var columns = row.Split(true, columnSeparator).ToArray();
                var key = keySelector(columns[0]);
                var value = valueSelector(columns[1]);
                map.Add(key, value);
            }

            return map;
        }
    }

    /// <summary>
    /// 枚举数相关
    /// </summary>
    public static partial class StringExtension
    {
        public static IEnumerable<string> Split(this string source, char separator, bool removeEmpty = true)
        {
            var separators = new char[] { separator };
            return source.Split(removeEmpty, separators);
        }

        public static IEnumerable<string> Split(this string source, bool removeEmpty = true, params char[] separators)
        {
            Ensure.NotNull(source, "source");
            Ensure.NotNullOrEmpty(separators, "separators");

            var option = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return source.Split(separators, option);
        }
    }

    /// <summary>
    /// IO 相关
    /// </summary>
    public static partial class StringExtension
    {
        public static Stream OpenStream(this string filePath, FileMode mode)
        {
            Ensure.True(File.Exists(filePath), "file", "file which path = {0} not exists".FormatWith(filePath));

            return new FileStream(filePath, mode);
        }
    }

    /// <summary>
    /// 正则表达式相关
    /// </summary>
    public static partial class StringExtension
    {
        public static bool IsMatch(this string source, string pattern, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            var options = RegexOptions.Compiled;
            if (ignoreCase)
                options |= RegexOptions.IgnoreCase;

            return Regex.IsMatch(source, pattern, options);
        }

        public static bool IsNotMatch(this string source, string pattern, bool ignoreCase = true)
        {
            return !source.IsMatch(pattern, ignoreCase);
        }
    }
}