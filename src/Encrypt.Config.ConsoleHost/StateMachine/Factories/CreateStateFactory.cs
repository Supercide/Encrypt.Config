using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.StateMachine.States;

namespace Encrypt.Config.ConsoleHost.StateMachine.Factories {
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