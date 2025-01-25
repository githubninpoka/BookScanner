namespace EbooksIndex.ClassLibrary.Interfaces
{
    public interface IEbookRetriever
    {
        Task<IMatchedEbook?> Retrieve(string repositoryId);
    }
}