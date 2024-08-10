using BookScanner.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Tokens;

namespace BookScanner.UserInteraction;

internal class UserInteractionConsole : IUserInteraction
{
    public void CommunicateToUser(string message)
    {
        Console.WriteLine(message);
    }

    public string GetOptionalSearchPattern()
    {
        int minimumNumberOfCharacters = 5;
        string searchString;
        bool IsCorrectPattern = false;
        string searchPattern = @"\w{" + minimumNumberOfCharacters + ",}";
        Regex regex = new Regex(searchPattern, RegexOptions.IgnoreCase);
        do
        {
            Console.WriteLine($"Please input a search term if you want to search for something other than {Constants.Constants.DEFAULT_MATCH_TERM}");
            Console.WriteLine($"Otherwise just press <enter>.");
            searchString = Console.ReadLine()!;
            if (searchString is null)
            {
                continue;
            }
            if (searchString == "")
            {
                IsCorrectPattern = true;
                return null!;
            }
            if (regex.IsMatch(searchString))
            {
                return searchString;
            }
            else
            {
                Console.WriteLine($"Your input string {searchString} does not meet the criteria. try again.");
                Console.WriteLine($"One phrase, more than {minimumNumberOfCharacters} characters.");
            }

        } while (!IsCorrectPattern);
        return searchString!;
    }

    public string GetOptionalUserDirectory()
    {
        string directoryPattern = $@"^[a-zA-Z]:\\(\w+)+\\*";
        Regex regex = new Regex(directoryPattern, RegexOptions.IgnoreCase);
        bool IsCorrectInput = false;
        string input;
        do
        {
            Console.WriteLine($"Please input a directory if you want to scan another directory than {Constants.Constants.DEFAULT_DIRECTORY_PATH}");
            Console.WriteLine($"Otherwise just press <enter>.");
            input = Console.ReadLine()!;
            if (input is null)
            {
                continue;
            }
            if (input == "")
            {
                IsCorrectInput = true;
                return null!;
            }
            if (regex.IsMatch(input))
            {
                if (Directory.Exists(input))
                {
                    Console.WriteLine($"I will find and process ebooks in folder {input}");
                    IsCorrectInput = true;
                }
                else
                {
                    Console.WriteLine($"Directory {input} does not seem to exist?");
                }
            }
        } while (!IsCorrectInput);
        return input!;
    }
    public string GetOptionalOutputDestination()
    {
        string outputDirectoryPattern = $@"^[a-zA-Z]:\\(\w+)+\\*";
        Regex regexDirectory = new Regex(outputDirectoryPattern, RegexOptions.IgnoreCase);
        string filePattern = $@"\w+\.txt$";
        Regex regexFile = new Regex(filePattern, RegexOptions.IgnoreCase);

        bool IsCorrectDirectoryInput = false;
        bool IsCorrectFileInput = false;
        string directoryInput;
        string fileInput = "";
        do
        {
            Console.WriteLine($"Please input a full directoryPath if you want to save the results to another file than {Constants.Constants.DEFAULT_OUTPUT_TXT_FILE}");
            Console.WriteLine($"Otherwise just press <enter>.");
            Console.WriteLine($"Chosen directory cannot be a root directory.");
            directoryInput = Console.ReadLine()!;
            if (directoryInput is null)
            {
                continue;
            }
            if (directoryInput == "")
            {
                IsCorrectDirectoryInput = true;
                return null!;
            }
            if (regexDirectory.IsMatch(directoryInput))
            {
                if (Directory.Exists(directoryInput))
                {
                    IsCorrectDirectoryInput = true;
                }
                else
                {
                    Console.WriteLine($"Directory {directoryInput} does not seem to exist?");
                    Console.WriteLine("Please make sure the directory does exist first.");
                }
            }
        } while (!IsCorrectDirectoryInput);

        if (directoryInput is not null && directoryInput != "")
        {
            do
            {
                Console.WriteLine($"Now please input a filename if you want to save the results to another file than {Constants.Constants.DEFAULT_OUTPUT_TXT_FILE}");
                Console.WriteLine($"Otherwise just press <enter> to fallback to the default output.");
                Console.WriteLine($"Format: chosenfilename.txt");
                fileInput = Console.ReadLine()!;
                if (fileInput is null)
                {
                    continue;
                }
                if (fileInput == "")
                {
                    IsCorrectFileInput = true;
                    return null!;
                }
                if (regexFile.IsMatch(fileInput))
                {
                    Console.WriteLine($"I will store the output at {directoryInput}\\{fileInput}");
                    IsCorrectFileInput = true;
                }
            } while (!IsCorrectFileInput);

        }
        string toReturnOutput = directoryInput + "\\" + fileInput;
        return toReturnOutput;
    }
}
