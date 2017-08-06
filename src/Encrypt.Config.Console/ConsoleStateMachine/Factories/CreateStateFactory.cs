using System;
using Encrypt.Config.Console.ConsoleStateMachine.States;

namespace Encrypt.Config.Console.ConsoleStateMachine.Factories {
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