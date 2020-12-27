using System;

namespace clockwork.Attributes
{
    /// <summary>
    /// Attribute for a Scheduled Clockwork Task
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]  
    public class FutureScheduled : Attribute
    {
        internal ClockworkTask Task;

        /// <summary>
        /// Specifies an action that repeats on a schedule. Note, the repeating schedule will begin on the start of the next cycle. i.e <see cref="Schedule.ByWeek"/> will start on the NEXT week
        /// </summary>
        /// <param name="scheduleType">Type of Scheduled Task</param>
        /// <param name="delayMilliseconds">Number of ms to delay before running the task, relative to the cycle</param>
        /// <param name="timezoneId">Specify a Timezone, see https://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c</param>
        /// <param name="repetitions">Number of times to repeat, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        public FutureScheduled(Schedule scheduleType, int repetitions, string timezoneId = "UTC", long delayMilliseconds = 0, bool continueOnFail = true)
        {
            Task = new ClockworkTaskBuilder(null)
                .BuildScheduledTask(scheduleType)
                .Delay(TimeSpan.FromMilliseconds(delayMilliseconds))
                .Repeat(repetitions)
                .SetTimezone(TimeZoneInfo.FindSystemTimeZoneById(timezoneId))
                .ContinueOnFail(continueOnFail)
                .Build();
        }
        
        /// <summary>
        /// Specifies an action that runs once on the next Schedule time. Note, the task will begin on the start of the next Schedule period. i.e <see cref="Schedule.ByWeek"/> will start on the NEXT week
        /// </summary>
        /// <param name="scheduleType">Type of Scheduled Task</param>
        /// <param name="delayMilliseconds">Number of ms to delay before running the task, after the schedule bagan</param>
        /// <param name="timezoneId">Specify a Timezone, see https://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        public FutureScheduled(Schedule scheduleType, string timezoneId = "UTC", long delayMilliseconds = 0, bool continueOnFail = true)
        {
            Task = new ClockworkTaskBuilder(null)
                .BuildScheduledTask(scheduleType)
                .Delay(TimeSpan.FromMilliseconds(delayMilliseconds))
                .SetTimezone(TimeZoneInfo.FindSystemTimeZoneById(timezoneId))
                .ContinueOnFail(continueOnFail)
                .Build();
        }
    }
}