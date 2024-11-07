using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub.Schema;

namespace Ebooks.ClassLibrary.Helpers;

public static class MetadataValidator
{
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
    public static bool ValidateTitle(EpubMetadataTitle titleObject) // this is probably an example of tightly coupled
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
