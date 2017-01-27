﻿using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftTransient
{
    class LivenessMonitor : Monitor
    {
        public class NotifyMessage : Event { }
        public class NotifyDone : Event { }

        [Start]
        [OnEventGotoState(typeof(NotifyMessage), typeof(HotState))]
        class Init : MonitorState { }

        [Hot]
        [OnEventGotoState(typeof(NotifyMessage), typeof(HotState))]
        [OnEventGotoState(typeof(NotifyDone), typeof(ColdState))]
        class HotState : MonitorState { }

        [Cold]
        [OnEventGotoState(typeof(NotifyMessage), typeof(ColdState))]
        [OnEventGotoState(typeof(NotifyDone), typeof(ColdState))]
        class ColdState : MonitorState { }
    }
}
