using EbooksIndex.ClassLibrary.Models;
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

    void FillSnippets(SearchParameters searchParameters, ILogger _logger);
}
