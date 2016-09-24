using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PSharp;

namespace Chord
{
    internal class LivenessMonitor : Monitor
    {
        #region events
        
        public class NotifyClientRequest : Event
        {
            public int Key;

            public NotifyClientRequest(int key)
                : base()
            {
                this.Key = key;
            }
        }

        public class NotifyClientResponse : Event
        {
            public int Key;

            public NotifyClientResponse(int key)
                : base()
            {
                this.Key = key;
            }
        }

        #endregion

        #region states

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MonitorState { }

        void InitOnEntry()
        {
            this.Goto(typeof(Responded));
        }

        [Cold]
        [OnEventPushState(typeof(NotifyClientRequest), typeof(Requested))]
        [DeferEvents(typeof(NotifyClientResponse))]
        class Responded : MonitorState { }

        [Hot]
        [OnEventGotoState(typeof(NotifyClientResponse), typeof(Responded))]
        class Requested : MonitorState { }

        #endregion
    }
}