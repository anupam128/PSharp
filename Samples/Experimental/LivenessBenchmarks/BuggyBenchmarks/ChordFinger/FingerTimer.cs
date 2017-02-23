using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFinger
{
    class FingerTimer : Machine
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
        int Counter;
        #endregion

        #region states
        [Start]
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
            Counter = 0;
            Raise(new Local());
        }

        void Process()
        {
            Counter++;
            if (Counter == 10)
            {
                Counter = 0;
                Send(Id, new Tick());
            }
            else
                Send(Id, new Local());
        }

        void OnTick()
        {
            Send(Target, new ChordNode.FixFingers());
            Raise(new Local());
        }
        #endregion
    }
}
