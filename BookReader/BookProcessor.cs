using BookScanner.Interfaces;
using BookScanner.Enumerations;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace BookScanner;

class BookProcessor
{
    private long timeSpent = 0;

    private IUserInteraction _userInteraction;
    private IWrite _writer;
    private IProcessedBook _processedBook;
    private Regex _regex;

    private string _filePath;
    private string _fileExt;

    public BookProcessor(string filePath, Regex regex, IWrite writer, IUserInteraction userInteraction)
    {
        _filePath = filePath;
        _regex = regex;
        _writer = writer;
        _userInteraction = userInteraction;
        _fileExt = Path.GetExtension(filePath.ToLower());
    }

    public void Process()
    {
        if (_fileExt == "." + FileExtensions.epub.ToString())
        {
            _processedBook = new ProcessedEpubBook(_filePath, _regex, _userInteraction);
        }
        else
        {
            _processedBook = new ProcessedPdfBook(_filePath, _regex, _userInteraction);
        }
        if (_processedBook.HasMatchedSnippets())
        {
            foreach (KeyValuePair<int, string> snippet in _processedBook.MatchesAndIndex)
            {
                _writer.Write(_filePath, _processedBook.Title, _processedBook.Author, _processedBook.Pages, snippet.Key, snippet.Value);
            }
        }

    }
}

