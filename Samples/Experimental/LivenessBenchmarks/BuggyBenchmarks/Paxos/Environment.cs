using Microsoft.PSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paxos
{
    class Environment : Machine
    {
        #region states
        [Start]
        [OnEntry(nameof(InitOnEntry))]
        class Init : MachineState { }
        #endregion

        #region fields
        List<MachineId> proposers;
        List<MachineId> acceptors;
        MachineId temp;
        int index;
        #endregion

        #region actions
        void InitOnEntry()
        {
            index = 0;
            acceptors = new List<MachineId>();
            //create acceptors
            while (index < 3)
            {
                temp = CreateMachine(typeof(AcceptorMachine));
                acceptors.Add(temp);
                index = index + 1;
            }
            //create proposers
            index = 0;
            proposers = new List<MachineId>();
            while (index < 2)
            {
                temp = CreateMachine(typeof(ProposerMachine), new ProposerMachine.Config(acceptors, index + 1));
                proposers.Add(temp);
                index = index + 1;
            }

            Raise(new Halt());
        }
        #endregion
    }
}