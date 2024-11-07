using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebooks.ClassLibrary.Helpers;

public static class StringCleaner
{
    public static string CleanString(string uncleanedText)
    {
        string[] uncleanedArray = uncleanedText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        string cleanedString = string.Join('\n', uncleanedArray);
        return cleanedString;
    }
}
