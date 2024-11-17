using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary;

public class OpenSearchAccessOptions
{
    public string? OpenSearchUrl { get; set; }
    public string? OpenSearchIndex { get; set; }
    public string? OpenSearchUser { get; set; }
    public string? OpenSearchPass { get; set; }
}
