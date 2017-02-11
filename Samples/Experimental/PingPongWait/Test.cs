using System;
using System.Collections.Generic;
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
            Test.Execute(runtime);
			runtime.Wait();
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            runtime.CreateMachine(typeof(Server));
        }
    }
}
