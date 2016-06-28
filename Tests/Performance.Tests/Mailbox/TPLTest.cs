using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;
using System.Collections.Generic;

namespace Mailbox
{
    public class TPLTest
    {
        Task[] Tasks;
        static BufferBlock<Event> Inbox = new BufferBlock<Event>();

        public void Start(int numOfTasks, int numOfSendsPerTask)
        {
            var options = new DataflowBlockOptions();
            options.TaskScheduler = new DebugTaskScheduler();
            //options.BoundedCapacity = 10;
            TPLTest.Inbox = new BufferBlock<Event>(options);

            this.Tasks = new Task[numOfTasks + 1];

            for (int idx = 0; idx < numOfTasks; idx++)
            {
                var producer = new Task(async () =>
                {
                    for (int i = 0; i < numOfSendsPerTask; i++)
                    {
                        await Inbox.SendAsync(new Mailbox.MailEvent());
                    }
                });

                this.Tasks[idx] = producer;
            }

            var consumer = new Task(async () =>
            {
                while (await Inbox.OutputAvailableAsync())
                {
                    await Inbox.ReceiveAsync();
                }
            });

            this.Tasks[numOfTasks] = consumer;

            Profiler profiler = new Profiler();
            profiler.StartMeasuringExecutionTime();

            foreach (var task in this.Tasks)
            {
                task.Start();
            }

            Task.WaitAll(Tasks);

            profiler.StopMeasuringExecutionTime();
            Console.WriteLine("... TPL data-flow sent and handled " +
                $"{numOfTasks * numOfSendsPerTask} events in " +
                $"'{profiler.Results()}' seconds.");
            Console.WriteLine($"... task scheduled {DebugTaskScheduler.Count}");
        }
    }

    internal sealed class DebugTaskScheduler : TaskScheduler
    {
        #region fields

        internal static int Count = 0;

        #endregion

        #region override methods

        /// <summary>
        /// Queues the specified task to the scheduler.
        /// </summary>
        /// <param name="task">Task</param>
        protected override void QueueTask(Task task)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Count++;
                base.TryExecuteTask(task);
            }, null);
        }

        /// <summary>
        /// Attempts to dequeue a task that was previously
        /// queued to this scheduler.
        /// </summary>
        /// <param name="task">Task</param>
        /// <returns>Boolean</returns>
        protected override bool TryDequeue(Task task)
        {
            return base.TryDequeue(task);
        }

        /// <summary>
        /// Determines whether the provided task can be
        /// executed synchronously in this call, and if
        /// it can, executes it.
        /// </summary>
        /// <param name="task">Task</param>
        /// <param name="taskWasPreviouslyQueued">Boolean</param>
        /// <returns>Boolean</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;// base.TryExecuteTask(task);
        }

        /// <summary>
        /// For debugger support only, generates an enumerable
        /// of task instances currently queued to the scheduler
        /// waiting to be executed.
        /// </summary>
        /// <returns>Scheduled tasks</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            var tasks = new List<Task>();
            return tasks;
        }

        #endregion
    }
}
