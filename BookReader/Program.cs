using BookScanner.Interfaces;
using BookScanner.UserInteraction;
using BookScanner.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using VersOne.Epub.Schema;

namespace BookScanner;

internal class Program
{
    // what I like:
    // the functionality of the application is useful to me.
    // i messed around until i had it working for PDF and ePub
    // then i started refactoring, but not trying to overdo it.
    // using an interface for the books themselves, the writer and the userinteraction.
    // playing with regex.
    // the project uses constants and enums. have tried to avoid magic number anti pattern
    // found a use for null-coalescing operator

    static void Main(string[] args)
    {
        IUserInteraction _userInteraction = new UserInteractionConsole(); // for if I want to make a website or wpf or some unit tests.

        string directoryToScan = _userInteraction.GetOptionalUserDirectory()?? Constants.Constants.DEFAULT_DIRECTORY_PATH;
        string searchTerm = _userInteraction.GetOptionalSearchPattern()?? Constants.Constants.DEFAULT_MATCH_TERM;
        string outputDestination = _userInteraction.GetOptionalOutputDestination() ?? Constants.Constants.DEFAULT_OUTPUT_TXT_FILE;
        
        IWrite writer = new BookSnippetWriterTxtFile(_userInteraction, outputDestination);

        Stopwatch sw = Stopwatch.StartNew();

        string regexString = $@"\b{searchTerm}\b";
        Regex regex = new Regex(regexString, RegexOptions.IgnoreCase);

        List<string> filePaths = FilesLister.GetFileNames(directoryToScan);
        _userInteraction.CommunicateToUser($"Total number of books to process: {filePaths.Count()}");
        int howManyBooksHaveBeenProcessed = 1;
        foreach (var filePath in filePaths)
        {
            _userInteraction.CommunicateToUser($"Trying book {howManyBooksHaveBeenProcessed} out of {filePaths.Count} {filePath}");
            howManyBooksHaveBeenProcessed++;
            
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

