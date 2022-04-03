using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorLib
{
    public static class BasicToolsExtensions
    {
        //string
        public static string TrimEnd(this string original, string trimString)
        {
            if (original.EndsWith(trimString))
            {
                return original.Substring(0, original.Length - trimString.Length);
            }
            else
            {
                return original;
            }
        }

        public static string TrimStart(this string original, string trimString)
        {
            if (original.StartsWith(trimString))
            {
                return original.Substring(trimString.Length);
            }
            else
            {
                return original;
            }
        }
    }
}