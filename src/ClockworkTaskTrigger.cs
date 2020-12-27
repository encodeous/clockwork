namespace clockwork
{
    /// <summary>
    /// Custom Trigger to Activate a task, instead of the default trigger (program execution)
    /// </summary>
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