using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;
using Encrypt.Config.ConsoleHost.Constants;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories {
    public class CreateStateFactory : IConsoleStateFactory
    {
        public bool CanParse(string command)
        {
            return command.ToLower()
                          .Contains(WellKnownCommands.CREATE_KEYS);
        }

        public ConsoleState Create()
        {
            return new CreateKeysState();
        }
    }
}