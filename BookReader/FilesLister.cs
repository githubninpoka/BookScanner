namespace BookReader;

public static class FilesLister
{
    public static List<string> GetFileNames(string directoryPath)
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
                foreach (var fileExtensionEnum in Enum.GetValues(typeof(Enumerations.FileExtensions)))
                {
                    if ("." + fileExtensionEnum.ToString() == fileExt)
                    {
                        //Console.WriteLine(fileExt + " is indeed included, namely " + fileExtensionEnum.ToString());
                        filePaths.Add(filePath);
                    }
                }
            }
        }
        return filePaths;
    }
}

