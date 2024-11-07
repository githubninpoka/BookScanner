using Ebooks.ClassLibrary.Enumerations;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebooks.ClassLibrary.Helpers;

public static class FileLister
{
    public static List<string> GetFileNames(string directoryPath, ILogger? logger = null)
    {
        List<string> filePaths = new();
        List<string> directoryPaths = new();
        directoryPaths.Add(directoryPath);

        foreach (string subdirectoryPath in Directory.EnumerateDirectories(directoryPath))
        {
            directoryPaths.Add(subdirectoryPath);
        }
        foreach (string subDirectory in directoryPaths)
        {
            foreach (string filePath in Directory.EnumerateFiles(subDirectory))
            {
                string fileExt = Path.GetExtension(filePath);
                fileExt = fileExt.ToLower();
                foreach (var fileExtensionEnum in Enum.GetValues(typeof(FileExtensions)))
                {
                    if ("." + fileExtensionEnum.ToString() == fileExt)
                    {
                        filePaths.Add(filePath);
                        if (logger != null)
                        {
                            logger.Debug("Adding file {var} to be indexed", filePath);
                        }
                    }
                }
            }
        }
        return filePaths;
    }
}
