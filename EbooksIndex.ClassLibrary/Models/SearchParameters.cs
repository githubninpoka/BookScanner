using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Models;

public class SearchParameters
{
    public string? SingleSearchString { get; set; }

    public bool OnlyReference { get; set; } = false;

    public bool AlsoTarot { get; set; } = false;

    public bool FuzzySearch { get; set; }
}
