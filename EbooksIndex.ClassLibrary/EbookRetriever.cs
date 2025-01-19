using EbooksIndex.ClassLibrary.DataAccess;
using EbooksIndex.ClassLibrary.Interfaces;
using EbooksIndex.ClassLibrary.Models;

using Microsoft.Extensions.Caching.Memory;
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
    // responsible for instantiating a book with its booktext

    private readonly ILogger<EbookRetriever> _logger;
    private readonly OpenSearchClient _db;
    private readonly  IMemoryCache _memoryCache;

    public EbookRetriever(
        ILogger<EbookRetriever> logger,
        OpenSearchAccess openSearchAccess,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _db = openSearchAccess.GetCLient();
        _memoryCache = memoryCache;
    }

    public async Task<IMatchedEbook?> Retrieve(string repositoryId)
    {
        _logger.LogInformation("{var} - {var2} - first step in retrieving a document by id", nameof(EbookRetriever), nameof(Retrieve));
        if (_memoryCache.TryGetValue(repositoryId, out MatchedEbook? returnableEbook))
        {
            _logger.LogInformation("{var} - {var2} - Serving the book {var3} from cache", nameof(EbookRetriever), nameof(Retrieve), repositoryId);
        }
        else
        {
            _logger.LogInformation("{var} - {var2} - Serving the book {var3} fresh", nameof(EbookRetriever), nameof(Retrieve), repositoryId);
            var openSearchResponse = await _db.GetAsync<MatchedEbook>(repositoryId);
            returnableEbook = openSearchResponse.Source;
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(600))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                .SetPriority(CacheItemPriority.Normal);
                
                
            _memoryCache.Set(repositoryId, returnableEbook, cacheOptions);
        }

        return returnableEbook;

    }
}
