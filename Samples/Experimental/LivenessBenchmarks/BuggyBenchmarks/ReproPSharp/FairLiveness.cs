using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReproPSharp
{
    class FairLiveness : Machine
    {
        #region events
        class UserEvent : Event { }
        class Done : Event { }
        class Loop : Event { }
        class Waiting : Event { }
        class Computing : Event { }
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(OnWaitForUserEntry))]
        [OnEventGotoState(typeof(UserEvent), typeof(HandleEvent))]
        class WaitForUser : MachineState { }

        [OnEntry(nameof(OnHandleEventEntry))]
        [OnEventGotoState(typeof(Done), typeof(WaitForUser))]
        [OnEventGotoState(typeof(Loop), typeof(HandleEvent))]
        class HandleEvent : MachineState { }
        #endregion

        #region actions
        void OnWaitForUserEntry()
        {
            this.Monitor<LivenessMonitor>(new LivenessMonitor.Waiting());
            Send(Id, new UserEvent());
        }

        void OnHandleEventEntry()
        {
            this.Monitor<LivenessMonitor>(new LivenessMonitor.Computing());
            if (FairRandom())
            {
                Send(Id, new Loop());
            }
            else
            {
                Send(Id, new Loop());
            }
        }
        #endregion
    }
}

class LivenessMonitor : Monitor
{
    #region events
    public class Waiting : Event { }
    public class Computing : Event { }
    #endregion

    #region states
    [Start]
    [Cold]
    [OnEventGotoState(typeof(Waiting), typeof(CanGetUserInput))]
    [OnEventGotoState(typeof(Computing), typeof(CannotGetUserInput))]
    class CanGetUserInput : MonitorState { }

    [Hot]
    [OnEventGotoState(typeof(Waiting), typeof(CanGetUserInput))]
    [OnEventGotoState(typeof(Computing), typeof(CannotGetUserInput))]
    class CannotGetUserInput : MonitorState { }
    #endregion
}
