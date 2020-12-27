using System;

namespace clockwork
{
    /// <summary>
    /// Builder for Time-Bound Tasks
    /// </summary>
    public class ClockworkTimedTaskBuilder
    {
        private Action Action;
        private int _repetitions = 1;
        private TimeSpan _repeatDelay;
        private ClockworkTaskTrigger _activationTrigger = null;
        private DateTime _startTime;
        private bool _isAbsolute = false;
        private TimeSpan _startDelay = TimeSpan.Zero;
        private bool _shouldContinueOnFail = true;
        private TimeZoneInfo _timeZone = TimeZoneInfo.Utc;

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
            _repetitions = repetitions;
            _repeatDelay = delay;
            return this;
        }
        /// <summary>
        /// Create a custom activation trigger
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder SetTrigger(ClockworkTaskTrigger trigger)
        {
            _activationTrigger = trigger;
            return this;
        }
        /// <summary>
        /// Delay task run after being triggered
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder Delayed(TimeSpan delay)
        {
            _isAbsolute = false;
            _startDelay = delay;
            return this;
        }
        /// <summary>
        /// Delay task run after being triggered
        /// </summary>
        /// <param name="absoluteDelay"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder Delayed(DateTime absoluteDelay)
        {
            _isAbsolute = true;
            _startTime = absoluteDelay;
            return this;
        }
        /// <summary>
        /// Sets the timezone for absolute delay
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder SetTimezone(TimeZoneInfo zone)
        {
            _timeZone = zone;
            return this;
        }
        /// <summary>
        /// Continue Running even after exceptions occur in task
        /// </summary>
        /// <param name="continueOnFail"></param>
        /// <returns></returns>
        public ClockworkTimedTaskBuilder ContinueOnFail(bool continueOnFail)
        {
            _shouldContinueOnFail = continueOnFail;
            return this;
        }

        /// <summary>
        /// Create the task from the options
        /// </summary>
        /// <returns></returns>
        public ClockworkTask Build()
        {
            if (_isAbsolute)
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    StartTime = TimeZoneInfo.ConvertTimeToUtc(_startTime, _timeZone),
                    ActivationTrigger = _activationTrigger,
                    ContinueOnFail = _shouldContinueOnFail,
                    IsAbsolute = true,
                    RepeatDelay = _repeatDelay,
                    Repetitions = _repetitions
                };
            }
            else
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    StartDelay = _startDelay,
                    ActivationTrigger = _activationTrigger,
                    ContinueOnFail = _shouldContinueOnFail,
                    IsAbsolute = false,
                    RepeatDelay = _repeatDelay,
                    Repetitions = _repetitions
                };
            }
        }
    }
}