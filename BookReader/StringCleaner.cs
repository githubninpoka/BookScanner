using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader
{
    internal static class StringCleaner
    {
        internal static string CleanString(string uncleanedText)
        {
            string[] uncleanedArray = uncleanedText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string returnMe = string.Join('\n', uncleanedArray);
            return returnMe;
        }
    }
}
