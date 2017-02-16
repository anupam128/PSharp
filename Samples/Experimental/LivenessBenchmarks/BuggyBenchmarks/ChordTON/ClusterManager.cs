using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordTON
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
            var chord0 = CreateMachine(typeof(ChordNode));
            Send(chord0, new ChordNode.SetId(0));
            var chord1 = CreateMachine(typeof(ChordNode));
            Send(chord1, new ChordNode.SetId(1));
            var chord2 = CreateMachine(typeof(ChordNode));
            Send(chord2, new ChordNode.SetId(2));
            var nodeIds = new Dictionary<int, MachineId>();
            nodeIds.Add(0, chord0);
            nodeIds.Add(1, chord1);
            nodeIds.Add(2, chord2);
            Send(chord0, new ChordNode.Config(nodeIds));
            Send(chord1, new ChordNode.Config(nodeIds));
            Send(chord2, new ChordNode.Config(nodeIds));

            Send(chord0, new ChordNode.Create());
            Send(chord0, new ChordNode.FixFingers());
            Send(chord0, new ChordNode.FixFingers());
            Send(chord0, new ChordNode.FixFingers());
            Send(chord1, new ChordNode.Join(0));
            //Send(chord2, new ChordNode.Join(0));

            Send(chord0, new ChordNode.Stabilize());
            Send(chord1, new ChordNode.Stabilize());
            //Send(chord2, new ChordNode.Stabilize());

            //Send(chord0, new ChordNode.Stabilize());
            //Send(chord1, new ChordNode.Stabilize());
            //Send(chord2, new ChordNode.Stabilize());

            //Send(chord0, new ChordNode.FixFingers());
            //Send(chord1, new ChordNode.FixFingers());
            //Send(chord2, new ChordNode.FixFingers());
            //Send(chord0, new ChordNode.FixFingers());
            //Send(chord1, new ChordNode.FixFingers());
            //Send(chord2, new ChordNode.FixFingers());
            //Send(chord0, new ChordNode.FixFingers());
            //Send(chord1, new ChordNode.FixFingers());
            //Send(chord2, new ChordNode.FixFingers());

            //Send(chord0, new ChordNode.Stabilize());
            //Send(chord1, new ChordNode.Stabilize());
            //Send(chord2, new ChordNode.Stabilize());


            Send(chord0, new ChordNode.PrintInfo());
            Send(chord1, new ChordNode.PrintInfo());
        }
        #endregion
    }
}
