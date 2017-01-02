using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class Server : Machine
    {
        #region events
        public class FirstEvent : Event{ }
        public class SecondEvent : Event { }
        public class ThirdEvent : Event { }
        class Local : Event { }
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventGotoState(typeof(Local), typeof(Waiting))]
        class Init : MachineState { }

        [DeferEvents(typeof(FirstEvent), typeof(SecondEvent), typeof(ThirdEvent))]
        [OnEventGotoState(typeof(Timer.Timeout), typeof(HandlingEvents))]
        class Waiting : MachineState { }

        [OnEventDoAction(typeof(FirstEvent), nameof(OnFirstEvent))]
        [OnEventDoAction(typeof(SecondEvent), nameof(OnSecondEvent))]
        [DeferEvents(typeof(ThirdEvent))]
        [OnEventGotoState(typeof(Timer.Timeout), typeof(Waiting))]
        class HandlingEvents : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            CreateMachine(typeof(Timer), new Timer.Config(this.Id));
            Raise(new Local());
        }

        void OnFirstEvent()
        {
            Console.WriteLine("Handling First Event");
            this.Monitor<LivenessMonitor>(new LivenessMonitor.GotFirstEvent());
        }

        void OnSecondEvent()
        {
            Console.WriteLine("Handling Second Event");
            this.Monitor<LivenessMonitor>(new LivenessMonitor.GotSecondEvent());
        }

        #endregion
    }
}
