using EbooksIndex.ClassLibrary.Extensions;

namespace LevenshteinTesterConsole;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        string x = "sandalfon";
        string y = " Sandalphon.!";
        Console.WriteLine( x.IsComparableToOpenSearchLevenshtein(y));
    }
}
