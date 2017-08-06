namespace Encrypt.Config.Console.ConsoleStateMachine.States {
    public abstract class ConsoleState
    {
        public abstract void Handle(Context context);
    }
}