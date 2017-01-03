using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class Client1 : Machine
    {
        #region events
        public class Config : Event
        {
            public MachineId Target;
            public Config(MachineId Target)
            {
                this.Target = Target;
            }
        }
        class Local : Event { }
        #endregion

        #region fields
        MachineId server;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventGotoState(typeof(Local), typeof(Handling))]
        class Init : MachineState { }

        [OnEntry(nameof(Process))]
        [OnEventGotoState(typeof(Local), typeof(Handling))]
        class Handling : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            var e = ReceivedEvent as Config;
            server = e.Target;
            Raise(new Local());
        }

        void Process()
        {
            Send(server, new Server.Event1());
            Send(server, new Server.Event3());
            Raise(new Local());
        }
        #endregion
    }
}
