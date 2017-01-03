using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExtraEvents
{
    class Server : Machine
    {
        #region events
        public class Event1 : Event { }
        public class Event2 : Event { }
        public class Event3 : Event { }
        class Local : Event { }
        #endregion

        #region fields
        int ctr1;
        int ctr2;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventGotoState(typeof(Local), typeof(HandleEvents))]
        class Init : MachineState { }

        [OnEventDoAction(typeof(Event1), nameof(OnEvent1))]
        [OnEventDoAction(typeof(Event2), nameof(OnEvent2))]
        [DeferEvents(typeof(Event3))]
        [OnEventDoAction(typeof(Timer.TimerTickEvent), nameof(OnTimeout))]
        [OnEventGotoState(typeof(Local), typeof(Handling))]
        class HandleEvents : MachineState { }

        [DeferEvents(typeof(Event1), typeof(Event2))]
        [OnEventDoAction(typeof(Event3), nameof(OnEvent3))]
        [OnEventGotoState(typeof(Timer.TimerTickEvent), typeof(HandleEvents))]
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
        }
        void OnEvent3()
        {
            Console.WriteLine("Handling event 3");
            Send(Id, new Event3());
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
