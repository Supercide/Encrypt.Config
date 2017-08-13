using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;
using Encrypt.Config.ConsoleHost.Constants;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories {
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