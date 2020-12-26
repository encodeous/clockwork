using System;

namespace clockwork.Future
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]  
    public class FutureRepeat : Attribute
    {
        internal DateTime StartTime;
        internal TimeSpan RepeatDelay;
        internal bool ContinueOnFail;
        internal int Repetitions;
        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="delayStartMilliseconds">Number of ms to wait since the program is started is called to execute the function</param>
        /// <param name="repeatDelayMilliseconds">Number of ms to wait in-between executions</param>
        /// <param name="repetitions">Number of times to repeat, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        public FutureRepeat(long delayStartMilliseconds, long repeatDelayMilliseconds, int repetitions = -1, bool continueOnFail = true)
        {
            StartTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(delayStartMilliseconds);
            RepeatDelay = TimeSpan.FromMilliseconds(repeatDelayMilliseconds);
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
        }
    }
}