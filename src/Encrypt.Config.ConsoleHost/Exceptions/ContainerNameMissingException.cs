using System;
using System.Runtime.Serialization;

namespace Encrypt.Config.ConsoleHost.Exceptions {
    [Serializable]
    public class ContainerNameMissingException : Exception
    {
        public ContainerNameMissingException(string message) : base(message) { }
    }
}