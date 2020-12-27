namespace clockwork.Attributes
{
    /// <summary>
    /// Clockwork Execution Schedule
    /// </summary>
    public enum Schedule
    {
        /// <summary>
        /// Schedule to the Start of each year
        /// </summary>
        ByYear,
        /// <summary>
        /// Schedule to the Start of each month
        /// </summary>
        ByMonth,
        /// <summary>
        /// Schedule to the Start of each week
        /// </summary>
        ByWeek,
        /// <summary>
        /// Schedule to the Start of each day
        /// </summary>
        ByDay,
        /// <summary>
        /// Schedule to the Start of each hour
        /// </summary>
        ByHour,
        /// <summary>
        /// Schedule to the Start of each minute
        /// </summary>
        ByMinute,
        /// <summary>
        /// Schedule to the Start of each second
        /// </summary>
        BySecond
    }
}