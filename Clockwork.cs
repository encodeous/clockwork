using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using clockwork.Future;

namespace clockwork
{
    public class Clockwork
    {
        public static ClockworkScheduler Default = new ClockworkScheduler();
        
        /// <summary>
        /// Run the default scheduler and scan for Future Attributes
        /// </summary>
        public static void Wind()
        {
            Default.Run();

            List<ClockworkTask> tasks = new List<ClockworkTask>();

                foreach (var type in Assembly.GetCallingAssembly().GetTypes())
                {
                    foreach (var method in type.GetMethods())
                    {
                        Action action;

                        var fonce = method.GetCustomAttributes<FutureOnce>().ToList();
                        var frep = method.GetCustomAttributes<FutureRepeat>().ToList();
                        var fsch = method.GetCustomAttributes<FutureRepeatScheduled>().ToList();
                        
                        if (method.IsStatic && (fonce.Any() || frep.Any() || fsch.Any()))
                        {
                            if (method.ReturnType != typeof(void))
                            {
                                if (method.ReturnType == typeof(Task))
                                {
                                    var func = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), method);
                                    action = async () => await func();
                                }
                                else
                                {
                                    var func = (Func<object>) Delegate.CreateDelegate(typeof(Func<object>), method);
                                    action = () => func();
                                }
                            }
                            else
                            {
                                action = (Action) Delegate.CreateDelegate(typeof(Action), method);
                            }
                        }
                        else
                        {
                            continue;
                        }

                        foreach (var fo in fonce)
                        {
                            var task = new ClockworkTask(action, fo.StartTime);
                            tasks.Add(task);
                        }
                        
                        foreach (var fo in frep)
                        {
                            var task = new ClockworkTask(action, fo.StartTime, fo.RepeatDelay, fo.Repetitions,
                                fo.ContinueOnFail);
                            tasks.Add(task);
                        }
                        
                        
                        foreach (var fo in fsch)
                        {
                            var task = new ClockworkTask(action, fo.CycleDelay, fo.ScheduleType, fo.Timezone,
                                fo.Repetitions, fo.ContinueOnFail);
                            tasks.Add(task);
                        }
                    }
                }

            Default.AddAll(tasks);
        }
    }
}