using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class Timer : Machine
    {
        internal class Config : Event
        {
            public MachineId Target;

            public Config(MachineId target)
            {
                this.Target = target;
            }
        }
        class Unit : Event { }
        public class Timeout : Event { }

        MachineId Target;
        int Counter;

        [Start]
        [OnEntry(nameof(InitOnEntryAction))]
        [OnEventGotoState(typeof(Unit), typeof(Active))]
        class Init : MachineState { }

        void InitOnEntryAction()
        {
            this.Target = (this.ReceivedEvent as Config).Target;
            this.Counter = 0;
            
            this.Raise(new Unit());
        }

        [OnEntry(nameof(ProcessTickEvent))]
        [OnEventGotoState(typeof(Unit), typeof(Active))]
        class Active : MachineState { }

        void ProcessTickEvent()
        {
            this.Counter++;
            if (this.FairRandom() && Counter == 10)
            {
                Counter = 0;
                this.Send(this.Target, new Timeout());
            }
            
            this.Raise(new Unit());
        }
    }
}
