using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRPZLabRab.Messages
{
    public sealed class LoginMessage
    {
        public LoginMessage(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
        }

        public bool IsLoggedIn { get; }
    }
}
