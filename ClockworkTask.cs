using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using clockwork.Future;

namespace clockwork
{
    public class ClockworkTask : IComparable
    {
        internal Action Action;
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
                    StartTime = Utils.GetNextTimeScheduleUtc(TaskSchedule, ScheduleTimeZone, StartTime) + CycleStartDelay;
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

        internal ClockworkActivateTaskTrigger ActivationTrigger;

        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="delayStartMilliseconds">Number of ms to wait since this task is activated to run the action</param>
        /// <param name="repeatDelayMilliseconds">Number of ms to wait in-between executions</param>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, long delayStartMilliseconds, long repeatDelayMilliseconds, int repetitions = -1, bool continueOnFail = true, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartDelay = TimeSpan.FromMilliseconds(delayStartMilliseconds);
            IsAbsolute = false;
            RepeatDelay = TimeSpan.FromMilliseconds(repeatDelayMilliseconds);
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="delayStart">Time to wait to run since activation</param>
        /// <param name="repeatDelay">Time to wait in-between executions</param>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, TimeSpan delayStart, TimeSpan repeatDelay, int repetitions = -1, bool continueOnFail = true, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartDelay = delayStart;
            IsAbsolute = false;
            RepeatDelay = repeatDelay;
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that repeats following the schedule
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="cycleDelayStart">Amount of time to delay before running the task, relative to the cycle</param>
        /// <param name="taskSchedule">Type of Scheduled Task</param>
        /// <param name="timeZone">Timezone of the Schedule</param>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, TimeSpan cycleDelayStart, Schedule taskSchedule, TimeZoneInfo timeZone, int repetitions = -1, bool continueOnFail = true, ClockworkActivateTaskTrigger trigger = null)
        {
            TaskSchedule = taskSchedule;
            ScheduleTimeZone = timeZone;
            IsScheduled = true;
            Action = action;
            StartTime = Utils.GetNextTimeScheduleUtc(taskSchedule, timeZone, DateTime.UtcNow);
            CycleStartDelay = cycleDelayStart;
            IsAbsolute = false;
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="startTime">Absolute time to start the task, if the task is not activated by then... it will be called immediately</param>
        /// <param name="repeatDelay">Time to wait in-between executions</param>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, DateTime startTime, TimeSpan repeatDelay, int repetitions = -1, bool continueOnFail = true, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartTime = startTime.ToUniversalTime();
            IsAbsolute = true;
            RepeatDelay = repeatDelay;
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that repeats
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="timeZoneInfo">Timezone to run the task</param>
        /// <param name="startTime">Absolute time to start the task, if the task is not activated by then... it will be called immediately</param>
        /// <param name="repeatDelay">Time to wait in-between executions</param>
        /// <param name="repetitions">Number of times to run, -1 for infinite</param>
        /// <param name="continueOnFail">Continue Repeating even if there was an exception</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, TimeZoneInfo timeZoneInfo, DateTime startTime, TimeSpan repeatDelay, int repetitions = -1, bool continueOnFail = true, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartTime = TimeZoneInfo.ConvertTimeToUtc(startTime.ToUniversalTime(), timeZoneInfo);
            IsAbsolute = true;
            RepeatDelay = repeatDelay;
            ContinueOnFail = continueOnFail;
            Repetitions = repetitions;
            ActivationTrigger = trigger;
        }
        
        
        
        /// <summary>
        /// Specifies an action that runs once
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="delayStartMilliseconds">Number of ms to wait since this task is activated to run the action</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, long delayStartMilliseconds, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartDelay = TimeSpan.FromMilliseconds(delayStartMilliseconds);
            IsAbsolute = false;
            RepeatDelay = TimeSpan.Zero;
            ContinueOnFail = false;
            Repetitions = 1;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that runs once
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="delayStart">Time to wait to run since activation</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, TimeSpan delayStart, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartDelay = delayStart;
            IsAbsolute = false;
            RepeatDelay = TimeSpan.Zero;
            ContinueOnFail = false;
            Repetitions = 1;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that runs once
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="startTime">Absolute time to start the task, if the task is not activated by then... it will be called immediately</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, DateTime startTime, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartTime = startTime.ToUniversalTime();
            IsAbsolute = true;
            RepeatDelay = TimeSpan.Zero;
            ContinueOnFail = false;
            Repetitions = 1;
            ActivationTrigger = trigger;
        }
        /// <summary>
        /// Specifies an action that runs once
        /// </summary>
        /// <param name="action">Action to run</param>
        /// <param name="timeZoneInfo">Timezone of the StartTime</param>
        /// <param name="startTime">Absolute time to start the task, if the task is not activated by then... it will be called immediately</param>
        /// <param name="trigger">Trigger to Activate the Task, by default, it is when the program starts</param>
        public ClockworkTask(Action action, TimeZoneInfo timeZoneInfo, DateTime startTime, ClockworkActivateTaskTrigger trigger = null)
        {
            Action = action;
            StartTime = TimeZoneInfo.ConvertTimeToUtc(startTime.ToUniversalTime(), timeZoneInfo);
            IsAbsolute = true;
            RepeatDelay = TimeSpan.Zero;
            ContinueOnFail = false;
            Repetitions = 1;
            ActivationTrigger = trigger;
        }

        public int CompareTo(object obj)
        {
            var x = (ClockworkTask) obj;
            return StartTime.CompareTo(x.StartTime);
        }
    }
}