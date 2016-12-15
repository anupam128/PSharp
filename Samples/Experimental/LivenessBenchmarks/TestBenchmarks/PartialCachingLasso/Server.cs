using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialCachingLasso
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

        #region fields
        int counter;
        #endregion

        #region actions
        void OnInitEntry()
        {
            Goto(typeof(Waiting));
        }

        void Process()
        {
            counter++;
            Send(Id, new Local());
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyMessage());
        }
        #endregion
    }
}
