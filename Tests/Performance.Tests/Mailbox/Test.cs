using System;

using Microsoft.PSharp;
using Microsoft.PSharp.Threading.Scheduling;
using Microsoft.PSharp.Utilities;

namespace Mailbox
{
    public class Test
    {
        static readonly int NumOfNodes = 10;
        static readonly int NumberOfSendsPerNode = 10000;

        static public Profiler Profiler = new Profiler();

        static void Main(string[] args)
        {
            new TPLTest().Start(NumOfNodes, NumberOfSendsPerNode);

            var runtime = PSharpRuntime.Create(Configuration.Create().
                WithSchedulingStrategy(SchedulingStrategy.Fair));
            Execute(runtime);
            runtime.Wait();

            Profiler.StopMeasuringExecutionTime();
            Console.WriteLine($"... P# sent and handled {NumOfNodes * NumberOfSendsPerNode} " +
                $"events in '{Profiler.Results()}' seconds.");
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            runtime.CreateMachine(typeof(Environment),
                new Environment.Config(NumOfNodes, NumberOfSendsPerNode));
        }
    }
}
