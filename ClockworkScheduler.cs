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
        public Task ExecutorTask { get; private set; }

        public void Run()
        {
            if (!Started)
            {
                Started = true;
                ExecutorTask = Task.Run(Executor);
            }
        }

        private void Activator(ClockworkTask task)
        {
            if (Started)
            {
                try
                {
                    _accessSlim.Wait();
                    if (!task.IsAbsolute)
                    {
                        task.StartTime = DateTime.UtcNow + task.StartDelay;
                    }

                    if (_wakeSlim.CurrentCount == 0) _wakeSlim.Release();
                    task.IsActivated = true;
                    _taskQueue.Add(task);
                }
                finally
                {
                    _accessSlim.Release();
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

                        if (DateTime.UtcNow < firstMin.StartTime)
                        {
                            await _wakeSlim.WaitAsync(firstMin.StartTime - DateTime.UtcNow);
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
        
        public void AddTask(ClockworkTask task)
        {
            if (task.ActivationTrigger == null)
            {
                Activator(task);
            }
            else
            {
                task.ActivationTrigger.TriggerCalledEvent += () => Activator(task);
            }
        }
        
        public void AddAll(IEnumerable<ClockworkTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.ActivationTrigger == null)
                {
                    Activator(task);
                }
                else
                {
                    task.ActivationTrigger.TriggerCalledEvent += () => Activator(task);
                }
            }
        }
    }
}