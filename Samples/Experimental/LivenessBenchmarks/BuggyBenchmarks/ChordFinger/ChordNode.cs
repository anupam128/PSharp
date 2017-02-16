using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordFinger
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
            public FindSuccessorFinger(int id, int target)
            {
                this.Id = id;
                this.Target = target;
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
            public FindSuccessorFingerResp(int succ)
            {
                this.Succ = succ;
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
            public FindSuccessorFingerFwd(int id, int succFwd)
            {
                this.Id = id;
                this.SuccFwd = succFwd;
            }
        }
        public class Create : Event { }
        class Local : Event{ }
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
        Dictionary<int, int> Finger;
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
        [OnEventDoAction(typeof(FindSuccessorFingerResp), nameof(OnFindSuccessorFingerResp))]
        [OnEventDoAction(typeof(FindSuccessorFingerFwd), nameof(OnFindSuccessorFingerFwd))]
        class Waiting : MachineState { }
        #endregion

        #region actions
        void OnFindSuccessor()
        {
            var id = (ReceivedEvent as FindSuccessor).Id;
            var target = (ReceivedEvent as FindSuccessor).Target;

            if (id > ChordId && id <= Successor)
                Send(NodeIDs[target], new FindSuccessorResp(Successor));
            else if(ChordId > Successor && id == Successor)
                Send(NodeIDs[target], new FindSuccessorResp(Successor));
            else if (ChordId == Successor)
                Send(NodeIDs[target], new FindSuccessorResp(Successor));
            else
            {
                int nPrime;
                if (ChordId == id && Finger[1] != ChordId)
                    nPrime = Finger[1];
                else
                    nPrime = ChordId;
                Send(NodeIDs[target], new FindSuccessorFwd(id, nPrime));
            }
        }

        void OnFindSuccessorResp()
        {
            var e = (ReceivedEvent as FindSuccessorResp);
            this.Successor = e.Succ;
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
        }

        void OnFindSuccessorFingerResp()
        {
            var e = (ReceivedEvent as FindSuccessorFingerResp);
            Finger[Next] = e.Succ;
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
        }

        void OnFindSuccessorFwd()
        {
            var e = (ReceivedEvent as FindSuccessorFwd);
            Send(NodeIDs[e.SuccFwd], new FindSuccessor(e.Id, ChordId));
        }

        void OnFindSuccessorFingerFwd()
        {
            var e = (ReceivedEvent as FindSuccessorFingerFwd);
            Send(NodeIDs[e.SuccFwd], new FindSuccessorFinger(e.Id, ChordId));
        }

        void OnFindSuccessorFinger()
        {
            var id = (ReceivedEvent as FindSuccessorFinger).Id;
            var target = (ReceivedEvent as FindSuccessorFinger).Target;

            if (id > ChordId && id <= Successor)
                Send(NodeIDs[target], new FindSuccessorFingerResp(Successor));
            else if (ChordId > Successor && id == Successor)
                Send(NodeIDs[target], new FindSuccessorFingerResp(Successor));
            else if (ChordId == Successor)
                Send(NodeIDs[target], new FindSuccessorFingerResp(Successor));
            else
            {
                int nPrime;
                if (ChordId == id && Finger[1] != ChordId)
                    nPrime = Finger[1];
                else
                    nPrime = ChordId;
                Send(NodeIDs[target], new FindSuccessorFingerFwd(id, nPrime));
            }
        }

        void OnCreate()
        {
            Predecessor = -1;
            Successor = ChordId;
            Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
        }

        void OnConfigure()
        {
            var e = (ReceivedEvent as Configure);
            this.ChordId = e.ChordId;
            this.NodeIDs = e.NodeIds;
            this.Successor = -1;
            this.Predecessor = -1;
            this.Finger = new Dictionary<int, int>();
            Finger.Add(-1, -1);
            Next = 0;
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
            if(Successor > -1)
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
            if(x > ChordId && x < Successor || (ChordId == Successor && x != ChordId && x != -1))
            {
                Successor = x;
                Console.WriteLine(">>>>> Successor : {0} ==> {1}", ChordId, Successor);
            }
            Send(NodeIDs[Successor], new Notify(ChordId));
        }

        void OnNotify()
        {
            var nPrime = (ReceivedEvent as Notify).N;
            if(Predecessor == -1 || 
                (nPrime > Predecessor && nPrime < ChordId || (Predecessor == ChordId && nPrime != ChordId)))
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
            if (Next > 1)
                Next = 1;
            Send(NodeIDs[ChordId], new FindSuccessorFinger(ChordId + (int)Math.Pow(2, Next - 1), ChordId));
        }
        #endregion
    }
}
