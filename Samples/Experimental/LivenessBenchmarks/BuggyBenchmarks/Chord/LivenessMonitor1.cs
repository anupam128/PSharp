using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chord
{
    class LivenessMonitor1 : Monitor
    {
        #region events
        public class FoundSuccessor : Event
        {
            public int Key;
            public FoundSuccessor(int key)
            {
                this.Key = key;
            }
        }
        class Local : Event { }
        #endregion

        #region fields
        Dictionary<int, bool> AllSuccessorsFound;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        [OnEventGotoState(typeof(Local), typeof(WaitingSuccessors))]
        class Init : MonitorState { }

        [Hot]
        [OnEventDoAction(typeof(FoundSuccessor), nameof(CheckSuccessors))]
        [OnEventGotoState(typeof(Local), typeof(FoundAllSuccessors))]
        class WaitingSuccessors : MonitorState { }

        [Cold]
        [OnEventGotoState(typeof(FoundSuccessor), typeof(FoundAllSuccessors))]
        class FoundAllSuccessors : MonitorState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            AllSuccessorsFound = new Dictionary<int, bool>();
            AllSuccessorsFound.Add(1, false);
            AllSuccessorsFound.Add(2, false);
            AllSuccessorsFound.Add(6, false);
            Raise(new Local());
        }

        void CheckSuccessors()
        {
            var k = (ReceivedEvent as FoundSuccessor).Key;
            AllSuccessorsFound[k] = true;
            if (!AllSuccessorsFound.Values.Contains(false))
            {
                Raise(new Local());
            }
        }
        #endregion
    }
}
