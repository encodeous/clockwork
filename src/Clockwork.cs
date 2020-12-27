using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using clockwork.Attributes;

namespace clockwork
{
    /// <summary>
    /// Clockwork Initialization
    /// </summary>
    public class Clockwork
    {
        /// <summary>
        /// Default Clockwork Scheduler that is Run when Wind is called
        /// </summary>
        public static ClockworkScheduler Default = new ClockworkScheduler();
        
        /// <summary>
        /// Run the default scheduler on a separate thread and scan for Future Attributes
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

                        var fonce = method.GetCustomAttributes<Future>().ToList();
                        var frep = method.GetCustomAttributes<FutureScheduled>().ToList();

                        if (method.IsStatic && (fonce.Any() || frep.Any()))
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
                            fo.Task.Action = action;
                            tasks.Add(fo.Task);
                        }
                        
                        foreach (var fo in frep)
                        {
                            fo.Task.Action = action;
                            tasks.Add(fo.Task);
                        }
                    }
                }

            Default.AddAll(tasks);
        }
    }
}