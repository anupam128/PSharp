using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFinger
{
    class ClusterManager : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            var n0 = CreateMachine(typeof(ChordNode));
            var n1 = CreateMachine(typeof(ChordNode));

            Dictionary<int, MachineId> NodeIds = new Dictionary<int, MachineId>();
            NodeIds.Add(0, n0);
            NodeIds.Add(1, n1);

            Send(n0, new ChordNode.Configure(0, NodeIds));
            Send(n1, new ChordNode.Configure(1, NodeIds));
            Send(n0, new ChordNode.Create());
            Send(n1, new ChordNode.Join(0));
        }
        #endregion
    }
}