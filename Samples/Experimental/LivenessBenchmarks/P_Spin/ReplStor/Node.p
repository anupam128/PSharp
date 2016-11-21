event Node_Update : machine;
event Node_Config : machine;
event Node_Fail;

event LivenessMonitor_Configure : int;
event LivenessMonitor_NodeUpdated;
event LivenessMonitor_NodeFailed;

machine Node
{
	var EnvTarget : machine;

	start state Init
	{	
		on Node_Config do (Target : machine)
		{
			EnvTarget = Target;
            send this, Local;
		}
		on Local do
		{
			if ($)
            {
                send EnvTarget, Node_Update, this;
            }
            send this, Local;
		}
		on Repaired do
		{
			announce LivenessMonitor_NodeUpdated;
		}
		on Node_Fail do
		{
			announce LivenessMonitor_NodeFailed;
		}
	}
}

spec Liveness observes LivenessMonitor_NodeFailed, LivenessMonitor_NodeUpdated, LivenessMonitor_Configure
{
	var NumberOfNodes : int;
    var FailedNodes : int;

	start state Init
	{
		on LivenessMonitor_Configure do (NumNodes : int)
		{
			NumberOfNodes = NumNodes;
            FailedNodes = 0;
            raise Local;
		}
		on Local goto Repaired;
	}

	cold state Repaired
	{	
		on LivenessMonitor_NodeFailed goto Repairing;
		on LivenessMonitor_NodeUpdated goto Repaired;
	}

	hot state Repairing
	{
		entry
		{
			FailedNodes = FailedNodes + 1;
		}
		on LivenessMonitor_NodeFailed do 
		{
			FailedNodes = FailedNodes + 1;
		}
		on LivenessMonitor_NodeUpdated do
		{
			FailedNodes = FailedNodes - 1;
            if(FailedNodes == 0)
            {
                raise Local;
            }
		}
		on Local goto Repaired;
	}
}