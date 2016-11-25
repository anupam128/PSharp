using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherLassoExample
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
            var machineA = CreateMachine(typeof(Server), new Server.Config(1));
            var machineB = CreateMachine(typeof(Server), new Server.Config(2));
            //var machineC = CreateMachine(typeof(Server), new Server.Config(3));
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyMessage());
            Send(machineA, new Server.SetNeighbour(machineB));
            Send(machineB, new Server.SetNeighbour(machineB));
            //Send(machineC, new Server.SetNeighbour(machineB));
            Send(machineA, new Server.Message(0));
        }
        #endregion
    }
}
