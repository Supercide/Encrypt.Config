using System.Linq;
using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.Exceptions;
using Encrypt.Config.Encryption.Asymmetric;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class CreateKeysState : ConsoleState {

        public override void Handle(Context context)
        {
            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.CONTAINER_NAME, out var containerName))
            {
                throw new ContainerNameMissingException("Missing container name argument when creating key. try create --help for more information");
            }

            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.USERNAME, out var username))
            {
                throw new UsernameMissingException("Missing username argument when creating key. try create --help for more information");
            }

            var rsaEncryption = new RSAEncryption(containerName, username);

            if(context.Arguments.Any(kvp => kvp.Key == WellKnownCommandArguments.EXPORT_KEY))
            {
                context.State = ExportState.CreateWithContainer(rsaEncryption);
            } else
            {
                SetEndState(context);
            }
        }
    }
}