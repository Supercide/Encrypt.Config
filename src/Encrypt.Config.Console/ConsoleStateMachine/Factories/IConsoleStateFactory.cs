using Encrypt.Config.Console.ConsoleStateMachine.States;

namespace Encrypt.Config.Console.ConsoleStateMachine.Factories {
    public interface IConsoleStateFactory
    {
        bool CanParse(string command);

        ConsoleState Create();
    }
}