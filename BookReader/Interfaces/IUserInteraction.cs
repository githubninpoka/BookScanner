using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookScanner.Interfaces;

public interface IUserInteraction
{
    string GetOptionalUserDirectory();
    string GetOptionalSearchPattern();
    string GetOptionalOutputDestination();
    void CommunicateToUser(string message);

}
