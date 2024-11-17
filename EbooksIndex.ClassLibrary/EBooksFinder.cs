using EbooksIndex.ClassLibrary.DataAccess;
using EbooksIndex.ClassLibrary.Interfaces;
using EbooksIndex.ClassLibrary.Models;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace EbooksIndex.ClassLibrary;

public class EBooksFinder
{
    //responsible for finding books with a match
    // not responsible for loading contents
    // or loading matches

    private ILogger<EBooksFinder> _logger;

    private List<IEbookMetaData> booksMetadata = new();

    private OpenSearchClient _db;

    public EBooksFinder(
        OpenSearchAccess openSearchAccess,
        ILogger<EBooksFinder> logger
        )
    {
        _logger = logger;
        _db = openSearchAccess.GetCLient();
    }
    public async Task<(int, List<IEbookMetaData>)> FindEBooksAsync(SearchParameters searchParameters)
    {
        _logger.LogDebug("first step in find ebooks async method");
        _logger.LogInformation("Logging with Referencebooks set to {var}", searchParameters.OnlyReference);
        ISearchResponse<BookMetaData> searchResponse;
        if (searchParameters.OnlyReference)
        {
            _logger.LogInformation("calling OS with References");
            searchResponse = await _db.SearchAsync<BookMetaData>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            mu => mu.MatchPhrase(m => m.Field("filePath").Query("GoToReferenceBooks")),
                            mu => mu.MatchPhrase(m => m.Field("bookText").Query(searchParameters.SingleSearchString))
                            )
                        )
                    )
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.Title,
                            f => f.Pages,
                            f => f.Author,
                            f => f.FilePath,
                            f => f.FileName
                            )
                )
                )
                .Take(Constants.Constants.OPENSEARCH_MAX_TAKE)
            );
        }
        else
        {
            _logger.LogInformation("calling OS without References");
            searchResponse = await _db.SearchAsync<BookMetaData>(s => s
                .Query(q => q
                .MatchPhrase(m => m.Field("bookText").Query(searchParameters.SingleSearchString))
                )
                .Source(sf => sf
                    .Includes(i => i
                        .Fields(
                            f => f.Title,
                            f => f.Pages,
                            f => f.Author,
                            f => f.FilePath,
                            f => f.FileName
                            )
                )
                )
                .Take(Constants.Constants.OPENSEARCH_MAX_TAKE)
            );
        }
        if (!searchResponse.IsValid)
        {
            _logger.LogWarning("Search was not valid {var}", searchResponse.DebugInformation);
        }
        else
        {
            _logger.LogInformation("Search contained a max of {var} results. Consider pagination if over {var2}", searchResponse.HitsMetadata.Total.Value, Constants.Constants.OPENSEARCH_MAX_TAKE);
        }

        _logger.LogDebug("After the async query to opensearch itself");

        foreach (var item in searchResponse.Hits)
        {
            string id = item.Id;
            var doc = item.Source;
            doc.OpenSearchId = id;
            booksMetadata.Add(doc);
        }
        //List<IEbookMetaData> returnMe = searchResponse.Documents.ToList<IEbookMetaData>();
        return ((int)searchResponse.HitsMetadata.Total.Value, booksMetadata);
    }
}
