using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace EbooksIndex.ClassLibrary.Interfaces;

public interface IMatchedEbook
{
    string Author { get; }
    string BookText { get; }
    int Pages { get; }
    string Title { get; }
    string FilePath { get; }
    string FileName { get; }
    string MD5Hash { get; }

    Dictionary<int,string> MatchedSnippets { get; }

    // Redundant comment, but I am very happy to write that I got this fuzzy logic working in a usable manner!
    void LoadMatches(string searchString);
    void MarkMatches(string searchString);
    void LoadFuzzyMatches(string searchString, ILogger _logger);
    void MarkFuzzyMatches(string searchString, ILogger _logger);
}
