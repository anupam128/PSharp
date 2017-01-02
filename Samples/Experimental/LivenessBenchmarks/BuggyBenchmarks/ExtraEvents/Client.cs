using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class Client : Machine
    {
        #region fields
        class Local : Event { }
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventGotoState(typeof(Local), typeof(Handling))]
        class Init : MachineState { }

        [OnEntry(nameof(ProcessEvent))]
        [OnEventGotoState(typeof(Local), typeof(Handling))]
        class Handling : MachineState { }
        #endregion

        #region fields
        MachineId server;
        #endregion

        #region actions
        void InitOnEntry()
        {
            server = CreateMachine(typeof(Server));
            Raise(new Local());
        }
        void ProcessEvent()
        {
            if (this.Random())
            {
                Send(server, new Server.FirstEvent());
            }
            if (this.Random())
            {
                Send(server, new Server.SecondEvent());
            }
            if (this.Random())
            {
                Send(server, new Server.ThirdEvent());
            }
            Raise(new Local());
        }
        #endregion
    }
}
