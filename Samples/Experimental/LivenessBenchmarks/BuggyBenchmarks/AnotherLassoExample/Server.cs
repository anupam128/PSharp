using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherLassoExample
{
    class Server : Machine
    {
        #region events
        class Unit : Event { }
        public class Message1 : Event { }
        public class Message : Event
        {
            public int Target;
            public Message(int target)
            {
                this.Target = target;
            }
        }
        public class SetNeighbour : Event
        {
            public MachineId Next;
            public MachineId Another;
            public SetNeighbour(MachineId next, MachineId another = null)
            {
                this.Next = next;
                this.Another = another;
            }
        }
        public class Config : Event
        {
            public int ServerId;
            public Config(int serverId)
            {
                this.ServerId = serverId;
            }
        }
        #endregion

        #region fields
        MachineId Neighbour;
        MachineId Neighbour1;
        int Value;
        int counter;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnInitEntry))]
        [OnEventDoAction(typeof(SetNeighbour), nameof(OnSetNeighbour))]
        [OnEventGotoState(typeof(Unit), typeof(Waiting))]
        class Init : MachineState { }

        [OnEventDoAction(typeof(Message), nameof(OnMessage))]
        [OnEventDoAction(typeof(Message1), nameof(OnMessage1))]
        [OnEventDoAction(typeof(Default), nameof(OnDefault))]
        class Waiting : MachineState { }
        #endregion

        #region actions
        void OnInitEntry()
        {
            var e = ReceivedEvent as Config;
            Value = e.ServerId;
            counter = 0;
        }
        void OnMessage()
        {
            var e = ReceivedEvent as Message;
            if(Value == 1)
            {
                Send(Id, new Message(Value));
                counter++;
            }
        }
        void OnDefault()
        {
            if (Value == 2)
                counter++;
        }
        void OnMessage1()
        {
            Send(Neighbour, new Message(Value));
        }
        void OnSetNeighbour()
        {
            var e = ReceivedEvent as SetNeighbour;
            Neighbour = e.Next;
            Neighbour1 = e.Another;
            Raise(new Unit());
        }
        #endregion
    }
}
