using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LassoExample
{
    class Environment : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            var machine1 = CreateMachine(typeof(Server), new Server.Config(1));
            var machine2 = CreateMachine(typeof(Server), new Server.Config(2));
            var machine3 = CreateMachine(typeof(Server), new Server.Config(3));
            var machine4 = CreateMachine(typeof(Server), new Server.Config(4));
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyMessage());
            Send(machine1, new Server.SetNeighbour(machine2));
            Send(machine2, new Server.SetNeighbour(machine3));
            Send(machine3, new Server.SetNeighbour(machine4));
            Send(machine4, new Server.SetNeighbour(machine1));
            //Send(machine1, new Server.Message(0));
        }
        #endregion
    }
}
