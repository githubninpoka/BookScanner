using Ebooks.ClassLibrary;
using Ebooks.ClassLibrary.Helpers;
using Ebooks.ClassLIbrary.Interfaces;
using OpenSearch.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary;

public class BookIndexer
{
    // Reads valid files
    // populates necessary objects
    // indexes those objects into OpenSearch if their Md5 hash was not already present in the index

    private readonly string directory;
    private readonly OpenSearchClient openSearchClient;
    private readonly ILogger logger;
    private List<string> filePaths = new List<string>();

    public BookIndexer(string directory, OpenSearchClient openSearchClient, ILogger logger)
    {
        this.directory = directory;
        this.openSearchClient = openSearchClient;
        this.logger = logger;
    }

    public async Task IndexAsync()
    {
        logger.Information("Starting gathering filenames at {var} ", nameof(IndexAsync));
        Stopwatch sw = Stopwatch.StartNew();
        filePaths = FileLister.GetFileNames(directory, logger);
        int numberOfBooks = 0;
        int numberOfAlreadyIndexedEntries = 0;
        foreach (var filePath in filePaths)
        {
            numberOfBooks++;
            // This entire loop can possibly be made into a Task in the thread pool
            IEbook book = BookFactory.Create(filePath, logger.ForContext<BookFactory>());
            if (book == null)
            {
                logger.Warning("Could not instantiate a book for {var}", filePath);
                continue;
            }
            else
            {
                book.Populate();
            }
            try
            {
                var searchResponse = await openSearchClient.SearchAsync<IEbook>(s => s
                    .Source(false)
                    .Query(q => q
                    .Term(t => t
                    .Field("mD5Hash")
                    .Value(book!.MD5Hash))));
                if (searchResponse.Documents.Count != 0)
                {
                    //logger.Warning("I skip indexing of {var} because it is already in the index", book.FileName);
                    numberOfAlreadyIndexedEntries = numberOfAlreadyIndexedEntries + 1;
                }
                else
                {

                    var response = await openSearchClient.IndexDocumentAsync<IEbook>(book);
                    if (!response.IsValid)
                    {
                        logger.Warning("Something is up with {var} namely {var2}", filePath, response.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not index {var}", filePath);
            }
        }
        sw.Stop();
        int runtimeSeconds = (int)sw.ElapsedMilliseconds / 1000;
        logger.Information("Total processing time was {var} seconds", runtimeSeconds);
        logger.Information("{var} items were already indexed out of {var2} found files on disk.", numberOfAlreadyIndexedEntries,numberOfBooks);
    }


}
