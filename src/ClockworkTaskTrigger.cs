namespace clockwork
{
    public class ClockworkTaskTrigger
    {
        internal delegate void Triggered();

        internal event Triggered TriggerCalledEvent;
        
        /// <summary>
        /// Call to Trigger the Task
        /// </summary>
        public void Activate()
        {
            TriggerCalledEvent?.Invoke();
        }
    }
}