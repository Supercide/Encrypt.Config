using System;
using Encrypt.Config.Console.ConsoleStateMachine.States;

namespace Encrypt.Config.Console.ConsoleStateMachine.Factories {
    public class JsonConfigEncryptionStateFactory : IConsoleStateFactory
    {
        public bool CanParse(string command)
        {
            return command.ToLower()
                          .Contains(WellKnownCommands.ENCRYPT_JSON_CONFIG);
        }

        public ConsoleState Create()
        {
            return new JsonConfigEncryptionState();
        }
    }
}