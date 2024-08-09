using BookReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig.Core;

namespace BookReader.UserInteraction
{
    internal class UserInteractionConsole : IUserInteraction
    {
        public void CommunicateToUser(string message)
        {
            Console.WriteLine(message);
        }

        public string GetOptionalSearchPattern()
        {
            string searchString;
            bool IsCorrectPattern = false ;
            string searchPattern = $@"\w+";
            Regex regex = new Regex(searchPattern, RegexOptions.IgnoreCase);
            do
            {
                Console.WriteLine($"Please input a search term if you want to search for something other than {Constants.Constants.DEFAULT_MATCH_TERM}");
                Console.WriteLine($"Otherwise just press <enter>.");
                searchString = Console.ReadLine();
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
                    Console.WriteLine("Single word, no spaces.");
                }

            } while (!IsCorrectPattern);
            return searchString!;
        }

        public string GetOptionalUserDirectory()
        {
            string pattern = $@"^[a-zA-Z]:\\(\w+)+\\*";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
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
    }
}
