using EbooksIndex.ClassLibrary.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Models;

public class SearchParameters
{
    [Required(ErrorMessage = "First enter something to search for.")]
    [MinLength(4, ErrorMessage = "Use at least 4 characters.")]
    [SingleWord(ErrorMessage = "Search for a word without special characters.")]
    public string? SingleSearchString { get; set; }

    public bool OnlyReference { get; set; } = false;

    public bool AlsoTarot { get; set; } = false;

    public bool FuzzySearch { get; set; } = false;

    public bool LargeSnippets { get; set; } = false;
}
