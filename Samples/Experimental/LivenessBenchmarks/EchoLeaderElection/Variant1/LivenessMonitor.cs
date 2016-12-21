using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Variant1
{
    class LivenessMonitor : Monitor
    {
        #region fields
        public class LeaderElected : Event
        {
            public int LeaderId;
            public LeaderElected(int leaderId)
            {
                this.LeaderId = leaderId;
            }
        }
        class Local : Event { }
        class GotoCold : Event { }
        class GotoHot : Event { }
        #endregion

        #region fields
        int CurrentLeader;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        [OnEventGotoState(typeof(Local), typeof(LeaderNotElected))]
        class Init : MonitorState { }

        [Hot]
        [OnEventDoAction(typeof(LeaderElected), nameof(OnLeaderElected))]
        [OnEventGotoState(typeof(GotoHot), typeof(LeaderNotElected))]
        [OnEventGotoState(typeof(GotoCold), typeof(Leader3))]
        class LeaderNotElected : MonitorState { }

        [Cold]
        [OnEventDoAction(typeof(LeaderElected), nameof(OnLeaderElected))]
        [OnEventGotoState(typeof(GotoHot), typeof(LeaderNotElected))]
        class Leader3 : MonitorState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            CurrentLeader = -1;
            Raise(new Local());
        }

        void OnLeaderElected()
        {
            CurrentLeader = (ReceivedEvent as LeaderElected).LeaderId;
            if (CurrentLeader == 3)
                Raise(new GotoCold());
            else
                Raise(new GotoHot());
        }
        #endregion
    }
}
