using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using OpenSearch.Net;
using Serilog;

namespace EbooksIndex.ClassLibrary.DataAccess;

public class OpenSearchAccess
{
    // to be added to the services container, with interface

    private readonly ILogger<OpenSearchAccess> _logger;
    private readonly string? _openSearchUser;
    private readonly string? _openSearchPassword;
    private readonly string? _openSearchIndex;
    private readonly string? _openSearchUrl;

    private OpenSearchClient? _openSearchClient;

    public OpenSearchAccess(IOptions<OpenSearchAccessOptions> openSearchAccessOptions,
        ILogger<OpenSearchAccess> logger)
    {
        _logger = logger;
        _openSearchUrl = openSearchAccessOptions.Value.OpenSearchUrl;
        _openSearchIndex = openSearchAccessOptions.Value.OpenSearchIndex;
        _openSearchUser = openSearchAccessOptions.Value.OpenSearchUser;
        _openSearchPassword = openSearchAccessOptions.Value.OpenSearchPass;
        _logger.LogInformation("Available URL: {var}", _openSearchUrl);
        _logger.LogInformation("index: {var}", _openSearchIndex);
    }

    public OpenSearchClient GetCLient()
    {

        if (!string.IsNullOrEmpty(_openSearchUrl))
        {
            ConnectionSettings opensearchConnectionSettings;
            var openSearchNodes = new Uri[]
                {
            new Uri(_openSearchUrl)
                };
            var openSearchConnectionPool = new StaticConnectionPool(openSearchNodes);
            opensearchConnectionSettings = new ConnectionSettings(openSearchConnectionPool);
            opensearchConnectionSettings.BasicAuthentication(_openSearchUser, _openSearchPassword);
            opensearchConnectionSettings.DefaultIndex(_openSearchIndex);
            opensearchConnectionSettings.ServerCertificateValidationCallback((o, c, ch, er) => true);
            opensearchConnectionSettings.RequestTimeout(TimeSpan.FromSeconds(10));
            //opensearchConnectionSettings.EnableDebugMode();
            //opensearchConnectionSettings.OnRequestCompleted(callDetails =>
            //{
            //    _logger.LogInformation("Request completed: {Method} {Uri}", callDetails.HttpMethod, callDetails.Uri);
            //    if (callDetails.ResponseBodyInBytes != null)
            //    {
            //        var responseBody = System.Text.Encoding.UTF8.GetString(callDetails.ResponseBodyInBytes);
            //        _logger.LogInformation("Response: {Response}", responseBody);
            //    }
            //})
            ;
                 var client = new OpenSearchClient(opensearchConnectionSettings);
            return client;
        }
        return null;
    }

}
