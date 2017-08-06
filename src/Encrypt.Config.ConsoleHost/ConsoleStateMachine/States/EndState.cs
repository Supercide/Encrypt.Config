using System;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class EndState : ConsoleState {
        public override void Handle(Context context)
        {
            Console.WriteLine("Finished...");
        }
    }
}