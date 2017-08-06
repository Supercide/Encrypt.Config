namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public abstract class ConsoleState
    {
        public abstract void Handle(Context context);

        protected void SetEndState(Context context)
        {
            context.State = new EndState();
        }
    }
}