using EbooksIndex.ClassLibrary.Models;

namespace EbooksIndex.ClassLibrary.Interfaces
{
    public interface IEbooksFinder

    {
        Task<(int, List<IEbookMetaData>)> FindEBooksAsync(SearchParameters searchParameters, CancellationToken cancellationToken);
    }
}