using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairScheduleCycle
{
    class LivenessMonitor : Monitor
    {
        public class NotifyMessage : Event { }

        [Start]
        [OnEventGotoState(typeof(NotifyMessage), typeof(HotState))]
        class Init : MonitorState { }

        [Hot]
        [OnEventGotoState(typeof(NotifyMessage), typeof(ColdState))]
        class HotState : MonitorState { }

        [Cold]
        class ColdState : MonitorState { }
    }
}
