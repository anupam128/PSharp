event Config : machine;
event FailureTimer_Timeout;
event FailureTimer_StartTimer;
event FailureTimer_TickEvent;

machine Main
{
	var FailMachine : machine;
	
	start state init
	{
		entry
		{
			FailMachine = new FailureMachine();
			send FailMachine, Config, this;
		}
		on FailureTimer_Timeout do
		{
			assert false;
		}
	}	
}

machine FailureMachine
{
	var Target : machine;

	start state Init
	{
		on Config do (target : machine)
		{
			Target = target;
            raise FailureTimer_StartTimer;
		}
		on FailureTimer_StartTimer goto Active;
	}

	state Active
	{
		entry
		{
			send this, FailureTimer_TickEvent;
		}
		on FailureTimer_TickEvent do
		{
			if ($)
            {
                send Target, FailureTimer_Timeout;
            }

            send this, FailureTimer_TickEvent;
		}

	}
}