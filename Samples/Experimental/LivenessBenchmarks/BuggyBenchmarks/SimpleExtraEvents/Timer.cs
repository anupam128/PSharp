using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExtraEvents
{
    class Timer : Machine
    {
        public class Config : Event
        {
            public MachineId Target;
            public Config(MachineId target)
            {
                this.Target = target;
            }
        }
        class Unit : Event { } 
        public class TimerTickEvent : Event { }

        MachineId Target;

        [Start]
        [OnEntry(nameof(InitOnEntryAction))]
        [OnEventGotoState(typeof(Unit), typeof(Active))]
        class Init : MachineState { }

        void InitOnEntryAction()
        {
            this.Target = (this.ReceivedEvent as Config).Target;

            this.Raise(new Unit());
        }

        [OnEntry(nameof(ProcessTickEvent))]
        [OnEventGotoState(typeof(Unit), typeof(Active))]
        class Active : MachineState { }

        void ProcessTickEvent()
        {
            if (this.FairRandom())
            {
                this.Send(this.Target, new TimerTickEvent());
            }

            this.Raise(new Unit());
        }
    }
}
