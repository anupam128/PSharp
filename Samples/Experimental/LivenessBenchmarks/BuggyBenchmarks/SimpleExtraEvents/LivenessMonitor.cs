using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExtraEvents
{
    class LivenessMonitor : Monitor
    {
        #region events
        public class CtrExceeded : Event { }
        #endregion

        #region states
        [Start]
        [Cold]
        [OnEventGotoState(typeof(CtrExceeded), typeof(HotState))]
        class init : MonitorState { }

        [Hot]
        [OnEventGotoState(typeof(CtrExceeded), typeof(HotState))]
        class HotState : MonitorState { }
        #endregion
    }
}
