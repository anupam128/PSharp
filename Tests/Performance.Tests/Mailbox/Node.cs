using Microsoft.PSharp;

namespace Mailbox
{
    internal class Node : Machine
    {
        internal class Config : Event
        {
            public MachineId Environment;
            public MachineId Mailbox;
            public int NumberOfSends;

            public Config(MachineId env, MachineId mailbox, int numOfSends)
                : base()
            {
                this.Environment = env;
                this.Mailbox = mailbox;
                this.NumberOfSends = numOfSends;
            }
        }

        internal class Begin : Event { }

        private MachineId Environment;
        private MachineId Mailbox;
        private int NumberOfSends;

        [Start]
        [OnEntry(nameof(InitOnEntry))]
        [OnEventDoAction(typeof(Begin), nameof(StartSending))]
        class Init : MachineState { }

        void InitOnEntry()
        {
            this.Environment = (this.ReceivedEvent as Config).Environment;
            this.Mailbox = (this.ReceivedEvent as Config).Mailbox;
            this.NumberOfSends = (this.ReceivedEvent as Config).NumberOfSends;
            this.Send(this.Environment, new Environment.Ready());
        }

        void StartSending()
        {
            for (int idx = 0; idx < this.NumberOfSends; idx++)
            {
                this.Send(this.Mailbox, new Mailbox.MailEvent());
            }
        }
    }
}
