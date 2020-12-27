using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using clockwork.Attributes;
namespace clockwork
{
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
        public Action Action;
        public bool IsAbsolute { get; internal set; }
        internal TimeSpan StartDelay;
        public DateTime StartTime { get; internal set; }
        public TimeSpan RepeatDelay { get; internal set; }
        public bool ContinueOnFail{ get; internal set; }
        public int Repetitions { get; internal set; }
        
        public bool IsScheduled { get; internal set; }
        public Schedule TaskSchedule { get; internal set; }
        public TimeZoneInfo ScheduleTimeZone { get; internal set; }
        public TimeSpan CycleStartDelay { get; internal set; }

        public int CurrentExecutions { get; private set; }

        public delegate void TaskExit(ClockworkTask task);
        public delegate void TaskException(ClockworkTask task, Exception e);

        /// <summary>
        /// Called when a Clockwork Task is finished running
        /// </summary>
        public event TaskExit OnExit;
        /// <summary>
        /// Called when a Clockwork Task failed
        /// </summary>
        public event TaskException OnException;

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