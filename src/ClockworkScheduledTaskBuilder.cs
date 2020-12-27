using System;
using clockwork.Attributes;

namespace clockwork
{
    public class ClockworkScheduledTaskBuilder
    {
        private Action Action;
        private int Repetitions = 1;
        private ClockworkTaskTrigger ActivationTrigger = null;
        private TimeSpan CycleStartDelay = TimeSpan.Zero;
        private bool ShouldContinueOnFail = true;
        private Schedule Schedule;
        private TimeZoneInfo TimeZone = TimeZoneInfo.Utc;

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
            Repetitions = repetitions;
            return this;
        }
        /// <summary>
        /// Create a custom activation trigger
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder SetTrigger(ClockworkTaskTrigger trigger)
        {
            ActivationTrigger = trigger;
            return this;
        }
        /// <summary>
        /// Sets the timezone for Schedule
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder SetTimezone(TimeZoneInfo zone)
        {
            TimeZone = zone;
            return this;
        }
        /// <summary>
        /// Amount of time to offset the execution of the task by for every Scheduled cycle, must not be greater than the scheduled length
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder Delay(TimeSpan delay)
        {
            CycleStartDelay = delay;
            return this;
        }
        /// <summary>
        /// Continue Running even after exceptions occur in task
        /// </summary>
        /// <param name="continueOnFail"></param>
        /// <returns></returns>
        public ClockworkScheduledTaskBuilder ContinueOnFail(bool continueOnFail)
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
            if (ActivationTrigger != null)
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    IsScheduled = true,
                    ActivationTrigger = ActivationTrigger,
                    ContinueOnFail = ShouldContinueOnFail,
                    Repetitions = Repetitions,
                    CycleStartDelay = CycleStartDelay,
                    TaskSchedule = Schedule,
                    ScheduleTimeZone = TimeZone
                };
            }
            else
            {
                return new ClockworkTask()
                {
                    Action = Action,
                    IsScheduled = true,
                    StartTime = Utils.GetNextTimeScheduleUtc(Schedule, TimeZone, DateTime.UtcNow),
                    ActivationTrigger = ActivationTrigger,
                    ContinueOnFail = ShouldContinueOnFail,
                    Repetitions = Repetitions,
                    CycleStartDelay = CycleStartDelay,
                    TaskSchedule = Schedule,
                    ScheduleTimeZone = TimeZone
                };
            }
        }
    }
}