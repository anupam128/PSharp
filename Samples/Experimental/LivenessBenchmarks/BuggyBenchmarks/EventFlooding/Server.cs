using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFlooding
{
    class Server : Machine
    {
        #region events
        public class Event1 : Event { }
        public class Event2 : Event { }
        class Local : Event { }
        #endregion

        #region fields
        int ctr1;
        int ctr2;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventGotoState(typeof(Local), typeof(Waiting))]
        class Init : MachineState { }

        [DeferEvents(typeof(Event1), typeof(Event2))]
        [OnEventDoAction(typeof(Timer.TimerTickEvent), nameof(OnTimeout))]
        [OnEventGotoState(typeof(Local), typeof(Handling))]
        class Waiting : MachineState { }

        [OnEventDoAction(typeof(Event1), nameof(OnEvent1))]
        [OnEventDoAction(typeof(Event2), nameof(OnEvent2))]
        [OnEventGotoState(typeof(Local), typeof(Waiting))]
        [IgnoreEvents(typeof(Timer.TimerTickEvent))]
        class Handling : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            ctr1 = 0;
            ctr2 = 0;
            CreateMachine(typeof(Timer), new Timer.Config(this.Id));
            Raise(new Local());
        }
        void OnEvent1()
        {
            ctr1++;
        }
        void OnEvent2()
        {
            ctr2++;
            Raise(new Local());
        }
        void OnTimeout()
        {
            if(ctr2 > ctr1)
            {
                this.Monitor<LivenessMonitor>(new LivenessMonitor.CtrExceeded());
            }
            Raise(new Local());
        }
        #endregion
    }
}
