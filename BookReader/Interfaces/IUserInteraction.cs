using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader.Interfaces
{
    public interface IUserInteraction
    {
        string GetOptionalUserDirectory();
        string GetOptionalSearchPattern();
        void CommunicateToUser(string message);

    }
}
