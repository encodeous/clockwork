using System;

namespace clockwork.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]  
    public class FutureRepeat : Attribute
    {
        internal DateTime StartTime;
        internal TimeSpan RepeatDelay;
        internal bool ContinueOnFail;
        internal bool ParallelExecute;
        internal int Repetitions;
        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="delayStartMilliseconds">Number of ms to wait since the program is started is called to execute the function</param>
        /// <param name="repeatDelayMilliseconds">Number of ms to wait in-between executions</param>
        /// <param name="repetitions">Number of times to repeat, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        /// <param name="parallelExecution">Allow the task to run even when the previous execution is not over</param>
        public FutureRepeat(long delayStartMilliseconds, long repeatDelayMilliseconds, int repetitions = -1, bool continueOnFail = true, bool parallelExecution = true)
        {
            StartTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(delayStartMilliseconds);
            RepeatDelay = TimeSpan.FromMilliseconds(repeatDelayMilliseconds);
            ContinueOnFail = continueOnFail;
            ParallelExecute = parallelExecution;
            Repetitions = repetitions;
        }
    }
}