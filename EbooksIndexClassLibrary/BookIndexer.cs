using Ebooks.ClassLibrary;
using Ebooks.ClassLibrary.Helpers;
using Ebooks.ClassLIbrary.Interfaces;
using OpenSearch.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndexClassLibrary;

public class BookIndexer
{
    // Reads valid files
    // populates necessary objects
    // indexes those objects into OpenSearch

    private readonly string directory;
    private readonly OpenSearchClient openSearchClient;
    private readonly ILogger logger;
    private List<string> filePaths = new List<string>();

    public BookIndexer(string directory, OpenSearchClient openSearchClient, ILogger logger )
    {
        this.directory = directory;
        this.openSearchClient = openSearchClient;
        this.logger = logger;
    }

    public async Task IndexAsync()
    {
        logger.Information("Starting gathering filenames at {var} ", nameof(IndexAsync));
        filePaths = FileLister.GetFileNames(directory, logger);
        foreach (var filePath in filePaths)
        {
            // This entire loop can possibly be made into a Task in the thread pool
            IEbook book = BookFactory.Create(filePath, logger.ForContext<BookFactory>());
            book.Populate();
            try
            {
                var response = await openSearchClient.IndexDocumentAsync<IEbook>(book);
                if (!response.IsValid)
                {
                    logger.Warning("Something is up with {var} namely {var2}", filePath, response.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not index {var}", filePath);
            }
        }
    }


}
