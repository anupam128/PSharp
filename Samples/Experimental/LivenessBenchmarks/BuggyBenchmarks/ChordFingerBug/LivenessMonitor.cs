using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFingerBug
{
    class LivenessMonitor : Monitor
    {
        #region events
        public class NotifySuccessor : Event
        {
            public int ChordId;
            public int Succ;
            public NotifySuccessor(int chordId, int succ)
            {
                this.ChordId = chordId;
                this.Succ = succ;
            }
        }
        class GotoHot : Event { }
        class GotoCold : Event { }
        class Local : Event { }
        #endregion

        #region fields
        Dictionary<int, int> NodeSuccessors;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventGotoState(typeof(Local), typeof(NoRing))]
        class Init : MonitorState { }

        [Hot]
        [OnEventDoAction(typeof(NotifySuccessor), nameof(CheckRing))]
        [OnEventGotoState(typeof(GotoHot), typeof(NoRing))]
        [OnEventGotoState(typeof(GotoCold), typeof(Ring))]
        class NoRing : MonitorState { }

        [Cold]
        [OnEventDoAction(typeof(NotifySuccessor), nameof(CheckRing))]
        [OnEventGotoState(typeof(GotoHot), typeof(NoRing))]
        [OnEventGotoState(typeof(GotoCold), typeof(Ring))]
        class Ring : MonitorState { }
        #endregion

        #region actions
        void CheckRing()
        {
            var e = ReceivedEvent as NotifySuccessor;
            NodeSuccessors[e.ChordId] = e.Succ;
            Console.WriteLine("Monitor received: {0} ==> {1}", e.ChordId, e.Succ);
            foreach(var k in NodeSuccessors.Keys)
            {
                Console.WriteLine("Monitor Log:  {0} ==> {1}", k, NodeSuccessors[k]);
            }
            if (NodeSuccessors[0] != 1 || NodeSuccessors[1] != 2 || NodeSuccessors[2] != 0)
            {
                Raise(new GotoHot());
            }
            else
                Raise(new GotoCold());
        }

        void InitOnEntry()
        {
            NodeSuccessors = new Dictionary<int, int>();
            NodeSuccessors.Add(0, -1);
            NodeSuccessors.Add(1, -1);
            NodeSuccessors.Add(2, -1);

            Raise(new Local());
        }
        #endregion
    }
}
