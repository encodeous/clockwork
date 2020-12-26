using System;
using clockwork.Future;

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
            return schedule switch
            {
                Schedule.Daily => moment.Date.AddDays(1).ToUniversalTime(),
                Schedule.Hourly => new DateTime(year, month, day, hour, 0, 0).AddHours(1).ToUniversalTime(),
                Schedule.Monthly => new DateTime(year, month, 0, 0, 0, 0).AddMonths(1).ToUniversalTime(),
                Schedule.Weekly => new DateTime(year, month, day, 0, 0, 0).AddDays(7).ToUniversalTime(),
                Schedule.Yearly => new DateTime(year, 0, 0, 0, 0, 0).AddYears(1).ToUniversalTime(),
                Schedule.EveryMinute => new DateTime(year, month, day, hour, minute, 0).AddMinutes(1).ToUniversalTime(),
                Schedule.EverySecond => new DateTime(year, month, day, hour, minute, second).AddSeconds(1)
                    .ToUniversalTime(),
                _ => DateTime.MinValue
            };
        }
    }
}