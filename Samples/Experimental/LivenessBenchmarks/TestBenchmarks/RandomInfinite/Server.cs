using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomInfinite
{
    class Server : Machine
    {
        #region events
        class Local : Event { }
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        class Init : MachineState { }

        [OnEntry(nameof(Process))]
        [OnEventDoAction(typeof(Local), nameof(Process))]
        class Waiting : MachineState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            Goto(typeof(Waiting));
        }

        void Process()
        {
            Send(Id, new Local());
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyMessage());
            if(this.Random())
            {
                this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyDone());
                this.Raise(new Halt());
            }
        }
        #endregion
    }
}
