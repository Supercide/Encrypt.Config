using System;
using System.IO;
using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;
using Encrypt.Config.RSA;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories {
    public class CreateContainerState : ConsoleState {

        public override void Handle(Context context)
        {
            string containerName;
            if (!context.Arguments.TryGetValue(WellKnownCommandArguments.CONTAINER_NAME, out containerName))
            {
                throw new InvalidOperationException();
            }

            string username;
            if (!context.Arguments.TryGetValue(WellKnownCommandArguments.USERNAME, out username))
            {
                throw new InvalidOperationException();
            }

            string keyPath;
            if (!context.Arguments.TryGetValue(WellKnownCommandArguments.IMPORT_KEY, out keyPath))
            {
                throw new InvalidOperationException();
            }

            var key = File.ReadAllText(keyPath);

            using (var wrapper = RSAContainerFactory.CreateFromKey(containerName, key, username))
            {
                
            }

            SetEndState(context);
        }
    }
}