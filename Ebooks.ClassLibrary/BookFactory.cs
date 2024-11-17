using Ebooks.ClassLIbrary.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebooks.ClassLibrary;

public class BookFactory
{
    public static IEbook Create(string filePath, ILogger? logger)
    {
        string fileExtension = Path.GetExtension(filePath.ToLower());
        switch (fileExtension)
        {
            case ".epub":
                try
                {
                    return new EpubBook(filePath, logger.ForContext<PdfBook>());
                    logger.Information("returning an instance of an epub book {var}", filePath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error while trying to instantiate object {var}", filePath);
                    return null;
                }
                break;
            case ".pdf":
                try
                {
                    return new PdfBook(filePath, logger.ForContext<PdfBook>());
                    logger.Information("returning an instance of a pdf book {var}", filePath);
                }
                catch(Exception ex)
                {
                    logger.Error(ex, "Error while trying to instantiate object {var}", filePath);
                    return null;
                }
                break;
            default:
                logger.Error("Error in {var} while trying to produce a book instance for {var2}", nameof(Create), filePath);
                return null;
                break;

        }
    }
}
