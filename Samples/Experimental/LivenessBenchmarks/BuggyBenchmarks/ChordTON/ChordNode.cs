using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordTON
{
    class ChordNode : Machine
    {
        #region events
        public class Config : Event
        {
            public Dictionary<int, MachineId> nodeIds;
            public Config(Dictionary<int, MachineId> nodeIds)
            {
                this.nodeIds = nodeIds;
            }
        }
        public class SetId : Event
        {
            public int ChordId;
            public SetId(int chordId)
            {
                this.ChordId = chordId;
            }
        }
        public class Create : Event { }
        public class Join : Event
        {
            public int JoinTo;
            public Join(int joinTo)
            {
                this.JoinTo = joinTo;
            }
        }
        public class FindSuccessorReq : Event
        {
            public int FindId;
            public int Target;
            public FindSuccessorReq(int findId, int target)
            {
                this.FindId = findId;
                this.Target = target;
            }
        }
        public class FindSuccessorResp : Event
        {
            public int Succ;
            public FindSuccessorResp(int succ)
            {
                this.Succ = succ;
            }
        }
        public class getPredecessorReq : Event
        {
            public int ReqId;
            public getPredecessorReq(int reqId)
            {
                this.ReqId = reqId;
            }
        }
        public class getPredecessorResp : Event
        {
            public int PredId;
            public getPredecessorResp(int pred)
            {
                this.PredId = pred;
            }
        }
        public class Stabilize : Event { }
        public class Notify : Event
        {
            public int PredecessorId;
            public Notify(int predecessor)
            {
                this.PredecessorId = predecessor;
            }
        }
        public class FixFingers : Event { }
        public class PrintInfo : Event { }
        class Local : Event { }
        class Local1 : Event { }
        class Local2 : Event { }
        #endregion

        #region fields
        int Predecessor;
        int Successor;
        int Next;
        Dictionary<int, int> finger;
        int ChordId;

        Dictionary<int, MachineId> NodeIds;
        #endregion

        #region states
        [Start]
        [OnEventDoAction(typeof(Config), nameof(OnConfig))]
        [OnEventDoAction(typeof(SetId), nameof(OnSetId))]
        [OnEventGotoState(typeof(Local), typeof(Waiting))]
        class Init : MachineState { }

        [OnEventDoAction(typeof(Create), nameof(OnCreate))]
        [OnEventDoAction(typeof(Join), nameof(OnJoin))]
        [OnEventDoAction(typeof(Stabilize), nameof(OnStabilize))]
        [OnEventDoAction(typeof(Notify), nameof(OnNotify))]
        [OnEventDoAction(typeof(FindSuccessorReq), nameof(OnFindSuccessorReq))]
        [OnEventDoAction(typeof(getPredecessorReq), nameof(OngetPredecessorReq))]
        [OnEventDoAction(typeof(getPredecessorResp), nameof(StabilizeCont))]
        [OnEventDoAction(typeof(FindSuccessorResp), nameof(OnFindSuccessorResp))]
        [OnEventDoAction(typeof(FixFingers), nameof(OnFixFingers))]
        [OnEventGotoState(typeof(Local), typeof(FixingFingers))]
        [OnEventDoAction(typeof(PrintInfo), nameof(OnPrintInfo))]
        [OnEventGotoState(typeof(Local1), typeof(Joining))]
        [OnEventGotoState(typeof(Local2), typeof(Stabilizing))]
        class Waiting : MachineState { }

        [OnEventDoAction(typeof(FindSuccessorResp), nameof(OnSuccesssorRespFix))]
        [OnEventGotoState(typeof(Local), typeof(Waiting))]
        [OnEventDoAction(typeof(FindSuccessorReq), nameof(OnFindSuccessorReq))]
        [OnEventDoAction(typeof(getPredecessorReq), nameof(OngetPredecessorReq))]
        [DeferEvents(typeof(getPredecessorResp), typeof(Notify), typeof(FixFingers),
            typeof(Stabilize), typeof(PrintInfo))]
        class FixingFingers : MachineState { }

        [OnEventDoAction(typeof(FindSuccessorResp), nameof(OnFindSuccessorResp))]
        [OnEventGotoState(typeof(Local1), typeof(Waiting))]
        [DeferEvents(typeof(Stabilize), typeof(PrintInfo))]
        class Joining : MachineState { }

        [OnEventDoAction(typeof(FindSuccessorResp), nameof(OnFindSuccessorRespStab))]
        [OnEventGotoState(typeof(Local2), typeof(Waiting))]
        //[OnEventDoAction(typeof(Notify), nameof(OnNotify))]
        //[OnEventDoAction(typeof(getPredecessorReq), nameof(OngetPredecessorReq))]
        //[OnEventDoAction(typeof(getPredecessorResp), nameof(StabilizeCont))]
        [DeferEvents(typeof(FindSuccessorReq), typeof(PrintInfo), typeof(getPredecessorResp), typeof(getPredecessorReq))]
        class Stabilizing : MachineState { }
        #endregion

        #region actions
        void OnPrintInfo()
        {
            Console.WriteLine("================ Node: " + ChordId);
            Console.WriteLine("Successor: " + Successor);
            Console.WriteLine("Predecessor: " + Predecessor);
            foreach(var f in finger)
            {
                Console.WriteLine(f.Key + " --> " + f.Value);
            }
        }

        void OnConfig()
        {
            this.NodeIds = (ReceivedEvent as Config).nodeIds;
            finger = new Dictionary<int, int>();
            Next = 0;
            this.Successor = -1;
            this.Predecessor = -1;
            Raise(new Local());
        }
        void OnSetId()
        {
            this.ChordId = (ReceivedEvent as SetId).ChordId;
        }

        void OnCreate()
        {
            this.Predecessor = -1;
            this.Successor = this.ChordId;
        }

        void OnJoin()
        {
            int joinToNodeId = (ReceivedEvent as Join).JoinTo;
            this.Predecessor = -1;
            this.Send(NodeIds[joinToNodeId], new FindSuccessorReq(this.ChordId, this.ChordId));
            Raise(new Local1());
        }

        void OnStabilize()
        {
            Send(NodeIds[Successor], new getPredecessorReq(this.ChordId));
            Raise(new Local2());
        }

        void StabilizeCont()
        { 
            var x = (ReceivedEvent as getPredecessorResp).PredId;
            if(Belongs(x, this.ChordId + 1, Successor - 1))
            {
                this.Successor = x;
            }
            Send(NodeIds[Successor], new Notify(this.ChordId));
        }

        void OnNotify()
        {
            var receivedId = (this.ReceivedEvent as Notify).PredecessorId;
            if(this.Predecessor == -1 || Belongs(receivedId, Predecessor + 1, ChordId - 1))
            {
                Predecessor = receivedId;
            }
        }

        void OnFindSuccessorReq()
        {
            var id = (ReceivedEvent as FindSuccessorReq).FindId;
            var target = (ReceivedEvent as FindSuccessorReq).Target;
            if (Belongs(id, ChordId + 1, this.Successor))
            {
                Send(NodeIds[target], new FindSuccessorResp(this.Successor));
            }
            else
            {
                int i;
                int nPrime = -1;
                for(i = 2; i>=0; i--)
                {
                    if(Belongs(finger[i], ChordId + 1, id - 1))
                    {
                        nPrime = finger[i];
                    }
                }
                if(i < 0)
                {
                    nPrime = ChordId;
                }
                Send(NodeIds[nPrime], new FindSuccessorReq(id, this.ChordId));
                var receivedEvt = Receive(typeof(FindSuccessorResp));
                Send(NodeIds[target], new FindSuccessorResp((receivedEvt as FindSuccessorResp).Succ));
            }
        }

        void OngetPredecessorReq()
        {
            var Target = (ReceivedEvent as getPredecessorReq).ReqId;
            Send(NodeIds[Target], new getPredecessorResp(this.Predecessor));
        }

        void OnFindSuccessorResp()
        {
            this.Successor = (ReceivedEvent as FindSuccessorResp).Succ;
            Raise(new Local1());
        }

        void OnFindSuccessorRespStab()
        {
            this.Successor = (ReceivedEvent as FindSuccessorResp).Succ;
            Raise(new Local2());
        }

        void OnFixFingers()
        {
            if (Next > 2)
            {
                Next = 0;
            }
            Send(Id, new FindSuccessorReq((ChordId + (int)(Math.Pow(2, Next))) % 3, this.ChordId));
            Raise(new Local());
        }
        void OnSuccesssorRespFix()
        { 
            finger[Next] = (ReceivedEvent as FindSuccessorResp).Succ;
            Next = Next + 1;
            Raise(new Local());
        }

        bool Belongs(int id, int start, int end)
        {
            if (id >= start && id <= end)
                return true;
            if (start > end)
            {
                if (id >= start)
                    return true;
                if (id >= 0 && id <= end)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
