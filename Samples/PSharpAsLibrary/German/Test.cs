﻿using System;
using System.Collections.Generic;
using Microsoft.PSharp;

namespace German
{
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
            runtime.CreateMachine(typeof(Host));
        }
    }
}
