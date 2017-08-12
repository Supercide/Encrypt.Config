using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories {
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