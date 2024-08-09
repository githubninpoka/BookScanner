using BookReader.Interfaces;
using BookReader.UserInteraction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using VersOne.Epub.Schema;

namespace BookReader;

internal class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        IUserInteraction _userInteraction = new UserInteractionConsole(); // for if I want to make a website or wpf or some unit tests.
        IWrite writer = new BookSnippetWriterTxtFile(_userInteraction);

        string directoryToScan = _userInteraction.GetOptionalUserDirectory()?? Constants.Constants.DEFAULT_DIRECTORY_PATH;

        //TODO: introduce a reader class for user input with an interface separating console from functionality
        string searchTerm = _userInteraction.GetOptionalSearchPattern() ?? Constants.Constants.DEFAULT_MATCH_TERM;
        string regexString = $@"\b{searchTerm}\b";
        Regex regex = new Regex(regexString, RegexOptions.IgnoreCase);

        List<string> filePaths = FilesLister.GetFileNames(directoryToScan);
        _userInteraction.CommunicateToUser($"Total number of books to process: {filePaths.Count()}");
        int howFarAreWe = 1;
        foreach (var filePath in filePaths)
        {
            _userInteraction.CommunicateToUser($"Trying book {howFarAreWe} out of {filePaths.Count} {filePath}");
            howFarAreWe++;
            
            IBook bookScanner;
            string fileExt = Path.GetExtension(filePath).ToLower();
            if (fileExt == "." + Enumerations.FileExtensions.epub.ToString())
            {
                bookScanner = new EpubBook(filePath, regex, _userInteraction);
            }
            else
            {
                bookScanner = new PdfBook(filePath, regex, _userInteraction);
            }
            if (bookScanner.HasMatchedSnippets())
            {
                foreach (KeyValuePair<int, string> snippet in bookScanner.MatchesAndIndex)
                {
                    writer.Write(filePath, bookScanner.Title, bookScanner.Author, bookScanner.Pages, snippet.Key, snippet.Value);
                }
            }
        }
        sw.Stop();
        _userInteraction.CommunicateToUser($"Total processing time {sw.ElapsedMilliseconds / 1000}");
    }
}

