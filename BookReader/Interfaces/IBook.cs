using System.Collections.Generic;

namespace BookReader.Interfaces
{
    public interface IBook
    {
        string Title { get; }
        string Author { get; }
        int Pages { get; }
        Dictionary<int,string> MatchesAndIndex { get; }


        (string? author, string? title, int? pages) GetMetaData();
        Dictionary<int, string> GetTextSnippets();
        bool HasMatchedSnippets();
    }
}