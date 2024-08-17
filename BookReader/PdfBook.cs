﻿using BookScanner.Interfaces;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using BookScanner.Constants;
using System.Collections.Generic;
using BookScanner.Helpers;


namespace BookScanner;

public class PdfBook : IBook
{
    private IUserInteraction _userInteraction;

    private string _title = "";
    private string _author = "";
    private string _bookText = "";

    private int _pages = 0;

    private Dictionary<int, string> _matchesAndIndex = new();

    public string Title
    {
        get
        {
            return _title;
        }
        private set
        {
            _title = value;
        }
    }
    public string Author
    {
        get
        {
            return _author;
        }
        set
        {
            _author = value;
        }
    }
    public string BookText
    {
        get
        {
            return _bookText;
        }
        set
        {
            _bookText = value;
        }
    }

    public int Pages
    {
        get
        {
            return _pages;
        }
        set
        {
            _pages = value;
        }
    }

    public Dictionary<int, string> MatchesAndIndex
    {
        get
        {
            return _matchesAndIndex;
        }
        private set
        {
            _matchesAndIndex = value;
        }
    }

    public PdfBook(string filePath, Regex regex, IUserInteraction userInteraction)
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
        using (PdfDocument document = PdfDocument.Open(filePath))
        {
            if (Validator.ValidateAuthor(document.Information.Author))
            {
                Author = document.Information.Author;
            }
            if (Validator.ValidateTitle(document.Information.Title))
            {
                Title = document.Information.Title;
            }
            Pages = document.NumberOfPages;
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
            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (Page page in document.GetPages())
                {
                    BookText += " " + page.Text;
                    //string pageText="";
                    //foreach (Letter c in page.Letters)
                    //{
                    //    pageText = pageText + c.Value;
                    //}
                    //BookText += pageText;
                    
                }
            }
            BookText = StringCleaner.CleanString(BookText);
        }
        catch (Exception ex)
        {
            _userInteraction.CommunicateToUser($"Caught an exception in {filePath} : {ex.Message}");
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
