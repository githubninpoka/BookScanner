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
    private OpenSearchClient _db;

    private List<IEbookMetaData> booksMetadata = new();

    public EBooksFinder(
        OpenSearchAccess openSearchAccess,
        ILogger<EBooksFinder> logger
        )
    {
        _logger = logger;
        _db = openSearchAccess.GetCLient();
    }
    public async Task<(int, List<IEbookMetaData>)> FindEBooksAsync(SearchParameters searchParameters, CancellationToken cancellationToken)
    {
        _logger.LogDebug("first step in find ebooks async method");
        _logger.LogInformation("Searching with Referencebooks set to {var}", searchParameters.OnlyReference);
        _logger.LogInformation("Searching with Tarotbooks set to {var}", searchParameters.AlsoTarot);
        ISearchResponse<BookMetaData> searchResponse;
        if (searchParameters.OnlyReference && searchParameters.AlsoTarot)
        {

            _logger.LogInformation("calling OS with limited reach");
            searchResponse = await _db.SearchAsync<BookMetaData>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            mu => mu.MatchPhrase(m => m.Field("filePath").Query("Tarot_enDigitaleTarots")),
                            mu => mu.MatchPhrase(m => m.Field("filePath").Query("GoToReferenceBooks"))
                            )
                        .MinimumShouldMatch(1)
                        .Must(
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
            ,cancellationToken);

        }
        else if (searchParameters.OnlyReference && !searchParameters.AlsoTarot)
        {
            _logger.LogInformation("calling OS with limited reach");
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
            ,cancellationToken);
        }
        else if (!searchParameters.OnlyReference && searchParameters.AlsoTarot)
        {

            _logger.LogInformation("calling OS with limited reach");
            searchResponse = await _db.SearchAsync<BookMetaData>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(

                            mu => mu.MatchPhrase(m => m.Field("filePath").Query("Tarot_enDigitaleTarots")),
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
            ,cancellationToken);
        }
        else if(searchParameters.FuzzySearch)
        {
            _logger.LogInformation("calling OS including everything fuzzy");
            searchResponse = await _db.SearchAsync<BookMetaData>(s => s
                .Query(q => q
                .Match(m => m.Field("bookText")
                .Query(searchParameters.SingleSearchString)
                .Fuzziness(Fuzziness.EditDistance(2))
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
            , cancellationToken);
        }
        else
        {
            _logger.LogInformation("calling OS including everything");
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
            , cancellationToken);
        }
        if (!searchResponse.IsValid)
        {
            _logger.LogWarning("Search was not valid {var}", searchResponse.DebugInformation);
            _logger.LogInformation("Did search terminate early: {var} ", searchResponse.TerminatedEarly);
            _logger.LogInformation("Did search timeout: {var} ", searchResponse.TimedOut);
            _logger.LogInformation("Message? {var} ", searchResponse.OriginalException.Message);
            return (0, null);
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
        _logger.LogWarning("Search was valid {var}", searchResponse.DebugInformation);
        _logger.LogInformation("OS search took {var} milliseconds", searchResponse.Took);
        return ((int)searchResponse.HitsMetadata.Total.Value, booksMetadata);
    }
}
