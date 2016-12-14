using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairScheduleCycle
{
    class Client : Machine
    {
        #region events
        public class Message : Event { }
        #endregion

        #region fields
        int count;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        [OnEventDoAction(typeof(Message), nameof(OnMessage))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            count = 0;
        }
        void OnMessage()
        {
            count++;
            Console.WriteLine("client scheduled");
            if(count <= 200)
                Send(Id, new Message());
        }
        #endregion
    }
}
