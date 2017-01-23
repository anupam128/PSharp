using System;
using System.Collections.Generic;
using Microsoft.PSharp;

namespace BoundedRaft
{
    internal class ElectionTimer : Machine
    {
        internal class ConfigureEvent : Event
        {
            public MachineId Target;

            public ConfigureEvent(MachineId id)
                : base()
            {
                this.Target = id;
            }
        }

        internal class StartTimer : Event { }
        internal class CancelTimer : Event { }
        internal class Timeout : Event { }
        private class Local : Event { }

        private class TickEvent : Event { }

        MachineId Target;
        int Counter;

        [Start]
        [OnEventDoAction(typeof(ConfigureEvent), nameof(Configure))]
        [OnEventGotoState(typeof(StartTimer), typeof(Active))]
        class Init : MachineState { }

        void Configure()
        {
            Counter = 0;
            this.Target = (this.ReceivedEvent as ConfigureEvent).Target;
        }

        [OnEntry(nameof(ActiveOnEntry))]
        [OnEventDoAction(typeof(TickEvent), nameof(Tick))]
        [OnEventGotoState(typeof(CancelTimer), typeof(Inactive))]
        [IgnoreEvents(typeof(StartTimer), typeof(Local))]
        class Active : MachineState { }

        void ActiveOnEntry()
        {
            this.Send(this.Id, new TickEvent());
        }

        void Tick()
        {
            if (this.Random())
            {
                Console.WriteLine("\n [ElectionTimer] " + this.Target + " | timed out\n");
                this.Send(this.Target, new Timeout(), true);
            }

            //this.Send(this.Id, new TickEvent());
            this.Raise(new CancelTimer());
        }
        
        [OnEntry(nameof(InactiveOnEntry))]
        [OnEventGotoState(typeof(StartTimer), typeof(Active))]
        [IgnoreEvents(typeof(CancelTimer), typeof(TickEvent))]
        [OnEventGotoState(typeof(Local), typeof(Inactive))]
        class Inactive : MachineState { }

        void InactiveOnEntry()
        {
            Counter++;
            if (Counter == 1000)
            {
                Counter = 0;
                Send(this.Id, new StartTimer());
            }
            else
            {
                this.Send(this.Id, new Local());
            }
        }
    }
}

