using System;

using Microsoft.PSharp;
using Microsoft.PSharp.Threading.Scheduling;
using Microsoft.PSharp.Utilities;

namespace Mailbox
{
    public class Test
    {
        internal static readonly int NumOfNodes = 10;
        internal static readonly int NumberOfSendsPerNode = 10000;

        static public Profiler Profiler = new Profiler();

        static void Main(string[] args)
        {
            //new TPLTest().Start(NumOfNodes, NumberOfSendsPerNode);

            //Profiler.StartMeasuringExecutionTime();

            var configurations = Configuration.Create();
            //configurations = configurations.WithSchedulingStrategy(SchedulingStrategy.Fair);
            //configurations = configurations.WithVerbosityEnabled(2);

            var runtime = PSharpRuntime.Create(configurations);

            //Profiler.StartMeasuringExecutionTime();

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
