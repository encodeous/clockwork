using System;

namespace clockwork.Future
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]  
    public class FutureOnce : Attribute
    {
        internal DateTime StartTime;
        /// <summary>
        /// Executes a function once
        /// </summary>
        /// <param name="delayStartMilliseconds">Number of ms to wait since Clockwork.Wind() is called to execute the function</param>
        public FutureOnce(long delayStartMilliseconds)
        {
            StartTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(delayStartMilliseconds);
        }
    }
}