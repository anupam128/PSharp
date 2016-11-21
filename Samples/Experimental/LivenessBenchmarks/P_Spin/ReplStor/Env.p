#include "Node.p"

event Local;
event Repaired;
event RepairNodes;
event Local1;

machine Main
{
	var Nodes : seq[machine];
	var FailedNodes : seq[machine];

	start state Init
	{
		entry
		{
			var node : machine;

			Nodes = default(seq[machine]);
            FailedNodes = default(seq[machine]);

			node = new Node();
			send node, Node_Config, this;
			Nodes += (sizeof(Nodes), node);
			/*node = new Node();
			send node, Node_Config, this;
			Nodes += (sizeof(Nodes), node); 
			*/
            announce LivenessMonitor_Configure, 1;
            raise Local;
		}
		on Local goto Waiting;
	}

	state Waiting
	{
		entry
		{
			var fNode : machine;
			var index : int;

			if ($)
            {
				/*if($)
				{
					index = 0;
				}
				else
				{
					index = 1;
				}
                
				fNode = Nodes[index];
                send fNode, Node_Fail;
                FailedNodes += (sizeof(FailedNodes), fNode);*/

				fNode = Nodes[0];
                send fNode, Node_Fail;
                FailedNodes += (sizeof(FailedNodes), fNode);
            }
            send this, Local1;
		}
		on Node_Update do (NodeId : machine)
		{
			var nodeId : machine;
			var index : int;
			var removeId : int;

			nodeId = NodeId;

			index = 0;
			removeId = -1;
			while (index < sizeof(FailedNodes))
			{
				if(FailedNodes[index] == nodeId)
				{
					removeId = index;
				}
				index = index + 1;
			}

			if(removeId >= 0)
			{
				FailedNodes -= removeId;
			}
		}
		on RepairNodes do
		{
			var index : int;

			index = 0;
			while(index < sizeof(FailedNodes))
			{
				send FailedNodes[index], Repaired;
				index = index + 1;
			}
		}
		on Local1 do
		{	
			if ($)
            {
                send this, RepairNodes;
            }
            //send this, Local1;
		}
	}
}