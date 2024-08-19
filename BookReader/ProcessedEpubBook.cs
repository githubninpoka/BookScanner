using BookScanner.Interfaces;
using BookScanner.Helpers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VersOne.Epub;

namespace BookScanner;

public class ProcessedEpubBook : IProcessedBook
{
    private IUserInteraction _userInteraction;
    private string _title = "Unknown title";
    private string _author = "Unknown author";
    private string _bookText = "";

    private int _pages = 0;

    private Dictionary<int, string> _matchesAndIndex = new();

    public string Title
    {
        get { return _title; }
        private set { _title = value; }
    }
    public string Author
    {
        get { return _author; }
        private set { _author = value; }
    }
    public string BookText
    {
        get { return _bookText; }
        private set { _bookText = value; }
    }
    public int Pages
    {
        get { return _pages; }
        private set { _pages = value; }
    }
    public Dictionary<int, string> MatchesAndIndex
    {
        get { return _matchesAndIndex; }
        private set { _matchesAndIndex = value; }
    }

    public ProcessedEpubBook(string filePath, Regex regex, IUserInteraction userInteraction)
    {
        _userInteraction = userInteraction;
        PopulateBookText(filePath);
        PopulateMatchesAndIndex(regex);
        if (MatchesAndIndex.Count > 0)
        {
            PopulateMetaData(filePath);
        }
    }

    private void PopulateMetaData(string filePath)
    {
        IList<string> authors = new List<string>();
        IList<string> titles = new List<string>();
        int? numberOfPages = null;
        try
        {
            using (EpubBookRef bookReference = EpubReader.OpenBook(filePath))
            {
                authors = EpubMetadataDelver.GetAuthors(bookReference);
                titles = EpubMetadataDelver.GetTitles(bookReference);
                numberOfPages = EpubMetadataDelver.GetNumberOfPages(bookReference);

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        if (authors.Count > 0)
        {
            Author = authors[0];
        }
        if (titles.Count > 0)
        {
            Title = titles[0];
        }
        if (numberOfPages != 0)
        {
            Pages = numberOfPages ?? 0;
        }
    }

    public (string? author, string? title, int? pages) GetMetaData()
    {
        return (Author, Title, Pages);
    }

    private void PopulateMatchesAndIndex(Regex regex)
    {
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
            MatchesAndIndex.Add(currentSnippetStartIndex, currentSnippet);
            match = regex.Match(BookText, Math.Min(BookText.Length, currentSnippetEndIndex));
        }
    }

    public Dictionary<int, string> GetTextSnippets()
    {
        return MatchesAndIndex;
    }

    private void PopulateBookText(string filePath)
    {
        try
        {
            using (EpubBookRef book = EpubReader.OpenBook(filePath))
            {
                IEnumerable<EpubLocalTextContentFileRef> contentReferences = book.GetReadingOrder();
                StringBuilder stringBuilder = new();
                foreach (var contentPieceReference in contentReferences)
                {
                    HtmlDocument htmlDocument = new();
                    htmlDocument.LoadHtml(contentPieceReference.ReadContent());
                    foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//text()"))
                    {
                        stringBuilder.AppendLine(node.InnerText.Trim());
                    }
                }
                BookText = StringCleaner.CleanString(stringBuilder.ToString());
            }
        }
        catch (Exception ex)
        {
            _userInteraction.CommunicateToUser($"Exception: {ex.Message} for {filePath}");
        }
    }
    public bool HasMatchedSnippets()
    {
        if (MatchesAndIndex.Count > 0)
        {
            return true;
        }
        return false;
    }

}

