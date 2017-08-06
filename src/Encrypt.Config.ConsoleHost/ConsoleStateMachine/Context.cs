using System.Collections.Generic;
using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine {
    public class Context
    {
        public ConsoleState State { get; set; }

        public IReadOnlyDictionary<string, string> Arguments { get; private set; }

        public Context(IReadOnlyDictionary<string, string> arguments, ConsoleState state)
        {
            Arguments = arguments;
            State = state;
        }

        public void Request()
        {
            State.Handle(this);
        }
    }
}