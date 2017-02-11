using System;
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace PingPong
{
    internal class Server : Machine
    {
		internal class Pong : Event { }

        [Start]
        [OnEventDoAction(typeof(Client.Ping), nameof(SendPong))]
        class Active : MachineState { }

        async Task SendPong()
        {
			var client = (this.ReceivedEvent as Client.Ping).Client;
            await this.Send(client, new Pong());
        }
    }
}
