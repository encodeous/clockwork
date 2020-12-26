using System;

namespace clockwork.Future
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]  
    public class FutureRepeatScheduled : Attribute
    {
        internal Schedule ScheduleType;
        internal TimeSpan CycleDelay;
        internal bool ContinueOnFail;
        internal int Repetitions;
        internal TimeZoneInfo Timezone;

        /// <summary>
        /// Specifies an action that repeats on a schedule. Note, the repeating schedule will begin on the start of the next cycle. i.e <see cref="Schedule.Weekly"/> will start on the NEXT week
        /// </summary>
        /// <param name="scheduleType">Type of Scheduled Task</param>
        /// <param name="cycleDelayMilliseconds">Number of ms to delay before running the task, relative to the cycle</param>
        /// <param name="timezoneId">Specify a Timezone, see https://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c</param>
        /// <param name="repetitions">Number of times to repeat, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        public FutureRepeatScheduled(Schedule scheduleType, string timezoneId = "UTC", long cycleDelayMilliseconds = 0, int repetitions = -1, bool continueOnFail = true)
        {
            ScheduleType = scheduleType;
            CycleDelay = TimeSpan.FromMilliseconds(cycleDelayMilliseconds);
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
            Timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }
    }
}