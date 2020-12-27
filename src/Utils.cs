using System;
using clockwork.Attributes;

namespace clockwork
{
    class Utils
    {
        public static DateTime GetNextTimeScheduleUtc(Schedule schedule, TimeZoneInfo timezone, DateTime startUtc)
        {
            DateTime moment = TimeZoneInfo.ConvertTimeFromUtc(startUtc, timezone);
            int year = moment.Year;
            int month = moment.Month;
            int day = moment.Day;
            int hour = moment.Hour;
            int minute = moment.Minute;
            int second = moment.Second;
            var k = schedule switch
            {
                Schedule.ByDay => moment.Date.AddDays(1),
                Schedule.ByHour => new DateTime(year, month, day, hour, 0, 0).AddHours(1),
                Schedule.ByMonth => new DateTime(year, month, 1, 0, 0, 0).AddMonths(1),
                Schedule.ByWeek => new DateTime(year, month, day, 0, 0, 0).AddDays(7),
                Schedule.ByYear => new DateTime(year, 1, 0, 0, 0, 0).AddYears(1),
                Schedule.ByMinute => new DateTime(year, month, day, hour, minute, 0).AddMinutes(1),
                Schedule.BySecond => new DateTime(year, month, day, hour, minute, second).AddSeconds(1),
                _ => DateTime.MinValue
            };
            return TimeZoneInfo.ConvertTimeToUtc(k, timezone);
        }
    }
}