using System;
using System.Runtime.Serialization;

namespace Encrypt.Config.ConsoleHost.Exceptions {
    [Serializable]
    public class MissingFilePathException : Exception
    {
        public MissingFilePathException(string message) : base(message) { }
    }
}