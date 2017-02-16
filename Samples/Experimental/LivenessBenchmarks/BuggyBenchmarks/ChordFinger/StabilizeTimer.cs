using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFinger
{
    class StabilizeTimer : Machine
    {
        #region events
        public class Config : Event
        {
            public MachineId Target;
            public Config(MachineId target)
            {
                this.Target = target;
            }
        }
        class Local : Event { }
        class Tick : Event { }
        #endregion

        #region fields
        MachineId Target;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventDoAction(typeof(Config), nameof(OnConfig))]
        [OnEventGotoState(typeof(Local), typeof(Active))]
        class Init : MachineState { }

        [OnEntry(nameof(Process))]
        [OnEventDoAction(typeof(Tick), nameof(OnTick))]
        [OnEventGotoState(typeof(Local), typeof(Active))]
        class Active : MachineState { }
        #endregion

        #region actions
        void OnConfig()
        {
            Target = (ReceivedEvent as Config).Target;
            Raise(new Local());
        }

        void Process()
        {
            if(Random())
            {
                Send(Id, new Tick());
            }
            else
                Send(Id, new Local());
        }

        void OnTick()
        {
            Send(Target, new ChordNode.Stabilize());
            Raise(new Local());
        }

        void InitOnEntry()
        {
        }
        #endregion
    }
}
