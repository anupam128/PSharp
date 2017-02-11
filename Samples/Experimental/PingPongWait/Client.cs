using System;
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace PingPong
{
    internal class Client : Machine
    {
        private MachineId Server;
        private int Counter;

        [Start]
        [OnEventDoAction(typeof(Config), nameof(Configure))]
        [OnEventGotoState(typeof(Unit), typeof(Active))]
        class Init : MachineState { }

        Task Configure()
        {
            this.Server = (this.ReceivedEvent as Config).Id;
            this.Counter = 0;
            this.Raise(new Unit());
			return this.DoneTask;
        }

        [OnEntry(nameof(ActiveOnEntry))]
        class Active : MachineState { }

        async Task ActiveOnEntry()
        {
            while (this.Counter < 5)
            {
                await this.Receive(typeof(Ping));
                this.SendPong();
            }

            this.Raise(new Halt());
        }

        private void SendPong()
        {
            this.Counter++;
            Console.WriteLine("\nTurns: {0} / 5\n", this.Counter);
            this.Send(this.Server, new Pong());
        }
    }
}
