﻿using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraEvents
{
    class Server : Machine
    {
        #region events
        public class Event1 : Event { }
        public class Event2 : Event { }
        public class Event3 : Event { }
        #endregion

        #region fields
        int ctr1;
        int ctr2;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [DeferEvents(typeof(Event3))]
        [OnEventDoAction(typeof(Event1), nameof(OnEvent1))]
        [OnEventDoAction(typeof(Event2), nameof(OnEvent2))]
        [OnEventDoAction(typeof(Timer.TimerTickEvent), nameof(OnTimeout))]
        class Init : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            ctr1 = 0;
            ctr2 = 0;
            CreateMachine(typeof(Timer), new Timer.Config(this.Id));
        }
        void OnEvent1()
        {
            ctr1++;
        }
        void OnEvent2()
        {
            ctr2++;
        }
        void OnTimeout()
        {
            if(ctr2 > ctr1)
            {
                this.Monitor<LivenessMonitor>(new LivenessMonitor.CtrExceeded());
            }
        }
        #endregion
    }
}
