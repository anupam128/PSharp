using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;

namespace PingPong
{
    public class Test
    {
        static void Main(string[] args)
        {
			Configuration config = Configuration.Create().
			    WithVerbosityEnabled(2).
			    WithDebuggingEnabled();
            var runtime = PSharpRuntime.Create(config);
            Task task = Test.Execute(runtime);
			task.Wait();
			//runtime.Wait();
			Console.ReadLine();
        }

        [Microsoft.PSharp.Test]
        public static async Task Execute(IPSharpRuntime runtime)
        {
			await runtime.CreateMachineAsync(typeof(Environment));
        }
    }
}
