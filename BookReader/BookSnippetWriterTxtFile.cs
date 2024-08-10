using BookScanner.Interfaces;

namespace BookScanner;

public class BookSnippetWriterTxtFile : IWrite
{
    private string _outputDestination;
    private IUserInteraction _userInteraction;
    public BookSnippetWriterTxtFile(IUserInteraction userInteraction, string outputDestination)
    {
        _userInteraction = userInteraction;
        _outputDestination = outputDestination;
    }
    public bool Write(string filePath, string title, string author, int pages, int characterPosition, string snippet)
    {
        string toSaveText = "\n" + new string('=', 40) + $"\nIn the book {filePath} \n" +
        $"titled {title} \nby {author} \nwith {pages} pages \n" +
        $"we read at character index {characterPosition} \n====\n" +
        $" {snippet}";

        try
        {
            File.AppendAllText(_outputDestination, toSaveText);
        }
        catch (IOException ex)
        {
            _userInteraction.CommunicateToUser($"Exception while writing to file: {ex.Message}");
            return false;
        }
        return true;
    }
}
