using System;

namespace Encrypt.Config.ConsoleHost.Exceptions
{
    [Serializable]
    public class UsernameMissingException : Exception
    {
        public UsernameMissingException(string message) : base(message) { }
    }
}
