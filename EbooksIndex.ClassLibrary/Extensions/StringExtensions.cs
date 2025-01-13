using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Extensions;

public static class StringExtensions
{
    public static int LevenshteinEditDistance(this string s, string t)
    {
        // entire method copied from https://www.c-sharpcorner.com/article/fuzzy-search-in-c-sharp/
        // Special cases
        if (s == t) return 0;
        if (s.Length == 0) return t.Length;
        if (t.Length == 0) return s.Length;
        // Initialize the distance matrix
        int[,] distance = new int[s.Length + 1, t.Length + 1];
        for (int i = 0; i <= s.Length; i++) distance[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) distance[0, j] = j;
        // Calculate the distance
        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }
        // Return the distance
        return distance[s.Length, t.Length];
    }

    public static bool IsComparableToOpenSearchLevenshtein(this string s, string t)
    {
        // https://www.elastic.co/guide/en/elasticsearch/reference/current/common-options.html#fuzziness
        // Elastic uses the following rules for default fuzzy matches (2025 january):
        // a string of 0, 1 or 2 characters must match exactly
        // a string 3, 4 or 5 characters may have a total of 1 edit distance
        // a strong of 6 or more characters may have a total of 2 edit distance
        // furthermore I lowercase every match

        // I feel I am not adhering to the DRY principle here, but to make it work I'll have to do this.

        int MaxDistance = 0;
        if (s.Length < 3) MaxDistance = 0;
        else if (s.Length > 3 && s.Length < 6) MaxDistance = 1;
        else MaxDistance = 2;

        if (s.LevenshteinEditDistance(t.PrepareForLevenshtein()) <= MaxDistance)
        {
            return true;
        }
        return false;
    }
    private static string TrimForLevenshtein(this string s)
    {
        return s.Trim(['.', ',', '!', '@', ':', ';', ' ']);
    }

    internal static string PrepareForLevenshtein(this string s)
    {
        // over time I could implement more than just simple trims and lowercasing.
        // this needs thorough testing again OpenSearch which will happen with time.
        return s.ToLower().TrimForLevenshtein();
    }
}
