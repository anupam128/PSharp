using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LassoExample
{
    class Server : Machine
    {
        #region events
        public class Config : Event
        {
            public int ServerId;
            public Config(int serverId)
            {
                this.ServerId = serverId;
            }
        }
        public class SetNeighbour : Event
        {
            public MachineId Next;
            public SetNeighbour(MachineId next)
            {
                this.Next = next;
            }
        }
        public class Message : Event
        {
            public int Value;
            public Message(int value)
            {
                this.Value = value;
            }
        }
        #endregion

        #region fields
        int ServerId;
        MachineId Next;
        int Value;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        [OnEventDoAction(typeof(SetNeighbour), nameof(OnSetNeighbour))]
        [OnEventDoAction(typeof(Message), nameof(OnMessage))]
        class Init : MachineState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            var e = ReceivedEvent as Config;
            this.ServerId = e.ServerId;
            this.Value = 0;
        }
        void OnSetNeighbour()
        {
            var e = ReceivedEvent as SetNeighbour;
            this.Next = e.Next;
        }
        void OnMessage()
        {
            var e = ReceivedEvent as Message;
            this.Value = e.Value;
            if(this.ServerId == 1)
            {
                this.Value++; 
            }
            Send(Next, new Message(Value));
        }
        #endregion
    }
}
