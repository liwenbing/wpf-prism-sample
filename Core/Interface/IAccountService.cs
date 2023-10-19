using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IAccountService
    {
        bool Login(string username, string password);

        bool Register(string username, string password);
    }
}
