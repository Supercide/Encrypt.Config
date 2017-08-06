using System;
using System.Linq;
using Encrypt.Config.RSA;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class CreateKeysState : ConsoleState {

        public override void Handle(Context context)
        {
            string containerName;
            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.CONTAINER_NAME, out containerName))
            {
                throw new InvalidOperationException();
            }

            string username;
            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.USERNAME, out username))
            {
                throw new InvalidOperationException();
            }

            var rsaWrapper = RSAContainerFactory.Create(containerName, username);
            try
            {
                if(context.Arguments.Any(kvp => kvp.Key == WellKnownCommandArguments.EXPORT_KEY))
                {
                    context.State = ExportState.CreateWithContainer(rsaWrapper);
                } else
                {
                    rsaWrapper.Dispose();
                    SetEndState(context);
                }

            } catch(Exception e)
            {
                rsaWrapper.Dispose();
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}