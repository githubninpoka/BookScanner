using EbooksIndex.ClassLibrary.Extensions;
using EbooksIndex.ClassLibrary.Interfaces;

using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace EbooksIndex.ClassLibrary.Models;

public class MatchedEbook : IMatchedEbook
{
    public string? Author { get; set; }
    public string? BookText { get; set; }
    public int Pages { get; set; }
    public string? Title { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? MD5Hash { get; set; }

    public Dictionary<int, string> MatchedSnippets { get; set; } = new();

    public void LoadMatches(string searchString)
    {
        MatchedSnippets.Clear(); // if the book was already matched to a searchstring, but is requested again, (possibly) with another searchString.

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
            string newSnippet = snippet.Replace(
                searchString,
                $"<span class=\"fw-bold text-decoration-underline\"> - {upperString} - </span>",
                StringComparison.OrdinalIgnoreCase);

            MatchedSnippets[item.Key] = newSnippet;
        }
    }

    public void LoadFuzzyMatches(string searchString, ILogger _logger)
    {
        // I tried a handful of Nuget packages, but in the end I just rolled my own.
        // FuzzySharp by Jacob Bayer (and several clones): I could not force edit distance like the OpenSearch implementation
        // GSF.Core by Grid Protection Alliance. Very good package, but very slow and plenty of overhead.

        MatchedSnippets.Clear();

        List<int> foundIndexes = new();
        string[] bookTextArray = BookText!.Split(' ');

        for (int i = 0; i < bookTextArray.Length; i++)
        {
            if (searchString.IsComparableToOpenSearchLevenshtein(bookTextArray[i]))
            {
                foundIndexes.Add(i);
            }
        }

        List<BlockInfo> blocks = new();
        for (int i = 0; i < foundIndexes.Count; i++)
        {
            _logger.LogInformation("{var4} - {var5} - Entry {var} is {var2} containing =>{var3}<=",nameof(MatchedEbook),nameof(LoadFuzzyMatches), i, foundIndexes[i], bookTextArray[foundIndexes[i]]);
            int startPosition = Math.Max(0, foundIndexes[i] - Constants.Constants.READ_BEFORE_WORDS_FOR_FUZZY);
            int endPosition = Math.Min(bookTextArray.Length - 1, foundIndexes[i] + Constants.Constants.READ_AHEAD_WORDS_FOR_FUZZY);
            while (i + 1 < foundIndexes.Count && endPosition > foundIndexes[i + 1])
            {
                _logger.LogInformation("{var} - {var2} - Extending snippet because searchterm occurred quickly again",nameof(MatchedEbook),nameof(LoadFuzzyMatches));
                endPosition = Math.Min(BookText.Length - 1, foundIndexes[i + 1] + Constants.Constants.READ_AHEAD_WORDS_FOR_FUZZY);
                i = i + 1;
            }
            blocks.Add(new BlockInfo { MatchMarker = foundIndexes[i], StartPosition = startPosition, EndPosition = endPosition });
        }
        foreach (var item in blocks)
        {
            _logger.LogInformation("{var4} - {var5} - To be matched block: {var1} - {var2} - {var3}",nameof(MatchedEbook),nameof(LoadFuzzyMatches), item.StartPosition, item.MatchMarker, item.EndPosition);
            string[] oneBlockOfWords = new string[item.EndPosition - item.StartPosition];
            Array.Copy(bookTextArray, item.StartPosition, oneBlockOfWords, 0, item.EndPosition - item.StartPosition);
            string snippet = string.Join(' ', oneBlockOfWords);
            MatchedSnippets.Add(item.MatchMarker, snippet);
        }
    }

    public void MarkFuzzyMatches(string searchString, ILogger _logger)
    {
        string upperString = searchString.ToUpper();
        foreach (var item in MatchedSnippets)
        {
            string snippet = item.Value;
            string[] words = snippet.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (searchString.IsComparableToOpenSearchLevenshtein(words[i]))
                {
                    words[i] = $"<span class=\"fw-bold text-decoration-underline\"> - {words[i]} - </span>";
                }
            }
            string newSnippet = string.Join(' ', words);
            MatchedSnippets[item.Key] = newSnippet;
        }
    }
}

record BlockInfo
{
    public int MatchMarker;
    public int StartPosition;
    public int EndPosition;
}
