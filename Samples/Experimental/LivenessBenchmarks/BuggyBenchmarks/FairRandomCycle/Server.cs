using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairRandomCycle
{
    class Server : Machine
    {
        #region events
        public class Message : Event { }
        #endregion

        #region states
        [Start]
        [OnEventDoAction(typeof(Message), nameof(OnMessage))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnMessage()
        {
            if (FairRandom())
            {
                //Send(Id, new Message());
            }
            Send(Id, new Message());
        }
        #endregion
    }
}
