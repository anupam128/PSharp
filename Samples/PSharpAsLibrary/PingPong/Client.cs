using System;
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace PingPong
{
    internal class Client : Machine
    {
		internal class Config : Event
		{
			public MachineId Id;

			public Config(MachineId id)
			{
				this.Id = id;
			}
		}

		internal class Unit : Event { }

		internal class Ping : Event
		{
			public MachineId Client;

			public Ping(MachineId client)
			{
				this.Client = client;
			}
		}

        private MachineId Server;
        private int Counter;

        [Start]
        [OnEventDoAction(typeof(Config), nameof(Configure))]
        class Init : MachineState { }

        async Task Configure()
        {
            this.Server = (this.ReceivedEvent as Config).Id;
            this.Counter = 0;
            this.Goto(typeof(Active));
			await this.DoneTask;
        }

        [OnEntry(nameof(ActiveOnEntry))]
        [OnEventDoAction(typeof(Server.Pong), nameof(SendPing))]
        class Active : MachineState { }

        async Task ActiveOnEntry()
        {
			await SendPing();
        }

        async Task SendPing()
        {
			this.Counter++;
			await this.Send(this.Server, new Ping(this.Id));

			Console.WriteLine("\nTurns: {0} / 5\n", this.Counter);

			if (this.Counter == 5)
			{
				this.Raise(new Halt());
			}
        }
    }
}
