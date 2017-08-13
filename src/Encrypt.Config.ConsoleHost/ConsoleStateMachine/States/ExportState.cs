using System;
using System.IO;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.Encryption.Asymmetric;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class ExportState : ConsoleState {

        private RSAEncryption _rsaEncryption;

        public ExportState()
        {

        }

        public override void Handle(Context context)
        {
            try
            {
                string filePath;
                if (!context.Arguments.TryGetValue(WellKnownCommandArguments.EXPORT_KEY, out filePath))
                {
                    throw new InvalidOperationException();
                }

                var exportPrivateKey = context.Arguments.ContainsKey(WellKnownCommandArguments.KEY_TYPE_PRIVATE);

                if (!exportPrivateKey && !context.Arguments.ContainsKey(WellKnownCommandArguments.KEY_TYPE_PUBLIC))
                {
                    throw new InvalidOperationException();
                }

                var keyData = _rsaEncryption.ExportKey(exportPrivateKey);

                File.WriteAllText(filePath, keyData);

                SetEndState(context);

            } catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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