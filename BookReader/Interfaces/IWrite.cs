using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader.Interfaces
{
    public interface IWrite
    {
        bool Write(string filePath, string title, string author, int pages, int characterPosition, string snippet);

    }
}
