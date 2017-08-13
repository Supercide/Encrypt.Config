using System;
using System.IO;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.Exceptions;
using Encrypt.Config.Encryption.Asymmetric;

namespace Encrypt.Config.ConsoleHost.StateMachine.States {
    public class ExportState : ConsoleState {

        private RSAEncryption _rsaEncryption;

        public ExportState()
        {

        }

        public override void Handle(Context context)
        {
            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.EXPORT_KEY_PATH, out var filePath))
            {
                throw new MissingFilePathException("Missing decryption key path. Try export --help for more information");
            }

            _rsaEncryption = _rsaEncryption ?? CreateContainer(context);

            var exportPrivateKey = context.Arguments.ContainsKey(WellKnownCommandArguments.KEY_TYPE_PRIVATE);

            var keyData = _rsaEncryption.ExportKey(exportPrivateKey);

            File.WriteAllText(filePath, keyData);

            SetEndState(context);
        }

        private RSAEncryption CreateContainer(Context context)
        {
            if (!context.Arguments.TryGetValue(WellKnownCommandArguments.CONTAINER_NAME, out var containerName))
            {
                throw new ContainerNameMissingException("Missing container name. Try export --help for more information");
            }

            return RSAEncryption.FromExistingContainer(containerName);
        }

        public static ExportState CreateWithContainer(RSAEncryption rsaEncryption)
        {
            var state = new ExportState
            {
                _rsaEncryption = rsaEncryption
            };

            return state;
        }
    }
}