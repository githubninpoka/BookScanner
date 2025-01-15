using OpenSearch.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Constants;

public class Constants
{
    public const int READ_BEFORE_CHARACTERS = 200;
    public const int READ_AHEAD_CHARACTERS = 1500;
    public const int READ_BEFORE_CHARACTERS_LARGE = 300;
    public const int READ_AHEAD_CHARACTERS_LARGE = 2500;

    public const int READ_BEFORE_WORDS_FOR_FUZZY = 20;
    public const int READ_AHEAD_WORDS_FOR_FUZZY = 150;
    public const int READ_BEFORE_WORDS_FOR_FUZZY_LARGE = 40;
    public const int READ_AHEAD_WORDS_FOR_FUZZY_LARGE = 300;

    public const int OPENSEARCH_MAX_TAKE = 150;
}
