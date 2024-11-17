using EbooksIndex.ClassLibrary.Interfaces;
using OpenSearch.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Models;

public class BookMetaData : IEbookMetaData
{
    public string? Author { get; set; }
    public int Pages { get; set; }

    public string? Title { get; set; }

    public string? FilePath { get; set; }

    public string? FileName { get; set; }

    public string? MD5Hash { get; set; }

    // the OpenSearch Id is set separately in the query
    public string? OpenSearchId { get; set; }
}
