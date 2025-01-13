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
        _logger.LogDebug("{var} - {var2} - first step in find ebooks async method", nameof(EBooksFinder), nameof(FindEBooksAsync));
        _logger.LogInformation("{var} - {var2} - Searching with Referencebooks set to {var3}", nameof(EBooksFinder), nameof(FindEBooksAsync), searchParameters.OnlyReference);
        _logger.LogInformation("{var} - {var2} - Searching with Tarotbooks set to {var3}", nameof(EBooksFinder), nameof(FindEBooksAsync), searchParameters.AlsoTarot);
        ISearchResponse<BookMetaData> searchResponse;
        if (searchParameters.OnlyReference && searchParameters.AlsoTarot)
        {

            _logger.LogInformation("{var} - {var2} - calling OS with limited reach", nameof(EBooksFinder), nameof(FindEBooksAsync));
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
            _logger.LogInformation("{var} - {var2} - calling OS with limited reach", nameof(EBooksFinder), nameof(FindEBooksAsync));
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

            _logger.LogInformation("{var} - {var2} - calling OS with limited reach", nameof(EBooksFinder), nameof(FindEBooksAsync));
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
            _logger.LogInformation("{var} - {var2} - calling OS including everything fuzzy", nameof(EBooksFinder), nameof(FindEBooksAsync));
        // https://www.elastic.co/guide/en/elasticsearch/reference/current/common-options.html#fuzziness
            searchResponse = await _db.SearchAsync<BookMetaData>(s => s
                .Query(q => q
                .Match(m => m.Field("bookText")
                .Query(searchParameters.SingleSearchString)
                //.Fuzziness(Fuzziness.EditDistance(2))
                .Fuzziness(Fuzziness.Auto) // will allow more differentiation when the search term is longer
                //.PrefixLength(0) // number of letters that need exact match at the beginning
                //.MaxExpansions(50) // number of variations that will be tried
                //.FuzzyTranspositions(true) // default: says that letters can be interchanged or not
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
            _logger.LogInformation("{var} - {var2} - calling OS including everything", nameof(EBooksFinder), nameof(FindEBooksAsync));
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
            _logger.LogWarning("{var2} - {var3} - Search was not valid {var}", nameof(EBooksFinder), nameof(FindEBooksAsync), searchResponse.DebugInformation);
            _logger.LogInformation("{var2} - {var3} - Did search terminate early: {var}", nameof(EBooksFinder), nameof(FindEBooksAsync), searchResponse.TerminatedEarly);
            _logger.LogInformation("{var2} - {var3} - Did search timeout: {var} ", nameof(EBooksFinder), nameof(FindEBooksAsync), searchResponse.TimedOut);
            _logger.LogInformation("{var2} - {var3} - Message? {var} ", nameof(EBooksFinder), nameof(FindEBooksAsync), searchResponse.OriginalException.Message);
            return (0, null);
        }
        else
        {
            _logger.LogInformation("{var3} - {var4} - Search contained a max of {var} results. Consider pagination if over {var2}", nameof(EBooksFinder), nameof(FindEBooksAsync), searchResponse.HitsMetadata.Total.Value, Constants.Constants.OPENSEARCH_MAX_TAKE);
        }

        _logger.LogDebug("{var} - {var2} - After the async query to opensearch itself",nameof(EBooksFinder), nameof(FindEBooksAsync));

        foreach (var item in searchResponse.Hits)
        {
            string id = item.Id;
            var doc = item.Source;

            doc.OpenSearchId = id;
            booksMetadata.Add(doc);
        }
        _logger.LogWarning("{var} - {var2} - Search was valid {var3}",nameof(EBooksFinder),nameof(FindEBooksAsync), searchResponse.DebugInformation);
        _logger.LogInformation("{var} - {var2} - OS search took {var3} milliseconds",nameof(EBooksFinder), nameof(FindEBooksAsync), searchResponse.Took);
        return ((int)searchResponse.HitsMetadata.Total.Value, booksMetadata);
    }
}
