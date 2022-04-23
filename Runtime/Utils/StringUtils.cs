using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CippSharp.Members
{
    internal static class StringUtils
    {
        #region Log Name

        /// <summary>
        /// Retrieve a more contextual name for logs, based on typeName.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string LogName(string typeName)
        {
            return string.Format("[{0}]: ", typeName);
        }
        
        /// <summary>
        /// Retrieve a more contextual name for logs, based on type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string LogName(Type type)
        {
            return string.Format("[{0}]: ", type.Name);
        }

        /// <summary>
        /// Retrieve a more contextual name for logs, based on object.
        /// If object is null an empty string is returned.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string LogName(object context)
        {
            return ((object)context == null) ? string.Empty : LogName(context.GetType());
        }
        
        #endregion
        
        #region Firt Char to
        
        /// <summary>
        /// Put the first Char of a string to UpperCase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return string.Format("{0}{1}", value.First().ToString().ToUpper(), value.Substring(1));
        }
        
        /// <summary>
        /// Put the first Char of a string to LowerCase
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToLower(string input)
        {
            string newString = input;
            if (!string.IsNullOrEmpty(newString) && char.IsUpper(newString[0]))
            {
                newString = char.ToLower(newString[0]) + newString.Substring(1);
            }
            return newString;
        }
        
        #endregion

        #region Split

        /// <summary>
        /// Perform the split only if it is possible, otherwise retrieve
        /// an array of length 1 with value as element.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="strings"></param>
        /// <param name="splitOptions"></param>
        /// <returns></returns>
        public static string[] CheckedSplit(string value, string[] strings, StringSplitOptions splitOptions)
        {
            if (string.IsNullOrEmpty(value))
            {
                return splitOptions.HasFlag(StringSplitOptions.RemoveEmptyEntries) ? new string[0] : new[] {value};
            }

            if (ArrayUtils.IsNullOrEmpty(strings))
            {
                return new[] {value};
            }

            if (!ContainsAnyString(value, strings))
            {
                return new[] {value};
            }

            return value.Split(strings, splitOptions);
        }

        /// <summary>
        /// The value contains Any of the strings?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static bool ContainsAnyString(string value, string[] strings)
        {
            return strings.Any(value.Contains);
        }

        /// <summary>
        /// Is any string == the other?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static bool EqualAnyString(string value, string[] strings)
        {
            return strings.Any(s => s == value);
        }


        #endregion
        
        #region Replacement

        /// <summary>
        /// Remove special characters from a string 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string input, string replace = "")
        {
            Regex reg = new Regex("[*'\",_&#^@]");
            input = reg.Replace(input, replace);

            Regex reg1 = new Regex("[ ]");
            input = reg.Replace(input, "-");
            return input;
        }
        
        /// <summary>
        /// Replace last occurrence of match.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceLastOccurrence(string source, string match, string replace)
        {
            int place = source.LastIndexOf(match);

            if (place == -1)
            {
                return source;
            }

            string result = source.Remove(place, match.Length).Insert(place, replace);
            return result;
        }
        
        /// <summary>
        /// Replace empty lines
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceEmptyLine(string lines, string replace = "")
        {
            return Regex.Replace(lines, @"^\s*$\n|\r", replace, RegexOptions.Multiline).TrimEnd();
        }
        
        /// <summary>
        /// Replace new lines
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceNewLine(string lines, string replace = "")
        {
            return Regex.Replace(lines, @"\r\n?|\n", replace, RegexOptions.Multiline).TrimEnd();
        }
        
        
        /// <summary>
        /// Replace any of the values with the replace value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="values"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceAny(string input, string[] values, string replace = "")
        {
            return values.Aggregate(input, (current, t) => current.Replace(t, replace));
        }

        #endregion
        
        /// <summary>
        /// Write a string array as flat string
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToFlatArray(IEnumerable<string> enumerable, string separator = "")
        {
            string[] array = enumerable.ToArray();
            string last = array.Last();
            return array.Aggregate(string.Empty, (current, s) => current + $"{s}{(s != last ? separator : string.Empty)}");
        }
    }
}
