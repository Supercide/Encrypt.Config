using System;
using System.IO;
using Encrypt.Config.RSA;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class ExportState : ConsoleState {

        private RSAWrapper _rsaWrapper;

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

                var rsaParameters = _rsaWrapper.Export(exportPrivateKey);

                File.WriteAllText(filePath, rsaParameters);

                SetEndState(context);

            } catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            } finally
            {
                _rsaWrapper.Dispose();
            }
        }

        public static ExportState CreateWithContainer(RSAWrapper wrapper)
        {
            var state = new ExportState
            {
                _rsaWrapper = wrapper
            };

            return state;
        }
    }
}