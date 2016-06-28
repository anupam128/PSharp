using Microsoft.PSharp;

namespace Mailbox
{
    internal class Mailbox : Machine
    {
        internal class MailEvent : Event { }

        private int Counter = 0;

        [Start]
        [OnEventDoAction(typeof(MailEvent), nameof(HandleMail))]
        class Init : MachineState { }

        void HandleMail()
        {
            this.Counter++;
            if (this.Counter == Test.NumOfNodes * Test.NumberOfSendsPerNode)
            {
                System.Console.WriteLine("... Events handled: " + this.Counter);
                this.Raise(new Halt());
            }
        }
    }
}
