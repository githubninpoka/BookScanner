namespace BookScanner.Interfaces
{
    public interface IEbook
    {
        string Author { get;  }
        string BookText { get;  }
        int Pages { get;  }
        string Title { get; }
    }
}