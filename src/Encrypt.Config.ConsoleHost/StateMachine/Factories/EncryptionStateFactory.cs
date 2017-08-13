using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.StateMachine.States;

namespace Encrypt.Config.ConsoleHost.StateMachine.Factories {
    public class EncryptionStateFactory : IConsoleStateFactory
    {
        public bool CanParse(string command)
        {
            return command.ToLower()
                          .Contains(WellKnownCommands.ENCRYPT_FILE);
        }

        public ConsoleState Create()
        {
            return new EncryptionState();
        }
    }
}