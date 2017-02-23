using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFingerBug
{
    class ChordNode : Machine
    {
        #region events
        public class Configure : Event
        {
            public int ChordId;
            public Dictionary<int, MachineId> NodeIds;
            public Configure(int chordId, Dictionary<int, MachineId> nodeIds)
            {
                this.ChordId = chordId;
                this.NodeIds = nodeIds;
            }
        }
        public class PrintSuccessor : Event { }
        class FindSuccessor : Event
        {
            public int Id;
            public int Target;
            public FindSuccessor(int id, int target)
            {
                this.Id = id;
                this.Target = target;
            }
        }
        class FindSuccessorFinger : Event
        {
            public int Id;
            public int Target;
            public int Next;
            public FindSuccessorFinger(int id, int target, int next)
            {
                this.Id = id;
                this.Target = target;
                this.Next = next;
            }
        }
        class FindSuccessorResp : Event
        {
            public int Succ;
            public FindSuccessorResp(int succ)
            {
                this.Succ = succ;
            }
        }
        class FindSuccessorFingerResp : Event
        {
            public int Succ;
            public int Next;
            public FindSuccessorFingerResp(int succ, int next)
            {
                this.Succ = succ;
                this.Next = next;
            }
        }
        class FindSuccessorFwd : Event
        {
            public int Id;
            public int SuccFwd;
            public FindSuccessorFwd(int id, int succFwd)
            {
                this.Id = id;
                this.SuccFwd = succFwd;
            }
        }
        class FindSuccessorFingerFwd : Event
        {
            public int Id;
            public int SuccFwd;
            public int Next;
            public FindSuccessorFingerFwd(int id, int succFwd, int next)
            {
                this.Id = id;
                this.SuccFwd = succFwd;
                this.Next = next;
            }
        }
        public class Create : Event { }
        class Local : Event{ }
        class Fixing : Event { }
        class Fixed : Event { }
        public class Join : Event
        {
            public int JoinToId;
            public Join(int joinToId)
            {
                this.JoinToId = joinToId;
            }
        }
        public class Stabilize : Event { }
        class GetPredecessor : Event
        {
            public int Target;
            public GetPredecessor(int target)
            {
                this.Target = target;
            }
        }
        class GetPredecessorResp : Event
        {
            public int Pred;
            public GetPredecessorResp(int pred)
            {
                this.Pred = pred;
            }
        }
        class Notify : Event
        {
            public int N;
            public Notify(int n)
            {
                this.N = n;
            }
        }
        public class FixFingers : Event { }
        #endregion

        #region fields
        Dictionary<int, MachineId> NodeIDs;
        int Successor;
        int Predecessor;
        int ChordId;
        int[] Finger;
        int Next;
        #endregion

        #region states
        [Start]
        [OnEventDoAction(typeof(Configure), nameof(OnConfigure))]
        [OnEventGotoState(typeof(Local), typeof(Waiting))]
        class Init : MachineState { }

        [OnEventDoAction(typeof(FindSuccessor), nameof(OnFindSuccessor))]
        [OnEventDoAction(typeof(FindSuccessorResp), nameof(OnFindSuccessorResp))]
        [OnEventDoAction(typeof(FindSuccessorFwd), nameof(OnFindSuccessorFwd))]
        [OnEventDoAction(typeof(Create), nameof(OnCreate))]
        [OnEventDoAction(typeof(Join), nameof(OnJoin))]
        [OnEventDoAction(typeof(Stabilize), nameof(OnStabilize))]
        [OnEventDoAction(typeof(GetPredecessor), nameof(OnGetPredecessor))]
        [OnEventDoAction(typeof(GetPredecessorResp), nameof(StabilizeCont))]
        [OnEventDoAction(typeof(Notify), nameof(OnNotify))]
        [OnEventDoAction(typeof(PrintSuccessor), nameof(OnPrintSuccessor))]
        [OnEventDoAction(typeof(FixFingers), nameof(OnFixFingers))]
        [OnEventDoAction(typeof(FindSuccessorFinger), nameof(OnFindSuccessorFinger))]
        [OnEventGotoState(typeof(Fixing), typeof(FixingInProgress))]
        class Waiting : MachineState { }

        [OnEventDoAction(typeof(FindSuccessorFinger), nameof(OnFindSuccessorFinger))]
        [OnEventDoAction(typeof(FindSuccessorFingerResp), nameof(OnFindSuccessorFingerResp))]
        [OnEventDoAction(typeof(FindSuccessorFingerFwd), nameof(OnFindSuccessorFingerFwd))]
        [OnEventGotoState(typeof(Fixed), typeof(Waiting))]

        [OnEventDoAction(typeof(Notify), nameof(OnNotify))]
        [OnEventDoAction(typeof(GetPredecessorResp), nameof(StabilizeCont))]
        [OnEventDoAction(typeof(GetPredecessor), nameof(OnGetPredecessor))]
        [OnEventDoAction(typeof(FindSuccessor), nameof(OnFindSuccessor))]
        [OnEventDoAction(typeof(FindSuccessorResp), nameof(OnFindSuccessorResp))]
        [OnEventDoAction(typeof(FindSuccessorFwd), nameof(OnFindSuccessorFwd))]
        [OnEventDoAction(typeof(Join), nameof(OnJoin))]
        [OnEventDoAction(typeof(Stabilize), nameof(OnStabilize))]
        [OnEventDoAction(typeof(Create), nameof(OnCreate))]

        [IgnoreEvents(typeof(FixFingers))]
        class FixingInProgress : MachineState { }
        #endregion

        #region actions
        void OnFindSuccessor()
        {
            var id = (ReceivedEvent as FindSuccessor).Id;
            var target = (ReceivedEvent as FindSuccessor).Target;

            if (id > ChordId && id <= Successor)
                Send(NodeIDs[target], new FindSuccessorResp(Successor));
            else if(ChordId > Successor && (!(id > Successor && id <= ChordId)))
                Send(NodeIDs[target], new FindSuccessorResp(Successor));
            else if (ChordId == Successor)
                Send(NodeIDs[target], new FindSuccessorResp(Successor));
            else
            {
                int nPrime = -1;
                for(int i  = 2; i >= 1; i--)
                {
                    if(Finger[i] > ChordId && Finger[i] < id || 
                        (ChordId == id && Finger[i] != ChordId) ||
                        (ChordId > id && !(Finger[i] >= id && Finger[i] <= ChordId)))
                    {
                        nPrime = Finger[i];
                    }
                }
                if(nPrime == -1)
                    nPrime = ChordId;
                Send(NodeIDs[target], new FindSuccessorFwd(id, nPrime));
            }
        }

        void OnFindSuccessorResp()
        {
            var e = (ReceivedEvent as FindSuccessorResp);
            this.Successor = e.Succ;
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifySuccessor(ChordId, Successor));
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
        }

        void OnFindSuccessorFingerResp()
        {
            var e = (ReceivedEvent as FindSuccessorFingerResp);
            Finger[e.Next] = e.Succ;
            Console.WriteLine("finger successor: " + ChordId + " " + Next + "==> " + Finger[Next]);
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);

            Console.WriteLine("<< Finger Table for {0}: ", ChordId);
            for (int k = 1; k <= 2; k++)
            {
                Console.WriteLine("<{0}>", Finger[k]);
            }
            Raise(new Fixed());
        }

        void OnFindSuccessorFwd()
        {
            var e = (ReceivedEvent as FindSuccessorFwd);
            Send(NodeIDs[e.SuccFwd], new FindSuccessor(e.Id, ChordId));
        }

        void OnFindSuccessorFingerFwd()
        {
            var e = (ReceivedEvent as FindSuccessorFingerFwd);
            Send(NodeIDs[e.SuccFwd], new FindSuccessorFinger(e.Id, ChordId, e.Next));
        }

        void OnFindSuccessorFinger()
        {
            var id = (ReceivedEvent as FindSuccessorFinger).Id;
            var target = (ReceivedEvent as FindSuccessorFinger).Target;
            var nxt = (ReceivedEvent as FindSuccessorFinger).Next;

            if (id > ChordId && id <= Successor)
            {
                Send(NodeIDs[target], new FindSuccessorFingerResp(Successor, nxt));
            }
            else if (ChordId > Successor && (!(id > Successor && id <= ChordId)))
            {
                Send(NodeIDs[target], new FindSuccessorFingerResp(Successor, nxt));
            }
            else if (ChordId == Successor)
            {
                Send(NodeIDs[target], new FindSuccessorFingerResp(Successor, nxt));
            }
            else
            {
                int nPrime = -1;
                for (int i = 2; i >= 1; i--)
                {
                    if (Finger[i] > ChordId && Finger[i] < id ||
                        (ChordId == id && Finger[i] != ChordId) ||
                        (ChordId > id && !(Finger[i] >= id && Finger[i] <= ChordId)))
                    {
                        nPrime = Finger[i];
                    }
                }
                if (nPrime == -1)
                    nPrime = ChordId;
                Send(NodeIDs[target], new FindSuccessorFingerFwd(id, nPrime, nxt));
            }
        }

        void OnCreate()
        {
            Predecessor = -1;
            Successor = ChordId;
            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifySuccessor(ChordId, Successor));
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
        }

        void OnConfigure()
        {
            var e = (ReceivedEvent as Configure);
            this.ChordId = e.ChordId;
            this.NodeIDs = e.NodeIds;
            this.Successor = -1;
            this.Predecessor = -1;
            this.Finger = new int[] { -1, -1, -1 };
            Next = 0;

            this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifySuccessor(ChordId, Successor));
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);

            var Timer = CreateMachine(typeof(StabilizeTimer));
            Send(Timer, new StabilizeTimer.Config(Id));

            var FinTimer = CreateMachine(typeof(FingerTimer));
            Send(FinTimer, new FingerTimer.Config(Id));

            Raise(new Local());
        }

        void OnJoin()
        {
            var nPrime = (ReceivedEvent as Join).JoinToId;
            this.Predecessor = -1;
            Send(NodeIDs[nPrime], new FindSuccessor(ChordId, ChordId));
        }

        void OnStabilize()
        {
            if (Successor > -1)
                Send(NodeIDs[Successor], new GetPredecessor(ChordId));
        }

        void OnGetPredecessor()
        {
            var e = (ReceivedEvent as GetPredecessor);
            Send(NodeIDs[e.Target], new GetPredecessorResp(Predecessor));
        }

        void StabilizeCont()
        {
            var e = (ReceivedEvent as GetPredecessorResp);
            var x = e.Pred;
            if (x > ChordId && x < Successor || (ChordId == Successor && x != ChordId && x != -1) ||
                (ChordId > Successor && !(x >= Successor && x <= ChordId) && x != -1))
            {
                if(ChordId != 0)
                {
                    Successor = x;
                    this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifySuccessor(ChordId, Successor));
                    Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
                }
            }
            if (Successor != -1)
                Send(NodeIDs[Successor], new Notify(ChordId));
        }

        void OnNotify()
        {
            var nPrime = (ReceivedEvent as Notify).N;
            if (Predecessor == -1 ||
                (nPrime > Predecessor && nPrime < ChordId || (Predecessor == ChordId && nPrime != ChordId)) ||
                (Predecessor > ChordId && !(nPrime >= ChordId && nPrime <= Predecessor)))
            {
                Predecessor = nPrime;
                Console.WriteLine(">>>>> Predecessor : {0} ==> {1}", ChordId, Predecessor);
            }
        }
        
        void OnPrintSuccessor()
        {
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
            Console.WriteLine(">>>>> Predecessor : {0} ==> {1}", ChordId, Predecessor);
        }

        void OnFixFingers()
        {
            Next = Next + 1;
            if (Next > 2)
                Next = 1;
            Console.WriteLine("Fixing finger for {0}, {1}", ChordId, Next);
            Send(NodeIDs[ChordId], new FindSuccessorFinger((ChordId + (int)Math.Pow(2, Next - 1)) % 4, ChordId, Next));
            Raise(new Fixing());
        }
        #endregion
    }
}
