using System;
using clockwork.Attributes;

namespace clockwork
{
    /// <summary>
    /// Builder for Scheduled Tasks
    /// </summary>
    public class ClockworkScheduledTaskBuilder
    {
        private Action Action;
        private int _repetitions = 1;
        private ClockworkTaskTrigger _activationTrigger = null;
        private TimeSpan _cycleStartDelay = TimeSpan.Zero;
        private bool _shouldContinueOnFail = true;
        private Schedule Schedule;
        private TimeZoneInfo _timeZone = TimeZoneInfo.Utc;

        public ClockworkScheduledTaskBuilder(Action action, Schedule schedule)
        {
            Action = action;
            Schedule = schedule;
        }
        /// <summary>
        /// Create a repeat task
        /// </summary>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder Repeat(int repetitions)
        {
            _repetitions = repetitions;
            return this;
        }
        /// <summary>
        /// Create a custom activation trigger
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder SetTrigger(ClockworkTaskTrigger trigger)
        {
            _activationTrigger = trigger;
            return this;
        }
        /// <summary>
        /// Sets the timezone for Schedule
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder SetTimezone(TimeZoneInfo zone)
        {
            _timeZone = zone;
            return this;
        }
        /// <summary>
        /// Amount of time to offset the execution of the task by for every Scheduled cycle, must not be greater than the scheduled length
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder Delay(TimeSpan delay)
        {
            _cycleStartDelay = delay;
            return this;
        }
        /// <summary>
        /// Continue Running even after exceptions occur in task
        /// </summary>
        /// <param name="continueOnFail"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder ContinueOnFail(bool continueOnFail)
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
            if (_activationTrigger != null)
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    IsScheduled = true,
                    ActivationTrigger = _activationTrigger,
                    ContinueOnFail = _shouldContinueOnFail,
                    Repetitions = _repetitions,
                    CycleStartDelay = _cycleStartDelay,
                    TaskSchedule = Schedule,
                    ScheduleTimeZone = _timeZone
                };
            }
            else
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    IsScheduled = true,
                    StartTime = Utils.GetNextTimeScheduleUtc(Schedule, _timeZone, DateTime.UtcNow),
                    ActivationTrigger = _activationTrigger,
                    ContinueOnFail = _shouldContinueOnFail,
                    Repetitions = _repetitions,
                    CycleStartDelay = _cycleStartDelay,
                    TaskSchedule = Schedule,
                    ScheduleTimeZone = _timeZone
                };
            }
        }
    }
}