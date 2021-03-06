﻿using System;
using Microsoft.PSharp;
using Microsoft.PSharp.Utilities;

namespace MultiPaxos.PSharpLanguage
{
    /// <summary>
    /// A single-process implementation of the MultiPaxos consensus protocol written using
    /// the P# high-level syntax.
    /// 
    /// A brief description of the MultiPaxos protocol can be found here:
    /// http://amberonrails.com/paxosmulti-paxos-algorithm/
    ///  
    /// Note: this is an abstract implementation aimed primarily to showcase the testing
    /// capabilities of P#.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // Optional: increases verbosity level to see the P# runtime log.
            var configuration = Configuration.Create().WithVerbosityEnabled(2);

            // Creates a new P# runtime instance, and passes an optional configuration.
            var runtime = PSharpRuntime.Create(configuration);

            // Executes the P# program.
            Program.Execute(runtime);

            // The P# runtime executes asynchronously, so we wait
            // to not terminate the process.
            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();
        }
        
        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            runtime.RegisterMonitor(typeof(ValidityCheck));
            runtime.CreateMachine(typeof(GodMachine));
        }
    }
}
