using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paxos
{
    class LivenessMonitor : Monitor
    {
        #region events
        public class NotifyPrepare : Event { }
        public class NotifyAccepted : Event { }
        #endregion

        #region states
        [Start]
        [OnEventGotoState(typeof(NotifyPrepare), typeof(ProposedState))]
        class Init : MonitorState { }

        [Hot]
        [IgnoreEvents(typeof(NotifyPrepare))]
        [OnEventGotoState(typeof(NotifyAccepted), typeof(AcceptedState))]
        class ProposedState : MonitorState { }

        [Cold]
        [IgnoreEvents(typeof(NotifyAccepted))]
        [OnEventGotoState(typeof(NotifyPrepare), typeof(ProposedState))]
        class AcceptedState : MonitorState { }
        #endregion
    }
}