using System;
using System.Threading.Tasks;

using Microsoft.PSharp;

namespace PingPong
{
    internal class Environment : Machine
    {
		[Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }

		async Task InitOnEntry()
        {
			var server = await this.CreateMachine(typeof(Server));
			for (int i = 0; i < 3; i++)
			{
				var client = await this.CreateMachine(typeof(Client));
				await this.Send(client, new Client.Config(server));
			}
        }
    }
}
