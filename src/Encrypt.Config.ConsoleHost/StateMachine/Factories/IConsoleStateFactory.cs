using Encrypt.Config.ConsoleHost.StateMachine.States;

namespace Encrypt.Config.ConsoleHost.StateMachine.Factories {
    public interface IConsoleStateFactory
    {
        bool CanParse(string command);

        ConsoleState Create();
    }
}