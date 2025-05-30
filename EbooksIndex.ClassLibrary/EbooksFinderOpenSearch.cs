﻿using EbooksIndex.ClassLibrary.DataAccess;
using EbooksIndex.ClassLibrary.Interfaces;
using EbooksIndex.ClassLibrary.Models;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace EbooksIndex.ClassLibrary;

public class EbooksFinderOpenSearch : IEbooksFinder
{
    //responsible for finding books with a match
    // not responsible for loading contents
    // or loading matches

    private ILogger<EbooksFinderOpenSearch> _logger;
    private OpenSearchAccess _openSearchAccess;
    private OpenSearchClient _db;

    private List<IEbookMetaData> booksMetadata = new();

    public EbooksFinderOpenSearch(
        OpenSearchAccess openSearchAccess,
        ILogger<EbooksFinderOpenSearch> logger
        )
    {
        _logger = logger;
        _openSearchAccess = openSearchAccess;
        _db = _openSearchAccess.GetCLient();
        // OpenSearchClient has no IDisposable, so for now I'm not 'using'

    }
    public async Task<(int, List<IEbookMetaData>)> FindEBooksAsync(SearchParameters searchParameters, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{var} - {var2} - first step in find ebooks async method - {var3}", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync),searchParameters.SingleSearchString);

        int querySize = Constants.Constants.OPENSEARCH_MAX_TAKE;

        MatchQuery searchString = new MatchQuery()
        {
            Field = "bookText",
            Query = searchParameters.SingleSearchString
        };

        // https://www.elastic.co/guide/en/elasticsearch/reference/current/common-options.html#fuzziness
        if (searchParameters.FuzzySearch)
        {
            searchString.Fuzziness = Fuzziness.Auto;
        }

        List<QueryContainer> pathsToMatch = new(); // apparently the boolQuery needs an IEnumerable of type QueryContainer and MatchQuery is a QueryContainer.
        List<string> pathStrings = new();
        if (searchParameters.OnlyReference)
        {
            pathStrings.Add("GoToReferenceBooks");
        }
        if (searchParameters.AlsoTarot)
        {
            pathStrings.Add("Tarot_enDigitaleTarots");
        }
        foreach (var path in pathStrings)
        {
            pathsToMatch.Add(new MatchQuery() { Field = "filePath", Query = path });
            _logger.LogInformation("{var1} - {var2} - Searching in {var3}", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), path);
        }

        List<Field> includedSourceFields = new() {
            new Field("title"),
            new Field("pages"),
            new Field("author"),
            new Field("filePath"),
            new Field("fileName")
        };

        SourceFilter sourceFilter = new SourceFilter()
        {
            Includes = includedSourceFields.ToArray()
        };

        BoolQuery boolQuery = new BoolQuery()
        {
            Must = [searchString]
        };
        if (pathsToMatch.Count > 0)
        {
            boolQuery.MinimumShouldMatch = 1;
            boolQuery.Should = pathsToMatch.ToArray();
        }
        var query = new SearchRequest()
        {
            Query = boolQuery,
            Size = querySize,
            Source = sourceFilter
        };

        ISearchResponse<BookMetaData> searchResponse = await _db.SearchAsync<BookMetaData>(query, cancellationToken);

        if (!searchResponse.IsValid)
        {
            _logger.LogWarning("{var2} - {var3} - Search was not valid {var}", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.DebugInformation);
            _logger.LogInformation("{var2} - {var3} - Did search terminate early: {var}", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.TerminatedEarly);
            _logger.LogInformation("{var2} - {var3} - Did search timeout: {var} ", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.TimedOut);
            _logger.LogInformation("{var2} - {var3} - Message? {var} ", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.OriginalException.Message);
            return (0, null);
        }
        else
        {
            _logger.LogInformation("{var3} - {var4} - Search contained a max of {var} results. Consider pagination if over {var2}", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.HitsMetadata.Total.Value, Constants.Constants.OPENSEARCH_MAX_TAKE);
        }

        _logger.LogDebug("{var} - {var2} - After the async query to opensearch itself", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync));

        foreach (var item in searchResponse.Hits)
        {
            string id = item.Id;
            var doc = item.Source;

            doc.BookId = id;
            booksMetadata.Add(doc);
        }
        _logger.LogWarning("{var} - {var2} - Search was valid {var3}", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.DebugInformation);
        _logger.LogInformation("{var} - {var2} - OS search took {var3} milliseconds", nameof(EbooksFinderOpenSearch), nameof(FindEBooksAsync), searchResponse.Took);
        return ((int)searchResponse.HitsMetadata.Total.Value, booksMetadata);
    }
}
