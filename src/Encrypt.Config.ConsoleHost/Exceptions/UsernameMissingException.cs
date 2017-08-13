using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Encrypt.Config.ConsoleHost.Exceptions
{
    [Serializable]
    public class UsernameMissingException : Exception
    {
        public UsernameMissingException(string message) : base(message) { }
    }
}
