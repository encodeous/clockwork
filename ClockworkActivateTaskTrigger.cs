namespace clockwork
{
    public class ClockworkActivateTaskTrigger
    {
        internal delegate void Triggered();

        internal event Triggered TriggerCalledEvent;
        
        public void Activate()
        {
            TriggerCalledEvent?.Invoke();
        }
    }
}