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
        [OnEntry(nameof(OnInitEntry))]
        [OnEventDoAction(typeof(Message), nameof(OnMessage))]
        class Init : MachineState { }
        #endregion

        #region fields
        int Counter;
        #endregion

        #region actions
        void OnMessage()
        {
            Counter++;
            if (FairRandom())
            {
                //Send(Id, new Message());
            }
            if(Counter <= 500)
                Send(Id, new Message());
        }

        void OnInitEntry()
        {
            Counter = 0;
        }
        #endregion
    }
}
