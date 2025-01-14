using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Models;

public class SearchParameters
{
    [MinLength(4, ErrorMessage = "Zoek met 1 woord, minimaal 4 karakters lang")]
    public string? SingleSearchString { get; set; }

    public bool OnlyReference { get; set; } = false;

    public bool AlsoTarot { get; set; } = false;

    public bool FuzzySearch { get; set; }
}
