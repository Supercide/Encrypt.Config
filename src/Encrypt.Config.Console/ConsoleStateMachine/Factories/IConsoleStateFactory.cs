using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories {
    public interface IConsoleStateFactory
    {
        bool CanParse(string command);

        ConsoleState Create();
    }
}