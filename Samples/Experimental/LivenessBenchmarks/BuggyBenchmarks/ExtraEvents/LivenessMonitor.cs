using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class LivenessMonitor : Monitor
    {
        public class GotFirstEvent : Event { }
        public class GotSecondEvent : Event { }

        [Start]
        [OnEventGotoState(typeof(GotFirstEvent), typeof(State1))]
        [OnEventGotoState(typeof(GotSecondEvent), typeof(State2))]
        class Init : MonitorState { }

        [Hot]
        [OnEventGotoState(typeof(GotFirstEvent), typeof(State1))]
        [OnEventGotoState(typeof(GotSecondEvent), typeof(ColdState))]
        class State1 : MonitorState { }

        [Cold]
        [OnEventGotoState(typeof(GotFirstEvent), typeof(ColdState))]
        [OnEventGotoState(typeof(GotSecondEvent), typeof(ColdState))]
        class ColdState : MonitorState { }

        [Hot]
        [OnEventGotoState(typeof(GotFirstEvent), typeof(State2))]
        [OnEventGotoState(typeof(GotSecondEvent), typeof(State2))]
        class State2 : MonitorState { }
    }
}
