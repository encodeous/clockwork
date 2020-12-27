# Clockwork - Rapid Task Scheduling

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/encodeous/clockwork/publish%20to%20nuget)](https://github.com/encodeous/clockwork) [![Nuget](https://img.shields.io/nuget/v/Encodeous.clockwork) ](https://www.nuget.org/packages/Encodeous.clockwork/)[![](https://img.shields.io/badge/View-Documentation-green)](https://encodeous.github.io/clockwork/index.html)

Clockwork is an easy to use Task Scheduler that reduces a lot of boilerplate code.

## Usage

Clockwork can either be used with in-code attributes or with the Scheduler Api.

### Clockwork with Attributes

Clockwork with Attributes requires the task function to be static.

There are two types of Clockwork attributes. 

- `Future` executes tasks bound by time or on repeat

- `FutureScheduled` will schedule executions to the beginning of `Years, Months, Weeks, Days, Hours, Minutes, or Seconds` with an optional offset.

Here is an example using Clockwork.

```c#
static void Main()
{
    // Start the Clockwork Scheduler
    Clockwork.Wind();
    Thread.Sleep(-1);
}
[Future(5000)]
public static void StartIn5Seconds()
{
    // This function will be run by the scheduler 5 seconds after the program starts
    Console.WriteLine("Started 5 Seconds Later");
}
[Future(2000, 5000)]
public static void RunEvery5Seconds()
{
    // This function will be run by the scheduler 2 seconds after the program starts, and will repeat every 5 seconds
    Console.WriteLine("Running every 5 Seconds");
}
[Future(2000, 5000)]
public static async Task RunOnStartOfMinute()
{
    // Clockwork also supports asynchronous functions, and will run all tasks in parallel
    // This will not block other tasks, nor will it block the future executions of the current task
    await Task.Delay(10000);
}
[FutureScheduled(Schedule.ByMonth, -1, "Eastern Standard Time")]
public static void RunOnStartOfMonth()
{
    // This function will be run by the scheduler on the start of every month in Eastern Standard Time
    Console.WriteLine("Running on the start of every month");
}
```

### Clockwork with the Advanced Scheduling Api

With the Clockwork Scheduling Api, it is possible to create tasks that have custom triggers.

Create an instance of `ClockworkTaskTrigger` and call the `Activate()` function to trigger the task. Please note that tasks will not trigger unless it is added to a task scheduler.

To create tasks with the Api, use the builder pattern shown below:

```c#
// Start the Clockwork Scheduler
Clockwork.Wind();
// Create a repeating task that runs every 5 seconds, with a delayed start of 2 seconds
var task = ClockworkTask.Create(() =>
    {
        Console.WriteLine("Repeat every 5 seconds");
    })
    .BuildTimedTask()
    .Repeat(-1, TimeSpan.FromSeconds(5))
    .Delayed(TimeSpan.FromSeconds(2))
    .Build();

// Custom Task Trigger
var trigger = new ClockworkTaskTrigger();
// Delay the activation of the event until the task is triggered
var customTrigger = ClockworkTask.Create(() =>
    {
        // This task will run on the middle of every second once the task is triggered and repeat indefinitely
        Console.WriteLine($"Triggered Task at {DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss.ffff tt")}");
    })
    .BuildScheduledTask(Schedule.BySecond)
    .Repeat(-1)
    .Delay(TimeSpan.FromMilliseconds(500))
    .SetTrigger(trigger)
    .Build();
// Add the tasks to the scheduler
Clockwork.Default.AddAll(new []{task, customTrigger});

Console.WriteLine("Waiting for a newline to trigger");
Console.ReadLine();
// Trigger the execution of the task
trigger.Activate();

Thread.Sleep(-1);
```