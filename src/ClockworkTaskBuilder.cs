using System;
using System.Threading.Tasks;
using clockwork.Attributes;

namespace clockwork
{
    /// <summary>
    /// Builds a Clockwork Task
    /// </summary>
    public class ClockworkTaskBuilder
    {
        private Action _action;
        /// <summary>
        /// Action to run
        /// </summary>
        /// <param name="action"></param>
        public ClockworkTaskBuilder(Action action)
        {
            _action = action;
        }
        /// <summary>
        /// Action to run
        /// </summary>
        /// <param name="action"></param>
        public ClockworkTaskBuilder(Func<Task> action)
        {
            _action = async () => await action();
        }
        /// <summary>
        /// Create a time-bound task
        /// </summary>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder BuildTimedTask()
        {
            return new ClockworkTimedTaskBuilder(_action);
        }
        /// <summary>
        /// Create a Schedule-bound task
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder BuildScheduledTask(Schedule schedule)
        {
            return new ClockworkScheduledTaskBuilder(_action, schedule);
        }
    }
}