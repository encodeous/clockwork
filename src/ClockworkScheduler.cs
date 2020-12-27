using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using C5;

namespace clockwork
{
    public class ClockworkScheduler
    {
        private IPriorityQueue<ClockworkTask> _taskQueue;
        private SemaphoreSlim _wakeSlim = new SemaphoreSlim(0,1);
        private SemaphoreSlim _accessSlim = new SemaphoreSlim(1,1);

        public ClockworkScheduler()
        {
            _taskQueue = new IntervalHeap<ClockworkTask>();
        }
        public bool Started { get; private set; } = false;

        /// <summary>
        /// Run the task scheduler on a separate thread
        /// </summary>
        public void Run()
        {
            if (!Started)
            {
                Started = true;
                Task.Run(Executor);
            }
        }

        private async Task Activator(ClockworkTask task)
        {
            if (Started)
            {
                try
                {
                    if (_wakeSlim.CurrentCount == 0) _wakeSlim.Release();
                    await _accessSlim.WaitAsync();
                    if (!task.IsAbsolute && !task.IsScheduled)
                    {
                        task.StartTime = DateTime.UtcNow + task.StartDelay;
                    }
                    
                    task.IsActivated = true;
                    _taskQueue.Add(task);
                }
                finally
                {
                    _accessSlim.Release();
                    if (_wakeSlim.CurrentCount == 0) _wakeSlim.Release();
                }
            }
        }

        private async Task Executor()
        {
            while (Started)
            {
                if (!_taskQueue.IsEmpty)
                {
                    try
                    {
                        await _accessSlim.WaitAsync();
                        var firstMin = _taskQueue.FindMin();
                        if (!firstMin.IsActivated)
                        {
                            _taskQueue.DeleteMin();
                            continue;
                        }

                        var k = DateTime.UtcNow;
                        if (k < firstMin.StartTime)
                        {
                            await _wakeSlim.WaitAsync(firstMin.StartTime - k);
                        }
                    }
                    finally
                    {
                        _accessSlim.Release();
                    }

                    try
                    {
                        await _accessSlim.WaitAsync();
                        var top = _taskQueue.FindMin();
                        if (top.StartTime <= DateTime.UtcNow)
                        {
                            _taskQueue.DeleteMin();
                            top.PreExecute();
                            Task.Run(top.Execute);
                            if (top.IsActivated)
                            {
                                _taskQueue.Add(top);
                            }
                        }
                    }
                    finally
                    {
                        _accessSlim.Release();
                    }

                }
                else
                {
                    await _wakeSlim.WaitAsync(TimeSpan.FromSeconds(1));
                }
            }
        }

        public async Task StopSchedulerAsync()
        {
            try
            {
                await _accessSlim.WaitAsync();
                while (!_taskQueue.IsEmpty)
                {
                    _taskQueue.DeleteMin();
                }

                Started = false;
                if (_wakeSlim.CurrentCount == 0) _wakeSlim.Release();
            }
            finally
            {
                _accessSlim.Release();
            }
        }
        /// <summary>
        /// Add Task to the Scheduler
        /// </summary>
        /// <param name="task"></param>
        public async Task AddTaskAsync(ClockworkTask task)
        {
            if (task.ActivationTrigger == null)
            {
                await Activator(task);
            }
            else
            {
                task.ActivationTrigger.TriggerCalledEvent += async () =>
                {
                    var nextRunStartBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                        DateTime.UtcNow);
                    var nextRunEndBoundBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                        nextRunStartBound);
                    if (task.CycleStartDelay > nextRunEndBoundBound - nextRunStartBound)
                    {
                        throw new ArgumentOutOfRangeException("task",
                            "The specified Clockwork Scheduled Start Delay Extends into the next cycle");
                    }
                    task.StartTime = nextRunStartBound + task.CycleStartDelay;
                    await Activator(task);
                };
            }
        }
        /// <summary>
        /// Add Task to the Scheduler
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(ClockworkTask task)
        {
            if (task.ActivationTrigger == null)
            {
                Activator(task).GetAwaiter().GetResult();
            }
            else
            {
                task.ActivationTrigger.TriggerCalledEvent += async () =>
                {
                    var nextRunStartBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                        DateTime.UtcNow);
                    var nextRunEndBoundBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                        nextRunStartBound);
                    if (task.CycleStartDelay > nextRunEndBoundBound - nextRunStartBound)
                    {
                        throw new ArgumentOutOfRangeException("task",
                            "The specified Clockwork Scheduled Start Delay Extends into the next cycle");
                    }
                    task.StartTime = nextRunStartBound + task.CycleStartDelay;
                    await Activator(task);
                };
            }
        }
        /// <summary>
        /// Add all tasks in the list to the Scheduler
        /// </summary>
        /// <param name="tasks"></param>
        public void AddAll(IEnumerable<ClockworkTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.ActivationTrigger == null)
                {
                    Activator(task).GetAwaiter().GetResult();
                }
                else
                {
                    task.ActivationTrigger.TriggerCalledEvent += async () =>
                    {
                        var nextRunStartBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                            DateTime.UtcNow);
                        var nextRunEndBoundBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                            nextRunStartBound);
                        if (task.CycleStartDelay > nextRunEndBoundBound - nextRunStartBound)
                        {
                            throw new ArgumentOutOfRangeException("tasks",
                                "A specified Clockwork Scheduled Start Delay Extends into the next cycle");
                        }
                        task.StartTime = nextRunStartBound + task.CycleStartDelay;
                        await Activator(task);
                    };
                }
            }
        }
        /// <summary>
        /// Add all tasks in the list to the Scheduler
        /// </summary>
        /// <param name="tasks"></param>
        public async Task AddAllAsync(IEnumerable<ClockworkTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.ActivationTrigger == null)
                {
                    await Activator(task);
                }
                else
                {
                    task.ActivationTrigger.TriggerCalledEvent += async () =>
                    {
                        var nextRunStartBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                            DateTime.UtcNow);
                        var nextRunEndBoundBound = Utils.GetNextTimeScheduleUtc(task.TaskSchedule, task.ScheduleTimeZone,
                            nextRunStartBound);
                        if (task.CycleStartDelay > nextRunEndBoundBound - nextRunStartBound)
                        {
                            throw new ArgumentOutOfRangeException("tasks",
                                "A specified Clockwork Scheduled Start Delay Extends into the next cycle");
                        }
                        task.StartTime = nextRunStartBound + task.CycleStartDelay;
                        await Activator(task);
                    };
                }
            }
        }
    }
}