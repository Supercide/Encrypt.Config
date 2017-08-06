using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories {
    public class CreateContainerStateFactory : IConsoleStateFactory
    {
        public bool CanParse(string command)
        {
            return command.ToLower()
                          .Contains(WellKnownCommands.CREATE_CONTAINER);
        }

        public ConsoleState Create()
        {
            return new CreateContainerState();
        }
    }
}