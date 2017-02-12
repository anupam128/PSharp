using System;
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace PingPong
{
    internal class Client : Machine
    {
		internal class Unit : Event { }

		internal class Config : Event
		{
			public MachineId Id;

			public Config(MachineId id)
			{
				this.Id = id;
			}
		}

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
        [OnEventGotoState(typeof(Unit), typeof(Active))]
        class Init : MachineState { }

        async Task Configure()
        {
            this.Server = (this.ReceivedEvent as Config).Id;
            this.Counter = 0;
            await this.Raise(new Unit());
        }

        [OnEntry(nameof(ActiveOnEntry))]
        class Active : MachineState { }

        async Task ActiveOnEntry()
        {
            while (this.Counter < 5)
            {
				await this.SendPing();
                await this.Receive(typeof(Server.Pong));
            }

            await this.Raise(new Halt());
        }

        async Task SendPing()
        {
            this.Counter++;
			Console.WriteLine($"\n'{this.Id}' turns: {this.Counter} / 5\n");
			await this.Send(this.Server, new Ping(this.Id));
        }
    }
}
