using Ebooks.ClassLibrary.Helpers;
using Ebooks.ClassLIbrary.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Ebooks.ClassLibrary;

public class PdfBook : IEbook
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

    public PdfBook(string filePath, ILogger logger)
    {
        this.filePath = filePath;
        this.logger = logger;
    }

    public void Populate()
    {
        MD5Hash = Md5Functions.ReturnMd5(filePath);
        using PdfDocument document = PdfDocument.Open(filePath);
        if (Helpers.MetadataValidator.ValidateAuthor(document.Information.Author))
        {
            Author = document.Information.Author;
        }
        if (Helpers.MetadataValidator.ValidateTitle(document.Information.Title))
        {
            Title = document.Information.Title;
        }
        Pages = document.NumberOfPages;

        StringBuilder sb = new StringBuilder();
        foreach(Page page in document.GetPages())
        {
            sb.Append(" " + page.Text);
        }
        BookText = Helpers.StringCleaner.CleanString(sb.ToString());
        if (BookText == "")
        {
            logger.Warning("Check file {var} because its text cannot be found", filePath);
        }
    }
}
