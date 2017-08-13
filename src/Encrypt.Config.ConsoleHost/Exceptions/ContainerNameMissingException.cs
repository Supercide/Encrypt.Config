using System;
using System.Runtime.Serialization;

namespace Encrypt.Config.ConsoleHost.Exceptions {
    [Serializable]
    public class ContainerNameMissingException : Exception
    {
        public ContainerNameMissingException() { }
        public ContainerNameMissingException(string message) : base(message) { }
        public ContainerNameMissingException(string message, Exception inner) : base(message, inner) { }

        protected ContainerNameMissingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}