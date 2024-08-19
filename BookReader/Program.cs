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
using System.Buffers;

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
    // made a bookProcessor for offloading some of the tasks from Main

    // Multithreading & async:
    // I tried multithreading but quickly found there was no real gain to be had in parallel processing
    // this having to do with both the used Pdf and Epub library not having awaitable methods.

    // what I learned about the domain:
    // especially pdfs can be hard to process. most of the time there is no metadata available
    // and the text, because pdfs are made for displaying properly and not for automatic processing,
    // can be hard to get in a normal readable way.


    static void Main(string[] args)
    {
        IUserInteraction _userInteraction = new UserInteractionConsole(); // for if I want to make a website or wpf or some unit tests.

        string directoryToScan = _userInteraction.GetOptionalUserDirectory() ?? Constants.Constants.DEFAULT_DIRECTORY_PATH;
        string searchTerm = _userInteraction.GetOptionalSearchPattern() ?? Constants.Constants.DEFAULT_MATCH_TERM;
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
            _userInteraction.CommunicateToUser($"Adding book {howManyBooksHaveBeenProcessed} out of {filePaths.Count} {filePath}");
            howManyBooksHaveBeenProcessed++;

            BookProcessor bookProcessor = new BookProcessor(filePath, regex, writer, _userInteraction);
            bookProcessor.Process();
            
        }
        sw.Stop();
        _userInteraction.CommunicateToUser($"Total processing time {sw.ElapsedMilliseconds / 1000}");
        Console.ReadLine();
    }
    
}

