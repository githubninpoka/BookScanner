using Ebooks.ClassLibrary.Helpers;
using Ebooks.ClassLIbrary.Interfaces;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VersOne.Epub;

namespace Ebooks.ClassLibrary;

public class EpubBook : IEbook
{
    private readonly string filePath;
    private readonly ILogger logger;

    public string Author { get; set; } = "";

    public int Pages { get; set; } = 0;

    public string Title { get; set; } = "";

    public string FilePath
    {
        get
        {
            return filePath;
        }
    }

    public string FileName
    {
        get
        {
            return Path.GetFileName(FilePath);
        }
    }
    public string MD5Hash { get; set; } = "";
    public string BookText { get; set; } = "";

    public EpubBook(string filePath, ILogger logger)
    {
        this.filePath = filePath;
        this.logger = logger;
    }

    public void Populate()
    {

        try
        {
            MD5Hash = Md5Functions.ReturnMd5(filePath);
            using EpubBookRef bookRef = EpubReader.OpenBook(filePath);
            Title = bookRef.Title;
            Author = bookRef.Author;
            IEnumerable<EpubLocalTextContentFileRef> contentReferences = bookRef.GetReadingOrder();
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
            if (BookText == "")
            {
                logger.Warning("Check file {var} because its text cannot be found", filePath);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Exception processing {var}: {var2}", filePath, ex.Message);
            // throw;
        }
    }
}
