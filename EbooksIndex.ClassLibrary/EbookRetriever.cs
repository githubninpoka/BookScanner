using EbooksIndex.ClassLibrary.DataAccess;
using EbooksIndex.ClassLibrary.Interfaces;
using EbooksIndex.ClassLibrary.Models;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary;

public class EbookRetriever
{
    private readonly ILogger<EbookRetriever> _logger;
    private readonly OpenSearchClient _db;

    // responsible for instantiating a book with its booktext
    private MatchedEbook? _matchedEbook;

    public EbookRetriever(
        ILogger<EbookRetriever> logger, 
        OpenSearchAccess openSearchAccess
        )
    {
        _logger = logger;
        _db = openSearchAccess.GetCLient();
    }

    public async Task<IMatchedEbook> Retrieve(string repositoryId)
    {
        _logger.LogDebug("first step in retrieving a document by id");

        var oneDocument= await _db.GetAsync<MatchedEbook>(repositoryId);
        return oneDocument.Source;
        
    }
}
