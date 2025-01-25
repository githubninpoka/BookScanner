namespace EbooksIndex.ClassLibrary.Interfaces;

public interface IEbookMetaData
{
    string Author { get; }
    int Pages { get; }
    string Title { get; }
    string FilePath { get; }
    string FileName { get; }
    string MD5Hash { get; }
    string BookId { get; set; }
}
