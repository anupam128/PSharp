﻿using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFingerBug
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtime = PSharpRuntime.Create();
            Program.Execute(runtime);
            Console.ReadLine();
        }

        [Microsoft.PSharp.Test]
        public static void Execute(PSharpRuntime runtime)
        {
            runtime.RegisterMonitor(typeof(LivenessMonitor));
            runtime.CreateMachine(typeof(ClusterManager));
        }
    }
}
