using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paxos
{
    class Timer : Machine
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
        public class StartTimer : Event { }
        public class CancelTimer : Event { }
        public class TimeOut : Event { }
        #endregion

        #region fields
        MachineId client;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }

        [OnEventGotoState(typeof(StartTimer), typeof(WaitForCancel))]
        [OnEventGotoState(typeof(CancelTimer), typeof(WaitForReq))]
        class WaitForReq : MachineState { }

        [IgnoreEvents(typeof(StartTimer))]
        [OnEventDoAction(typeof(CancelTimer), nameof(OnCancel))]
        [OnEventDoAction(typeof(Default), nameof(OnDefault))]
        class WaitForCancel : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            client = (ReceivedEvent as Config).Target;
            Goto(typeof(WaitForReq));
        }

        void OnCancel()
        {
            if (Random()) {  }
            else
            {
                Send(client, new TimeOut());
            }
            Goto(typeof(WaitForReq));
        }

        void OnDefault()
        {
            Send(client, new TimeOut());
            Goto(typeof(WaitForReq));
        }
        #endregion
    }
}

