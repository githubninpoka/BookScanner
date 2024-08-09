namespace BookReader;

public static class Validator
{
    // seems duplicate but isn't. The business logic for the validations will differ per type.
    public static bool ValidateAuthor(string author)
    {
        if (author is null)
        {
            return false;
        }
        if (author == "")
        {
            return false;
        }
        if (author == "Test")
        {
            return false;
        }
        return true;
    }
    public static bool ValidateTitle(string title) 
    {
        if (title is null)
        {
            return false;
        }
        if (title == "")
        {
            return false;
        }
        if (title == "Test")
        {
            return false;
        }
        return true;
    }
    public static bool ValidateTitle(VersOne.Epub.Schema.EpubMetadataTitle titleObject) // this is probably an example of tightly coupled
    {
        if (titleObject.Id == "subtitle")
        {
            return false;
        }
        if (titleObject.Title == "")
        {
            return false;
        }
        if (titleObject.Title == "Test")
        {
            return false;
        }
        return true;
    }
}