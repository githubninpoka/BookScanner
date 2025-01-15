using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary.Annotations;

public class SingleWordAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string pattern = @"\W";
        Match match = Regex.Match(value.ToString(), pattern);
        if (match.Success)
        {
            return false;
        }
        return true;
    }
}
