using Microsoft.PSharp;

namespace Mailbox
{
    internal class Mailbox : Machine
    {
        internal class MailEvent : Event { }

        [Start]
        [OnEventDoAction(typeof(MailEvent), nameof(HandleMail))]
        class Init : MachineState { }

        void HandleMail()
        {
            //System.Console.WriteLine("test");
        }
    }
}
