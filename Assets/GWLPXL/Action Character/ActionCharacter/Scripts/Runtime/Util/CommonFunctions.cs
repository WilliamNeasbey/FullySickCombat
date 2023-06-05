using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
namespace GWLPXL.ActionCharacter
{


    public static class CommonFunctions 
    {
        static StringBuilder sb = new StringBuilder();
        static Dictionary<string, string> stringKeys = new Dictionary<string, string>();
        /// <summary>
        /// helper, compareordinal == 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool WordEquals(string a, string b)
        {
            return string.CompareOrdinal(a, b) == 0;
        }

        /// <summary>
        /// return the string key
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string StringKey(string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                DebugHelpers.DebugMessage("String Key is empty", null, DebugMessageType.Warning);
                return original;
            }
            if (stringKeys.ContainsKey(original) == false)
            {
                sb.Clear();
                sb.Append(original.ToLowerInvariant());
                string key = sb.ToString();
                stringKeys[original] = key;
            }

            return stringKeys[original];
        }

    }
}