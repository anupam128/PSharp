using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredEvents
{
    class Environment : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        [DeferEvents(typeof(Client.Request))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            CreateMachine(typeof(Server));
            CreateMachine(typeof(Client), new Client.Config(this.Id));
        }
        #endregion
    }
}
