using System;
using System.IO;
using System.Security.Cryptography;
using Encrypt.Config.RSA;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class ExportState : ConsoleState {

        private RSACryptoServiceProvider _rsaCryptoServiceProvider;

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

                var exportPrivateKey = context.Arguments.ContainsKey(WellKnownCommandArguments.EXPORT_PRIVATE_KEY);

                if (!exportPrivateKey && !context.Arguments.ContainsKey(WellKnownCommandArguments.EXPORT_PUBLIC_KEY))
                {
                    throw new InvalidOperationException();
                }

                var rsaParameters = _rsaCryptoServiceProvider.ToXmlString(exportPrivateKey);

                File.WriteAllText(filePath, rsaParameters);

                SetEndState(context);

            } catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            } finally
            {
                _rsaCryptoServiceProvider.Dispose();
            }
        }

        public static ExportState CreateWithContainer(RSACryptoServiceProvider rsaCryptoServiceProvider)
        {
            var state = new ExportState
            {
                _rsaCryptoServiceProvider = rsaCryptoServiceProvider
            };

            return state;
        }
    }
}