using System;
using System.Collections.Generic;
using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;
using Microsoft.PSharp.TestingServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PingPong
{
    [TestClass]
    public class Test
    {
        static void Main(string[] args)
        {
            var runtime = PSharpRuntime.Create();
            Test.Execute(runtime);
            Console.ReadLine();
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            runtime.CreateMachine(typeof(Server), "TheUltimateServerMachine");
        }

        [TestMethod]
        public void TestReceivingExternalEvents()
        {
            var configuration = Configuration.Create();
            configuration.Verbose = 2;

            var engine = TestingEngineFactory.CreateBugFindingEngine(
                configuration, Execute);
            engine.Run();

            Assert.AreEqual(0, engine.NumOfFoundBugs);
        }
    }
}
