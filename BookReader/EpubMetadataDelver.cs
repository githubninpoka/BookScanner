using BookScanner.Helpers;
using VersOne.Epub;

namespace BookScanner;

public static class EpubMetadataDelver
{
    public static IList<string> GetAuthors(EpubBookRef currentBook)
    {
        List<string> authors = new();
        if (currentBook.AuthorList is not null)
        {
            foreach (string author in currentBook.AuthorList)
            {
                if (Validator.ValidateAuthor(author))
                {
                    authors.Add(author);
                }
            }
        }
        else if (currentBook.Author is not null && Validator.ValidateAuthor(currentBook.Author))
        {
            authors.Add(currentBook.Author);
        }
        else if (currentBook.Schema.Epub2Ncx is not null &&
            currentBook.Schema.Epub2Ncx.DocAuthors is not null)
        {
            foreach (string author in currentBook.Schema.Epub2Ncx.DocAuthors)
            {
                if (Validator.ValidateAuthor(author))
                {
                    authors.Add(author);
                }
            }
        }
        else if (currentBook.Schema.Package.Metadata is not null &&
            currentBook.Schema.Package.Metadata.Creators is not null)
        {
            foreach (string author in currentBook.Schema.Package.Metadata.Creators.Select(x => x.Creator))
            {
                if (Validator.ValidateAuthor(author))
                {
                    authors.Add(author);
                }
            }
        }
        return authors;
    }

    public static IList<string> GetTitles(EpubBookRef currentBook)
    {
        List<string> titles = new List<string>();
        if (currentBook.Schema.Package.Metadata.Titles is not null)
        {
            foreach (var titleObject in currentBook.Schema.Package.Metadata.Titles)
            {
                if (Validator.ValidateTitle(titleObject))
                {
                    titles.Add(titleObject.Title);
                }
            }
            if (currentBook.Schema.Package.Metadata.Titles.Count() > 1)
            {
                foreach (var item in currentBook.Schema.Package.Metadata.Titles)
                {
                    Console.WriteLine(item.Id);
                    Console.WriteLine(item.Title);
                }
            }
        }
        else if (currentBook.Title is not null && Validator.ValidateTitle(currentBook.Title))
        {
            titles.Add(currentBook.Title);
        }
        else if (currentBook.Schema.Epub2Ncx is not null &&
            currentBook.Schema.Epub2Ncx.DocTitle is not null &&
            Validator.ValidateTitle(currentBook.Schema.Epub2Ncx.DocTitle)
            )
        {
            titles.Add(currentBook.Schema.Epub2Ncx.DocTitle);
        }
        return titles;
    }

    internal static int? GetNumberOfPages(EpubBookRef bookReference)
    {
        int numberOfPages =0;

        if (bookReference.Schema.Epub2Ncx is not null &&
            bookReference.Schema.Epub2Ncx.Head is not null &&
             bookReference.Schema.Epub2Ncx.Head.Items is not null)
        {
            if (bookReference.Schema.Epub2Ncx.Head.Items.Any(x => x.Name == "dtb:totalPageCount"))
            {
                numberOfPages = Convert.ToInt32(bookReference.Schema.Epub2Ncx.Head.Items
                    .Where(x => x.Name == "dtb.totalPageCount").Select(x => x.Content).FirstOrDefault());
            }
        }

        if (bookReference.Schema.Epub2Ncx is not null &&
            bookReference.Schema.Epub2Ncx.PageList is not null &&
            bookReference.Schema.Epub2Ncx.PageList.Items is not null)
        {
            numberOfPages = bookReference.Schema.Epub2Ncx.PageList.Items.Count();
        }

        return (numberOfPages > 0 ? numberOfPages : null);
    }
}
