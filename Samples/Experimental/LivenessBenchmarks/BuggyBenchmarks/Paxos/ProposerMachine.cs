using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paxos
{
    class ProposerMachine : Machine
    {
        #region events
        public class Config : Event
        {
            public List<MachineId> Acceptors;
            public int ServerId;
            public Config(List<MachineId> acceptors, int serverId)
            {
                this.Acceptors = acceptors;
                this.ServerId = serverId;
            }
        }
        public class agree : Event
        {
            public ProposalType propType;
            public agree(ProposalType propType)
            {
                this.propType = propType;
            }
        }
        public class reject : Event
        {
            public ProposalIdType proposalIdType;
            public reject(ProposalIdType proposalIdType)
            {
                this.proposalIdType = proposalIdType;
            }
        }
        public class accepted : Event
        {
            public ProposalType propType;
            public accepted(ProposalType propType)
            {
                this.propType = propType;
            }
        }
        #endregion

        #region fields
        List<MachineId> acceptors;
        int majority;
        int serverid;
        MachineId timer;
        int proposeValue;
        ProposalIdType nextProposalId;

        int numOfAgreeRecv;
        int numOfAcceptRecv;
        ProposalType promisedAgree;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }

        [IgnoreEvents(typeof(accepted))]
        [OnEntry(nameof(ProposerPhaseOneEntry))]
        [OnEventDoAction(typeof(agree), nameof(OnAgree))]
        [OnEventDoAction(typeof(reject), nameof(OnReject))]
        [OnEventGotoState(typeof(Timer.TimeOut), typeof(ProposerPhaseOne))]
        class ProposerPhaseOne : MachineState { }

        [IgnoreEvents(typeof(agree))]
        [OnEntry(nameof(ProposerPhaseTwoEntry))]
        [OnEventGotoState(typeof(Timer.TimeOut), typeof(ProposerPhaseOne))]
        [OnEventDoAction(typeof(reject), nameof(OnReject2))]
        [OnEventDoAction(typeof(accepted), nameof(OnAccepted))]
        class ProposerPhaseTwo : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            var payload = ReceivedEvent as Config;
            acceptors = payload.Acceptors;
            serverid = payload.ServerId;
            //propose some random value;
            proposeValue = serverid * 10 + 1;
            nextProposalId = new ProposalIdType(serverid, 1);
            majority = 3 / 2 + 1;
            timer = CreateMachine(typeof(Timer), new Timer.Config(Id));

            promisedAgree = new ProposalType();

            this.Goto(typeof(ProposerPhaseOne));
        }
        
        void ProposerPhaseOneEntry()
        {
            numOfAgreeRecv = 0;
            SendToAllAcceptors(new AcceptorMachine.prepare(Id, new ProposalType(nextProposalId, proposeValue)));
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyPrepare());
            Send(timer, new Timer.StartTimer());
        }

        void OnAgree()
        {
            var payload = (ReceivedEvent as agree).propType;
            numOfAgreeRecv = numOfAgreeRecv + 1;
            if (AcceptorMachine.ProposalLessThan(promisedAgree.pid, payload.pid))
            {
                promisedAgree = payload;
            }
            if (numOfAgreeRecv == majority)
            {
                //cancel the timer and goto next phase
                Send(timer, new Timer.CancelTimer());
                this.Goto(typeof(ProposerPhaseTwo));
            }
        }

        void OnReject()
        {
            ProposalIdType payload = (ReceivedEvent as reject).proposalIdType;
            if (nextProposalId.round <= payload.round)
            {
                nextProposalId.round = payload.round + 1;
            }
            Send(timer, new Timer.CancelTimer());
            this.Goto(typeof(ProposerPhaseOne));
        }

        void ProposerPhaseTwoEntry()
        {
            numOfAcceptRecv = 0;
            proposeValue = GetValueToBeProposed();
            SendToAllAcceptors(new AcceptorMachine.accept(Id, new ProposalType(nextProposalId, proposeValue)));
            Send(timer, new Timer.StartTimer());
        }

        void OnReject2()
        {
            ProposalIdType payload = (ReceivedEvent as reject).proposalIdType;
            if (nextProposalId.round <= payload.round)
            {
                nextProposalId.round = payload.round;
            }
            Send(timer, new Timer.CancelTimer());
            this.Goto(typeof(ProposerPhaseOne));
        }

        void OnAccepted()
        {
            var payload = (ReceivedEvent as accepted).propType;
            if (AcceptorMachine.ProposalIdEqual(payload.pid, nextProposalId))
            {
                numOfAcceptRecv = numOfAcceptRecv + 1;
            }

            if (numOfAcceptRecv == majority)
            {
                Send(timer, new Timer.CancelTimer()); ;
                this.Assert(false);
                // done proposing lets halt
                Raise(new Halt());
            }
        }
        #endregion

        #region private methods
        int GetValueToBeProposed()
        {
            if (promisedAgree.value == 0)
            {
                return proposeValue;
            }
            else
            {
                return promisedAgree.value;
            }
        }

        void SendToAllAcceptors(Event e)
        {
            int index;
            index = 0;
            while (index < acceptors.Count)
            {
                Send(acceptors[index], e);
                index = index + 1;
            }
        }
        #endregion
    }
}