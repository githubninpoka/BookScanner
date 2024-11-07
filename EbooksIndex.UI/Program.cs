using Ebooks.ClassLibrary;
using Ebooks.ClassLIbrary.Interfaces;
using EbooksIndexClassLibrary;
using Microsoft.Extensions.Configuration;
using OpenSearch.Client;
using OpenSearch.Net;
using Serilog;

namespace EbooksIndex.UI;

internal class Program
{
    static OpenSearchClient? openSearchClient;
    static string? ebooksSourceDirectory;
    static Serilog.Core.Logger? logger;

    static async Task Main()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();

        //Serilog.Debugging.SelfLog.Enable(Console.Error);
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        Log.Information("Does this work?");
        logger = (Serilog.Core.Logger)Log.Logger;

        ConnectionSettings opensearchConnectionSettings;
        string openSearchDefaultIndex;
        try
        {
            ebooksSourceDirectory = configuration.GetSection("EbooksSourceDirectory").Value!.ToString();
            string openSearchUsername = configuration.GetSection("OpenSearchUser").Value!.ToString();
            string openSearchPassword = configuration.GetSection("OpenSearchPass").Value!.ToString();
            openSearchDefaultIndex = configuration.GetSection("OpenSearchIndex").Value!.ToString();
            var openSearchNodes = new Uri[]
            {
            new Uri(configuration.GetSection("OpenSearchUrl").Value!.ToString())
            };
            var openSearchConnectionPool = new StaticConnectionPool(openSearchNodes);
            opensearchConnectionSettings = new ConnectionSettings(openSearchConnectionPool);
            opensearchConnectionSettings.BasicAuthentication(openSearchUsername, openSearchPassword);
            opensearchConnectionSettings.DefaultIndex(openSearchDefaultIndex);
            opensearchConnectionSettings.ServerCertificateValidationCallback((o, c, ch, er) => true);
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "The application settings are not provided properly");
            throw;
        }

        logger.Information("Configuration complete.");
        openSearchClient = new OpenSearchClient(opensearchConnectionSettings);
        var existsResponse = await openSearchClient.Indices.ExistsAsync(openSearchDefaultIndex);
        if (!existsResponse.Exists)
        {
            var createResponse = await openSearchClient.Indices.CreateAsync(openSearchDefaultIndex, c => c.Map(m => m.AutoMap<IEbook>()));
            logger.Information("creating index response: {var}", createResponse.DebugInformation);
        }
        BookIndexer bookIndexer = new(ebooksSourceDirectory, openSearchClient, logger.ForContext<BookIndexer>());
        await bookIndexer.IndexAsync();
        
        logger.Information("Indexing completed.");

        Log.CloseAndFlush();
    }
}
