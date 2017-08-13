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
                if (!context.Arguments.TryGetValue(WellKnownCommandArguments.EXPORT_KEY, out var filePath))
                {
                    throw new InvalidOperationException();
                }

                var exportPrivateKey = context.Arguments.ContainsKey(WellKnownCommandArguments.KEY_TYPE_PRIVATE);

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