using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using clockwork.Attributes;
namespace clockwork
{
    /// <summary>
    /// A Clockwork Task
    /// </summary>
    public class ClockworkTask : IComparable
    {
        /// <summary>
        /// Action to run
        /// </summary>
        /// <param name="action"></param>
        public static ClockworkTaskBuilder Create(Action action)
        {
            return new ClockworkTaskBuilder(action);
        }
        /// <summary>
        /// Action to run
        /// </summary>
        /// <param name="action"></param>
        public static ClockworkTaskBuilder Create(Func<Task> action)
        {
            return new ClockworkTaskBuilder(action);
        }
        /// <summary>
        /// Please use <see cref="ClockworkTask.Create"/> to build tasks!
        /// </summary>
        public ClockworkTask()
        {
            
        }
        /// <summary>
        /// Add the task to the default scheduler
        /// </summary>
        public void Add()
        {
            Clockwork.Default.AddTask(this);
        }
        /// <summary>
        /// Action to be performed
        /// </summary>
        public Action Action;
        /// <summary>
        /// Checks if the task start time is Absolute (only applicable to non-scheduled tasks)
        /// </summary>
        public bool IsAbsolute { get; internal set; }
        /// <summary>
        /// The amount of time to delay before starting the task
        /// </summary>
        internal TimeSpan StartDelay;
        /// <summary>
        /// The absolute time to wait until to start the task
        /// </summary>
        public DateTime StartTime { get; internal set; }
        /// <summary>
        /// The amount of time to wait inbetween executions
        /// </summary>
        public TimeSpan RepeatDelay { get; internal set; }
        /// <summary>
        /// Whether to continue executing the task even when an exception occurred
        /// </summary>
        public bool ContinueOnFail{ get; internal set; }
        /// <summary>
        /// The amount of times to execute the Task, -1 for infinite
        /// </summary>
        public int Repetitions { get; internal set; }
        /// <summary>
        /// Checks if the task is Scheduled
        /// </summary>
        public bool IsScheduled { get; internal set; }
        /// <summary>
        /// The Schedule of the task
        /// </summary>
        public Schedule TaskSchedule { get; internal set; }
        /// <summary>
        /// The Timezone that the Schedule runs on
        /// </summary>
        public TimeZoneInfo ScheduleTimeZone { get; internal set; }
        /// <summary>
        /// The amount of time to offset the wait before running a scheduled task
        /// </summary>
        public TimeSpan CycleStartDelay { get; internal set; }
        /// <summary>
        /// The number of times the task has already executed
        /// </summary>
        public int CurrentExecutions { get; private set; }
        /// <summary>
        /// Delegate for when the task is exited
        /// </summary>
        /// <param name="task">The Task</param>
        public delegate void TaskExit(ClockworkTask task);
        /// <summary>
        /// Delegate for when the task crashed
        /// </summary>
        /// <param name="task">The Task</param>
        /// <param name="e">The exception</param>
        public delegate void TaskException(ClockworkTask task, Exception e);

        /// <summary>
        /// Called when a Clockwork Task is finished running
        /// </summary>
        public event TaskExit OnExit;
        /// <summary>
        /// Called when a Clockwork Task failed
        /// </summary>
        public event TaskException OnException;
        /// <summary>
        /// Checks if the task is active
        /// </summary>
        public bool IsActivated { get; internal set; }

        /// <summary>
        /// Stops future executions of the task
        /// </summary>
        public void DeactivateTask()
        {
            IsActivated = false;
        }

        internal void PreExecute()
        {
            CurrentExecutions++;
            if (CurrentExecutions >= Repetitions && Repetitions != -1)
            {
                IsActivated = false;
            }
            else
            {
                if (IsScheduled)
                {
                    var nextRunStartBound = Utils.GetNextTimeScheduleUtc(TaskSchedule, ScheduleTimeZone, StartTime);
                    var nextRunEndBoundBound = Utils.GetNextTimeScheduleUtc(TaskSchedule, ScheduleTimeZone,
                        nextRunStartBound);
                    if (CycleStartDelay > nextRunEndBoundBound - nextRunStartBound)
                    {
                        throw new ArgumentOutOfRangeException("CycleStartDelay",
                            "The specified Clockwork Scheduled Start Delay Extends into the next cycle");
                    }
                    StartTime = nextRunStartBound + CycleStartDelay;
                }
                else
                {
                    StartTime += RepeatDelay;
                }
            }
        }
        internal void Execute()
        {
            try
            {
                Action.Invoke();
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                Action.Invoke();
                if (!ContinueOnFail)
                {
                    DeactivateTask();
                }
            }
            
            if (!IsActivated)
            {
                OnExit?.Invoke(this);
            }
        }

        internal ClockworkTaskTrigger ActivationTrigger;
        public int CompareTo(object obj)
        {
            var x = (ClockworkTask) obj;
            return StartTime.CompareTo(x.StartTime);
        }
    }
}