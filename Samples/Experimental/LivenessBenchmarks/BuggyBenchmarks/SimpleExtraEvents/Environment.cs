using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExtraEvents
{
    class Environment : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            var server = CreateMachine(typeof(Server));
            CreateMachine(typeof(Client1), new Client1.Config(server));
            CreateMachine(typeof(Client2), new Client2.Config(server));
        }
        #endregion
    }
}
