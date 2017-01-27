using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftTransient
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
            CreateMachine(typeof(Server));
        }
        #endregion
    }
}
