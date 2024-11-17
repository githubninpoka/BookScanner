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
    void LoadMatches(string searchString);
    void MarkMatches(string searchString);
}
