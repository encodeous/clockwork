using System;

namespace clockwork.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]  
    public class Future : Attribute
    {
        internal ClockworkTask Task;
        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="delayStartMilliseconds">Number of ms to wait since the program is started is called to execute the function</param>
        /// <param name="repeatDelayMilliseconds">Number of ms to wait in-between executions</param>
        /// <param name="repetitions">Number of times to repeat, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        public Future(long delayStartMilliseconds, long repeatDelayMilliseconds, int repetitions = -1, bool continueOnFail = true)
        {
            Task = new ClockworkTaskBuilder(null)
                .BuildTimedTask()
                .Delayed(DateTime.UtcNow + TimeSpan.FromMilliseconds(delayStartMilliseconds))
                .Repeat(repetitions, TimeSpan.FromMilliseconds(repeatDelayMilliseconds))
                .ContinueOnFail(continueOnFail)
                .Build();
        }
        
        /// <summary>
        /// Specifies an action that runs once
        /// </summary>
        /// <param name="delayStartMilliseconds">Number of ms to wait since the program is started is called to execute the function</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        public Future(long delayStartMilliseconds, bool continueOnFail = true)
        {
            Task = new ClockworkTaskBuilder(null)
                .BuildTimedTask()
                .Delayed(DateTime.UtcNow + TimeSpan.FromMilliseconds(delayStartMilliseconds))
                .ContinueOnFail(continueOnFail)
                .Build();
        }
    }
}