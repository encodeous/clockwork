using System;

namespace clockwork
{
    public class ClockworkTimedTaskBuilder
    {
        private Action Action;
        private int Repetitions = 1;
        private TimeSpan RepeatDelay;
        private ClockworkTaskTrigger ActivationTrigger = null;
        private DateTime StartTime;
        private bool IsAbsolute = false;
        private TimeSpan StartDelay = TimeSpan.Zero;
        private bool ShouldContinueOnFail = true;
        private TimeZoneInfo TimeZone = TimeZoneInfo.Utc;

        public ClockworkTimedTaskBuilder(Action action)
        {
            Action = action;
        }
        /// <summary>
        /// Create a repeat task
        /// </summary>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <param name="delay">Amount of time inbetween executions</param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder Repeat(int repetitions, TimeSpan delay)
        {
            Repetitions = repetitions;
            RepeatDelay = delay;
            return this;
        }
        /// <summary>
        /// Create a custom activation trigger
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder SetTrigger(ClockworkTaskTrigger trigger)
        {
            ActivationTrigger = trigger;
            return this;
        }
        /// <summary>
        /// Delay task run after being triggered
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder Delayed(TimeSpan delay)
        {
            IsAbsolute = false;
            StartDelay = delay;
            return this;
        }
        /// <summary>
        /// Delay task run after being triggered
        /// </summary>
        /// <param name="absoluteDelay"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder Delayed(DateTime absoluteDelay)
        {
            IsAbsolute = true;
            StartTime = absoluteDelay;
            return this;
        }
        /// <summary>
        /// Sets the timezone for absolute delay
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder SetTimezone(TimeZoneInfo zone)
        {
            TimeZone = zone;
            return this;
        }
        /// <summary>
        /// Continue Running even after exceptions occur in task
        /// </summary>
        /// <param name="continueOnFail"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder ContinueOnFail(bool continueOnFail)
        {
            ShouldContinueOnFail = continueOnFail;
            return this;
        }

        /// <summary>
        /// Create the task from the options
        /// </summary>
        /// <returns></returns>
        public ClockworkTask Build()
        {
            if (IsAbsolute)
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    StartTime = TimeZoneInfo.ConvertTimeToUtc(StartTime, TimeZone),
                    ActivationTrigger = ActivationTrigger,
                    ContinueOnFail = ShouldContinueOnFail,
                    IsAbsolute = true,
                    RepeatDelay = RepeatDelay,
                    Repetitions = Repetitions
                };
            }
            else
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    StartDelay = StartDelay,
                    ActivationTrigger = ActivationTrigger,
                    ContinueOnFail = ShouldContinueOnFail,
                    IsAbsolute = false,
                    RepeatDelay = RepeatDelay,
                    Repetitions = Repetitions
                };
            }
        }
    }
}