using System;

namespace Encrypt.Config.ConsoleHost.Exceptions {
    [Serializable]
    public class MissingKeyException : Exception
    {
        public MissingKeyException(string message) : base(message) { }
    }
}