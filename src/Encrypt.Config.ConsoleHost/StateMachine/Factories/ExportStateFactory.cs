using Encrypt.Config.ConsoleHost.Constants;
using Encrypt.Config.ConsoleHost.StateMachine.States;

namespace Encrypt.Config.ConsoleHost.StateMachine.Factories {
    public class ExportStateFactory : IConsoleStateFactory
    {
        public bool CanParse(string command)
        {
            return command.ToLower()
                          .Contains(WellKnownCommands.EXPORT_KEY);
        }

        public ConsoleState Create()
        {
            return new ExportState();
        }
    }
}