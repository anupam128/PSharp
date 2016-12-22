using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredEvents
{
    class Client : Machine
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

        public class Request : Event { }
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnConfig))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnConfig()
        {
            var e = (ReceivedEvent as Config).Target;
            Send(e, new Request());
        }
        #endregion
    }
}
