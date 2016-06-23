using System.Collections.Generic;
using Microsoft.PSharp;

namespace Mailbox
{
    internal class Environment : Machine
    {
        internal class Config : Event
        {
            public int NumberOfNodes;
            public int NumberOfSendsPerNode;

            public Config(int numOfNodes, int numOfSends)
                : base()
            {
                this.NumberOfNodes = numOfNodes;
                this.NumberOfSendsPerNode = numOfSends;
            }
        }

        internal class Ready : Event { }

        private List<MachineId> Nodes;
        private int ReadyCounter;

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventDoAction(typeof(Ready), nameof(BeginMailbox))]
        class Init : MachineState { }

        void InitOnEntry()
        {
            int numOfNodes = (this.ReceivedEvent as Config).NumberOfNodes;
            int numOfSends = (this.ReceivedEvent as Config).NumberOfSendsPerNode;

            this.Nodes = new List<MachineId>();
            this.ReadyCounter = 0;

            var mailbox = this.CreateMachine(typeof(Mailbox));

            for (int idx = 0; idx < numOfNodes; idx++)
            {
                var node = this.CreateMachine(typeof(Node),
                    new Node.Config(this.Id, mailbox, numOfSends));
                this.Nodes.Add(node);
            }
        }

        void BeginMailbox()
        {
            this.ReadyCounter++;
            if (this.ReadyCounter == this.Nodes.Count)
            {
                Test.Profiler.StartMeasuringExecutionTime();

                foreach (var node in this.Nodes)
                {
                    this.Send(node, new Node.Begin());
                }
            }
        }
    }
}
