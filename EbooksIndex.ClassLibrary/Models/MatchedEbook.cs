using EbooksIndex.ClassLibrary.Interfaces;
using FuzzySharp;
using FuzzySharp.Extractor;
using System.Text.RegularExpressions;

namespace EbooksIndex.ClassLibrary.Models;

public class MatchedEbook : IMatchedEbook
{
    public string Author { get; set; }
    public string BookText { get; set; }
    public int Pages { get; set; }
    public string Title { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string MD5Hash { get; set; }

    public Dictionary<int, string> MatchedSnippets { get; set; } = new();

    public void LoadMatches(string searchString)
    {
        // \b means to match a word boundary
        string regexString = $@"\b{searchString}\b";
        Regex regex = new Regex(regexString, RegexOptions.IgnoreCase);

        string currentSnippet = "";
        Match match = regex.Match(BookText);

        while (match.Success)
        {
            int currentSnippetStartIndex = match.Index - Constants.Constants.READ_BEFORE_CHARACTERS;
            int currentSnippetEndIndex = match.Index + Constants.Constants.READ_AHEAD_CHARACTERS;
            bool ContinueGrowingCurrentSnippet = true;
            while (match.Success && ContinueGrowingCurrentSnippet)
            {
                if (match.Index <= currentSnippetEndIndex)
                {
                    currentSnippetEndIndex = match.Index + Constants.Constants.READ_AHEAD_CHARACTERS;
                    match = match.NextMatch();
                }
                else
                {
                    ContinueGrowingCurrentSnippet = false;
                }
            }
            currentSnippet = BookText[Math.Max(0, currentSnippetStartIndex - Constants.Constants.READ_BEFORE_CHARACTERS)..Math.Min(BookText.Length, currentSnippetEndIndex)];
            MatchedSnippets.Add(currentSnippetStartIndex, currentSnippet);
            match = regex.Match(BookText, Math.Min(BookText.Length, currentSnippetEndIndex));
        }

    }

    public void MarkMatches(string searchString)
    {
        string upperString = searchString.ToUpper();
        foreach (var item in MatchedSnippets)
        {
            string snippet = item.Value;
            string newSnippet=snippet.Replace(
                searchString, 
                $"<span class=\"fw-bold text-decoration-underline\"> - {upperString} - </span>",
                StringComparison.OrdinalIgnoreCase);
            //string newSnippet = snippet.Replace(searchString, $"_-_-_{searchString}_-_-_", StringComparison.OrdinalIgnoreCase);

            MatchedSnippets[item.Key] = newSnippet ;
        }
    }

    public void LoadFuzzyMatches(string searchString)
    {
        // installed nuget package FuzzySharp for this.
        // uninstall if it turns out to be a failing effort.
        int cutOff = 91; // random number. will have to be moved to configuration
        string currentSnippet = "";
        string[] bookTextArray = BookText.Split(' ');
        IEnumerable<ExtractedResult<string>> matches = Process.ExtractAll(searchString, bookTextArray, cutoff: cutOff);
        foreach(ExtractedResult<string> match in matches)
        {
            MatchedSnippets.Add(match.Index, match.Value);
        }

        

    }
}
