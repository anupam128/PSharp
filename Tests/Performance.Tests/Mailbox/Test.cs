using System;

using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;

namespace Mailbox
{
    public class Test
    {
        static readonly int NumOfNodes = 5;
        static readonly int NumberOfSendsPerNode = 100000;

        static public Profiler Profiler = new Profiler();

        static void Main(string[] args)
        {
            var runtime = PSharpRuntime.Create();
            Execute(runtime);
            runtime.Wait();

            Profiler.StopMeasuringExecutionTime();
            Console.WriteLine($"... P# sent {NumOfNodes * NumberOfSendsPerNode} " +
                $"events for '{Profiler.Results()}' seconds.");
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            runtime.CreateMachine(typeof(Environment),
                new Environment.Config(NumOfNodes, NumberOfSendsPerNode));
        }
    }
}
