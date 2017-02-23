using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paxos
{
    public class ProposalIdType
    {
        public int serverid;
        public int round;
        public ProposalIdType()
        {

        }
        public ProposalIdType(int serverid, int round)
        {
            this.serverid = serverid;
            this.round = round;
        }
    }
    public class ProposalType
    {
        public ProposalIdType pid;
        public int value;
        public ProposalType()
        {
            pid = new ProposalIdType();
        }
        public ProposalType(ProposalIdType pid, int value)
        {
            this.pid = pid;
            this.value = value;
        }
    }

    class AcceptorMachine : Machine
    {
        #region events
        public class prepare : Event
        {
            public MachineId proposer;
            public ProposalType proposal;
            public prepare(MachineId proposer, ProposalType proposal)
            {
                this.proposer = proposer;
                this.proposal = proposal;
            }
        }
        public class accept : Event
        {
            public MachineId proposer;
            public ProposalType proposal;
            public accept(MachineId proposer, ProposalType proposal)
            {
                this.proposer = proposer;
                this.proposal = proposal;
            }
        }
        #endregion

        #region fields
        ProposalType lastRecvProposal;
        #endregion

        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }

        [OnEventDoAction(typeof(prepare), nameof(OnPrepare))]
        [OnEventDoAction(typeof(accept), nameof(OnAccept))]
        class WaitForRequests : MachineState { }
        #endregion

        #region actions
        void InitOnEntry()
        {
            lastRecvProposal = new ProposalType();
            this.Goto(typeof(WaitForRequests));
        }

        void OnPrepare()
        {
            var payload = ReceivedEvent as prepare;
            if (lastRecvProposal.value == 0)
            {
                Send(payload.proposer, new ProposerMachine.agree(new ProposalType()));
                lastRecvProposal = payload.proposal;
            }
            else if (ProposalLessThan(payload.proposal.pid, lastRecvProposal.pid))
            {
                Send(payload.proposer, new ProposerMachine.reject(lastRecvProposal.pid));
            }
            else
            {
                Send(payload.proposer, new ProposerMachine.agree(lastRecvProposal));
                lastRecvProposal = payload.proposal;
            }
        }

        void OnAccept()
        {
            var payload = ReceivedEvent as accept;
            if (!ProposalIdEqual(payload.proposal.pid, lastRecvProposal.pid))
            {
                Send(payload.proposer, new ProposerMachine.reject(lastRecvProposal.pid));
            }
            else
            {
                Send(payload.proposer, new ProposerMachine.accepted(payload.proposal));
                this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyAccepted());
            }
        }
        #endregion

        #region static methods
        internal static bool ProposalIdEqual(ProposalIdType id1, ProposalIdType id2)
        {
            if (id1.serverid == id2.serverid && id1.round == id2.round)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool ProposalLessThan(ProposalIdType id1, ProposalIdType id2)
        {
            if (id1.round < id2.round)
            {
                return true;
            }
            else if (id1.round == id2.round)
            {
                if (id1.serverid < id2.serverid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

    }
}