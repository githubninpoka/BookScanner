namespace Ebooks.ClassLIbrary.Interfaces;

public interface IEbook
{
    string Author { get; }
    string BookText { get; }
    int Pages { get; }
    string Title { get; }
    string FilePath { get;  }
    string FileName { get; }
    string MD5Hash { get; }

    void Populate();
}