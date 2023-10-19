using Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class AccountService : IAccountService
    {
        public bool Login(string username, string password)
        {
            if(username == null || password == null) return false;

            return username == "admin" && password == "123456";
        }

        public bool Register(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
