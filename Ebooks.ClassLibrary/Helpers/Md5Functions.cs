using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ebooks.ClassLibrary.Helpers;

public static class Md5Functions
{
    public static string ReturnMd5(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
