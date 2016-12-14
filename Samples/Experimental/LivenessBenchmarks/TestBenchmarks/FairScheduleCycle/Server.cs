using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairScheduleCycle
{
    class Server : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        class Init : MachineState { }

        [OnEventDoAction(typeof(Client.Message), nameof(OnMessage))]
        class Waiting : MachineState { }
        #endregion

        #region fields
        MachineId MyClient;
        int count;
        #endregion

        #region actions
        void OnInitEntry()
        {
            MyClient = CreateMachine(typeof(Client));
            count = 0;
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyMessage());
            Send(Id, new Client.Message());
            Send(MyClient, new Client.Message());

            this.Goto(typeof(Waiting));
        }

        void OnMessage()
        {
            count++;
            Console.WriteLine("server scheduled");
            if(count <= 200)
                Send(Id, new Client.Message());
        }
        #endregion
    }
}
