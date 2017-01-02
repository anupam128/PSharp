using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class Environment : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init: MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            CreateMachine(typeof(Client));
        }
        #endregion
    }
}
