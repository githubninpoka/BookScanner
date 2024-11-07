namespace Ebooks.ClassLIbrary.Interfaces;

public interface IEbook
{
    string Author { get; }
    string BookText { get; }
    int Pages { get; }
    string Title { get; }

    string FilePath { get;  }

    void Populate();
}