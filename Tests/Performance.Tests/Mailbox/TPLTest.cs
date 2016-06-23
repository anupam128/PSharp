using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;

namespace Mailbox
{
    public class TPLTest
    {
        Task[] Tasks;
        static BufferBlock<Event> Inbox = new BufferBlock<Event>();

        public void Start(int numOfTasks, int numOfSendsPerTask)
        {
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
        }
    }
}
