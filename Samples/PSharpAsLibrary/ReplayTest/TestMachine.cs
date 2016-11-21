using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayTest
{
    public class TestMachine : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            Console.WriteLine("Action started");
            if (this.Random())
            {
                Console.WriteLine("Non-deterministic true choice");
                this.Assert(false);
            }
            else
            {
                Console.WriteLine("Non-deterministic false choice");
            }
            Console.WriteLine("Action finished");
        }
        #endregion
    }
}
