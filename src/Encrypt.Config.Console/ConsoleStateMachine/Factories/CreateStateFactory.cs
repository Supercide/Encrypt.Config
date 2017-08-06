using System;
using Encrypt.Config.Console.ConsoleStateMachine.States;

namespace Encrypt.Config.Console.ConsoleStateMachine.Factories {
    public class CreateStateFactory : IConsoleStateFactory
    {
        public bool CanParse(string command)
        {
            throw new NotImplementedException();
        }

        public ConsoleState Create()
        {
            return new CreateState();
        }
    }
}