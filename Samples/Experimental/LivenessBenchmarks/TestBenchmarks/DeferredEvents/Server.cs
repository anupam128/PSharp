using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredEvents
{
    class Server : Machine
    {
        #region events
        class Local : Event { }
        #endregion

        #region fields
        int Counter;
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
            Counter = 0;
            Goto(typeof(Waiting));
        }

        void Process()
        {
            Counter++;
            Send(Id, new Local());
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyMessage());
            if(Counter == 10000)
            {
                this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyDone());
                this.Raise(new Halt());
            }
        }
        #endregion
    }
}
