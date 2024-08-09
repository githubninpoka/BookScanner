using BookReader.Interfaces;

namespace BookReader
{
    public class BookSnippetWriterTxtFile : IWrite
    {
        public IUserInteraction UserInteraction { get; set; }
        public BookSnippetWriterTxtFile(IUserInteraction userInteraction)
        {
            UserInteraction = userInteraction;
        }
        public bool Write(string filePath, string title, string author, int pages, int characterPosition, string snippet)
        {
            string toSaveText = "\n" + new string('=', 40) + $"\nIn the book {filePath} \n" +
            $"titled {title} \nby {author} \nwith {pages} pages \n" +
            $"we read at character index {characterPosition} \n====\n" +
            $" {snippet}";

            //TODO: Create a writer with an interface
            try
            {
                File.AppendAllText(Constants.Constants.DEFAULT_OUTPUT_TXT_FILE, toSaveText);
            }
            catch (IOException ex)
            {
                UserInteraction.CommunicateToUser($"Exception while writing to file: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}
