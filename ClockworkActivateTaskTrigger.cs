namespace clockwork
{
    public class ClockworkTaskTrigger
    {
        internal delegate void Triggered();

        internal Triggered TriggerCalledDelegate;
        public void TriggerFunction()
        {
            TriggerCalledDelegate?.Invoke();
        }
    }
}