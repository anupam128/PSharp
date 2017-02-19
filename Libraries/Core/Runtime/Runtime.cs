//-----------------------------------------------------------------------
// <copyright file="Runtime.cs">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//      IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
//      CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//      TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
//      SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.PSharp.Collections;
using Microsoft.PSharp.Net;
using Microsoft.PSharp.Utilities;

namespace Microsoft.PSharp
{
    /// <summary>
    /// Class implementing the production P# runtime.
    /// </summary>
	internal class Runtime : IPSharpRuntime
    {
        #region fields

        /// <summary>
        /// The configuration.
        /// </summary>
        internal Configuration Configuration;

        /// <summary>
        /// Map from unique machine ids to machines.
        /// </summary>
		protected AsyncDictionary<int, Machine> MachineMap;

		/// <summary>
        /// List of monitors in the program.
        /// </summary>
        protected AsyncList<Monitor> Monitors;

		///// <summary>
		///// Map from task ids to machines.
		///// </summary>
		//protected ConcurrentDictionary<int, Machine> TaskMap;

		/// <summary>
		/// Network provider for remote communication.
		/// </summary>
		internal INetworkProvider NetworkProvider;

        #endregion

        #region public methods

        /// <summary>
        /// Creates a new machine of the specified type and with
        /// the specified optional event. This event can only be
        /// used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine</param>
        /// <param name="e">Event</param>
        /// <returns>MachineId</returns>
        public virtual async Task<MachineId> CreateMachineAsync(Type type, Event e = null)
        {
			return await this.TryCreateMachine(null, type, null, e);
        }

        /// <summary>
        /// Creates a new machine of the specified type and name, and
        /// with the specified optional event. This event can only be
        /// used to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine</param>
        /// <param name="friendlyName">Friendly machine name used for logging</param>
        /// <param name="e">Event</param>
        /// <returns>MachineId</returns>
        public virtual async Task<MachineId> CreateMachineAsync(Type type, string friendlyName, Event e = null)
        {
            return await this.TryCreateMachine(null, type, friendlyName, e);
        }

        /// <summary>
        /// Creates a new remote machine of the specified type and with
        /// the specified optional event. This event can only be used
        /// to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine</param>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="e">Event</param>
        /// <returns>MachineId</returns>
        public virtual async Task<MachineId> RemoteCreateMachineAsync(Type type, string endpoint, Event e = null)
        {
            return await this.TryCreateRemoteMachine(null, type, null, endpoint, e);
        }

        /// <summary>
        /// Creates a new remote machine of the specified type and name, and
        /// with the specified optional event. This event can only be used
        /// to access its payload, and cannot be handled.
        /// </summary>
        /// <param name="type">Type of the machine</param>
        /// <param name="friendlyName">Friendly machine name used for logging</param>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="e">Event</param>
        /// <returns>MachineId</returns>
        public virtual async Task<MachineId> RemoteCreateMachineAsync(Type type, string friendlyName,
            string endpoint, Event e = null)
        {
            return await this.TryCreateRemoteMachine(null, type, friendlyName, endpoint, e);
        }

        /// <summary>
        /// Gets the id of the currently executing machine. Returns null if none.
        /// <returns>MachineId</returns>
        /// </summary>
        public virtual MachineId GetCurrentMachineId()
        {
			throw new NotImplementedException();
            //if(Task.CurrentId == null || !this.TaskMap.ContainsKey((int) Task.CurrentId))
            //{
            //    return null;
            //}
            //Machine machine = this.TaskMap[(int)Task.CurrentId];
            //return machine.Id;
        }

        /// <summary>
        /// Sends an asynchronous event to a machine.
        /// </summary>
        /// <param name="target">Target machine id</param>
        /// <param name="e">Event</param>
        public virtual async Task SendEventAsync(MachineId target, Event e)
        {
            // If the target machine is null then report an error and exit.
            this.Assert(target != null, "Cannot send to a null machine.");
            // If the event is null then report an error and exit.
            this.Assert(e != null, "Cannot send a null event.");
			await this.Send(null, target, e);
        }

        /// <summary>
        /// Sends an asynchronous event to a remote machine.
        /// </summary>
        /// <param name="target">Target machine id</param>
        /// <param name="e">Event</param>
        public virtual async Task RemoteSendEventAsync(MachineId target, Event e)
        {
            // If the target machine is null then report an error and exit.
            this.Assert(target != null, "Cannot send to a null machine.");
            // If the event is null then report an error and exit.
            this.Assert(e != null, "Cannot send a null event.");
            await this.SendRemotely(null, target, e);
        }

        /// <summary>
        /// Blocks and waits to receive an event of the specified types.
        /// Returns the received event.
        /// </summary>
        /// <param name="eventTypes">Event types</param>
        /// <returns>Received event</returns>
        public virtual async Task<Event> ReceiveEventAsync(params Type[] eventTypes)
        {
			throw new NotImplementedException();
            //this.Assert(Task.CurrentId != null, "Only machines can " +
            //    "wait to receive an event.");
            //this.Assert(this.TaskMap.ContainsKey((int)Task.CurrentId),
            //    "Only machines can wait to receive an event; task " +
            //    $"{(int)Task.CurrentId} does not correspond to a machine.");

            //Machine machine = this.TaskMap[(int)Task.CurrentId];
            //return await machine.Receive(eventTypes);
        }

        /// <summary>
        /// Blocks and waits to receive an event of the specified types that satisfies
        /// the specified predicate. Returns the received event.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="predicate">Predicate</param>
        /// <returns>Received event</returns>
        public virtual async Task<Event> ReceiveEventAsync(Type eventType, Func<Event, bool> predicate)
        {
			throw new NotImplementedException();
            //this.Assert(Task.CurrentId != null, "Only machines can " +
            //    "wait to receive an event.");
            //this.Assert(this.TaskMap.ContainsKey((int)Task.CurrentId),
            //    "Only machines can wait to receive an event; task " +
            //    $"{(int)Task.CurrentId} does not belong to a machine.");

            //Machine machine = this.TaskMap[(int)Task.CurrentId];
            //return await machine.Receive(eventType, predicate);
        }

        /// <summary>
        /// Blocks and waits to receive an event of the specified types that satisfy
        /// the specified predicates. Returns the received event.
        /// </summary>
        /// <param name="events">Event types and predicates</param>
        /// <returns>Received event</returns>
        public virtual async Task<Event> ReceiveEventAsync(params Tuple<Type, Func<Event, bool>>[] events)
        {
			throw new NotImplementedException();
            //this.Assert(Task.CurrentId != null, "Only machines can " +
            //    "wait to receive an event.");
            //this.Assert(this.TaskMap.ContainsKey((int)Task.CurrentId),
            //    "Only machines can wait to receive an event; task " +
            //    $"{(int)Task.CurrentId} does not belong to a machine.");

            //Machine machine = this.TaskMap[(int)Task.CurrentId];
            //return await machine.Receive(events);
        }

        /// <summary>
        /// Registers a new specification monitor of the specified type.
        /// </summary>
        /// <param name="type">Type of the monitor</param>
		public virtual async Task RegisterMonitorAsync(Type type)
        {
            await this.TryCreateMonitor(type);
        }

        /// <summary>
        /// Invokes the specified monitor with the specified event.
        /// </summary>
        /// <typeparam name="T">Type of the monitor</typeparam>
        /// <param name="e">Event</param>
		public virtual async Task InvokeMonitorAsync<T>(Event e)
        {
            // If the event is null then report an error and exit.
            this.Assert(e != null, "Cannot monitor a null event.");
            await this.Monitor<T>(null, e);
        }

        /// <summary>
        /// Returns a nondeterministic boolean choice, that can be controlled
        /// during analysis or testing.
        /// </summary>
        /// <returns>Boolean</returns>
        public virtual bool Random()
        {
            return this.GetNondeterministicBooleanChoice(null, 2);
        }

        /// <summary>
        /// Returns a nondeterministic boolean choice, that can be controlled
        /// during analysis or testing. The value is used to generate a number
        /// in the range [0..maxValue), where 0 triggers true.
        /// </summary>
        /// <param name="maxValue">Max value</param>
        /// <returns>Boolean</returns>
        public virtual bool Random(int maxValue)
        {
            return this.GetNondeterministicBooleanChoice(null, maxValue);
        }

        /// <summary>
        /// Returns a nondeterministic integer choice, that can be
        /// controlled during analysis or testing. The value is used
        /// to generate an integer in the range [0..maxValue).
        /// </summary>
        /// <param name="maxValue">Max value</param>
        /// <returns>Integer</returns>
        public virtual int RandomInteger(int maxValue)
        {
            return this.GetNondeterministicIntegerChoice(null, maxValue);
        }

        /// <summary>
        /// Checks if the assertion holds, and if not it reports
        /// an error and exits.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        public virtual void Assert(bool predicate)
        {
            if (!predicate)
            {
                ErrorReporter.Report("Assertion failure.");

                if (this.Configuration.PauseOnAssertionFailure)
                {
                    IO.GetLine();
                }

                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks if the assertion holds, and if not it reports
        /// an error and exits.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="s">Message</param>
        /// <param name="args">Message arguments</param>
        public virtual void Assert(bool predicate, string s, params object[] args)
        {
            if (!predicate)
            {
                string message = IO.Format(s, args);
                ErrorReporter.Report(message);

                if (this.Configuration.PauseOnAssertionFailure)
                {
                    IO.GetLine();
                }

                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="s">String</param>
        /// <param name="args">Arguments</param>
        public virtual void Log(string s, params object[] args)
        {
            if (this.Configuration.Verbose > 1)
            {
                IO.Log(s, args);
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
		internal Runtime()
        {
            this.Configuration = Configuration.Create();
            this.NetworkProvider = new DefaultNetworkProvider(this);
            this.Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="netProvider">NetworkProvider</param>
        internal Runtime(INetworkProvider netProvider)
        {
            this.Configuration = Configuration.Create();
            this.NetworkProvider = netProvider;
            this.Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        internal Runtime(Configuration configuration)
        {
            this.Configuration = configuration;
            this.NetworkProvider = new DefaultNetworkProvider(this);
            this.Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="netProvider">NetworkProvider</param>
        internal Runtime(Configuration configuration, INetworkProvider netProvider)
        {
            this.Configuration = configuration;
            this.NetworkProvider = netProvider;
            this.Initialize();
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Gets the currently executing machine.
        /// </summary>
        /// <returns>Machine or null, if not present</returns>
        internal virtual Machine GetCurrentMachine()
        {
			throw new NotImplementedException();
            ////  The current task does not correspond to a machine.
            //if (Task.CurrentId == null)
            //{
            //    return null;
            //}

            //// The current task does not correspond to a machine.
            //if (!this.TaskMap.ContainsKey((int)Task.CurrentId))
            //{
            //    return null;
            //}

            //return this.TaskMap[(int)Task.CurrentId];
        }

        /// <summary>
        /// Tries to create a new machine of the specified type.
        /// </summary>
        /// <param name="creator">Creator machine</param>
        /// <param name="type">Type of the machine</param>
        /// <param name="friendlyName">Friendly machine name used for logging</param>
        /// <param name="e">Event</param>
        /// <returns>MachineId</returns>
        internal virtual async Task<MachineId> TryCreateMachine(Machine creator, Type type,
            string friendlyName, Event e)
        {
            this.Assert(type.IsSubclassOf(typeof(Machine)),
                $"Type '{type.Name}' is not a machine.");
            
            MachineId mid = new MachineId(type, friendlyName, this);
			Machine machine = await MachineFactory.Create(type);

            machine.SetMachineId(mid);
            machine.InitializeStateInformation();

			await this.MachineMap.Add(mid.Value, machine);

            this.Log($"<CreateLog> Machine '{mid}' is created.");

			this.RunMachineEventHandler(machine, e, true);

            return mid;
        }

        /// <summary>
        /// Tries to create a new remote machine of the specified type.
        /// </summary>
        /// <param name="creator">Creator machine</param>
        /// <param name="type">Type of the machine</param>
        /// <param name="friendlyName">Friendly machine name used for logging</param>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="e">Event</param>
        /// <returns>MachineId</returns>
        internal virtual async Task<MachineId> TryCreateRemoteMachine(Machine creator, Type type,
            string friendlyName, string endpoint, Event e)
        {
            this.Assert(type.IsSubclassOf(typeof(Machine)),
                $"Type '{type.Name}' is not a machine.");
            return await this.NetworkProvider.RemoteCreateMachineAsync(type, friendlyName, endpoint, e);
        }

        /// <summary>
        /// Tries to create a new monitor of the specified type.
        /// </summary>
        /// <param name="type">Type of the monitor</param>
        internal virtual async Task TryCreateMonitor(Type type)
        {
            if (!this.Configuration.EnableMonitorsInProduction)
            {
                // No-op in production.
                return;
            }

            this.Assert(type.IsSubclassOf(typeof(Monitor)), $"Type '{type.Name}' " +
                "is not a subclass of Monitor.\n");

            MachineId mid = new MachineId(type, null, this);
            Object monitor = Activator.CreateInstance(type);
            (monitor as Monitor).SetMachineId(mid);
            (monitor as Monitor).InitializeStateInformation();

			await this.Monitors.Add(monitor as Monitor);

            this.Log($"<CreateLog> Monitor '{type.Name}' is created.");

            (monitor as Monitor).GotoStartState();
        }

        /// <summary>
        /// Tries to create a new task machine.
        /// </summary>
        /// <param name="userTask">Task</param>
        internal virtual void TryCreateTaskMachine(Task userTask)
        {
            // No-op in production.
        }

        /// <summary>
        /// Sends an asynchronous event to a machine.
        /// </summary>
        /// <param name="sender">Sender machine</param>
        /// <param name="mid">MachineId</param>
        /// <param name="e">Event</param>
        internal virtual async Task Send(AbstractMachine sender, MachineId mid, Event e)
        {
            EventInfo eventInfo = new EventInfo(e, null);

            Machine machine = null;
			var result = await this.MachineMap.TryGetValue(mid.Value);
			if (!result.Item1)
            {
                return;
            }

			machine = result.Item2;

            if (sender != null)
            {
                this.Log($"<SendLog> Machine '{sender.Id}' sent event " +
                    $"'{eventInfo.EventName}' to '{mid}'.");
            }
            else
            {
                this.Log($"<SendLog> Event '{eventInfo.EventName}' was sent to '{mid}'.");
            }

            bool runHandler = await machine.Enqueue(eventInfo);
            if (runHandler)
            {
                this.RunMachineEventHandler(machine);
            }
        }

        /// <summary>
        /// Sends an asynchronous event to a remote machine.
        /// </summary>
        /// <param name="sender">Sender machine</param>
        /// <param name="mid">MachineId</param>
        /// <param name="e">Event</param>
        internal virtual async Task SendRemotely(AbstractMachine sender, MachineId mid, Event e)
        {
            await this.NetworkProvider.RemoteSendEventAsync(mid, e);
        }

        /// <summary>
        /// Invokes the specified monitor with the specified event.
        /// </summary>
        /// <param name="sender">Sender machine</param>
        /// <typeparam name="T">Type of the monitor</typeparam>
        /// <param name="e">Event</param>
        internal virtual async Task Monitor<T>(AbstractMachine sender, Event e)
        {
            if (!this.Configuration.EnableMonitorsInProduction)
            {
                // No-op in production.
                return;
            }

			Monitor monitor = await this.Monitors.Get(m => m.GetType() == typeof(T));
            if (monitor != null)
            {
				await monitor.MonitorEvent(e);
            }
        }

        /// <summary>
        /// Returns a nondeterministic boolean choice, that can be
        /// controlled during analysis or testing.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="maxValue">Max value</param>
        /// <returns>Boolean</returns>
        internal virtual bool GetNondeterministicBooleanChoice(
            AbstractMachine machine, int maxValue)
        {
            Random random = new Random(DateTime.Now.Millisecond);

            bool result = false;
            if (random.Next(maxValue) == 0)
            {
                result = true;
            }

            if (machine != null)
            {
                this.Log($"<RandomLog> Machine '{machine.Id}' " +
                    $"nondeterministically chose '{result}'.");
            }
            else
            {
                this.Log($"<RandomLog> Runtime nondeterministically chose '{result}'.");
            }

            return result;
        }

        /// <summary>
        /// Returns a fair nondeterministic boolean choice, that can be
        /// controlled during analysis or testing.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="uniqueId">Unique id</param>
        /// <returns>Boolean</returns>
        internal virtual bool GetFairNondeterministicBooleanChoice(
            AbstractMachine machine, string uniqueId)
        {
            return this.GetNondeterministicBooleanChoice(machine, 2);
        }

        /// <summary>
        /// Returns a nondeterministic integer choice, that can be
        /// controlled during analysis or testing.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="maxValue">Max value</param>
        /// <returns>Integer</returns>
        internal virtual int GetNondeterministicIntegerChoice(
            AbstractMachine machine, int maxValue)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            var result = random.Next(maxValue);

            if (machine != null)
            {
                this.Log($"<RandomLog> Machine '{machine.Id}' " +
                    $"nondeterministically chose '{result}'.");
            }
            else
            {
                this.Log($"<RandomLog> Runtime nondeterministically chose '{result}'.");
            }

            return result;
        }

        /// <summary>
        /// Notifies that a machine entered a state.
        /// </summary>
        /// <param name="machine">AbstractMachine</param>
        internal virtual void NotifyEnteredState(AbstractMachine machine)
        {
            // No-op in production, except for logging.
            if (this.Configuration.Verbose <= 1)
            {
                return;
            }

            if (machine is Machine)
            {
                string machineState = (machine as Machine).CurrentStateName;

                this.Log($"<StateLog> Machine '{machine.Id}' enters " +
                    $"state '{machineState}'.");
            }
            else if (machine is Monitor)
            {
                string liveness = "";
                string monitorState = (machine as Monitor).CurrentStateNameWithTemperature;

                if ((machine as Monitor).IsInHotState())
                {
                    liveness = "'hot' ";
                }
                else if ((machine as Monitor).IsInColdState())
                {
                    liveness = "'cold' ";
                }

                this.Log($"<MonitorLog> Monitor '{machine.GetType().Name}' " +
                    $"enters {liveness}state '{monitorState}'.");
            }

        }

        /// <summary>
        /// Notifies that a machine exited a state.
        /// </summary>
        /// <param name="machine">AbstractMachine</param>
        internal virtual void NotifyExitedState(AbstractMachine machine)
        {
            // No-op in production, except for logging.
            if (this.Configuration.Verbose <= 1)
            {
                return;
            }

            if (machine is Machine)
            {
                string machineState = (machine as Machine).CurrentStateName;

                this.Log($"<StateLog> Machine '{machine.Id}' exits " +
                    $"state '{machineState}'.");
            }
            else if (machine is Monitor)
            {
                string liveness = "";
                string monitorState = (machine as Monitor).CurrentStateName;

                if ((machine as Monitor).IsInHotState())
                {
                    liveness = "'hot' ";
                    monitorState += "[hot]";
                }
                else if ((machine as Monitor).IsInColdState())
                {
                    liveness = "'cold' ";
                    monitorState += "[cold]";
                }

                this.Log($"<MonitorLog> Monitor '{machine.GetType().Name}' " +
                    $"exits {liveness}state '{monitorState}'.");
            }

        }

        /// <summary>
        /// Notifies that a machine invoked an action.
        /// </summary>
        /// <param name="machine">AbstractMachine</param>
        /// <param name="action">Action</param>
        /// <param name="receivedEvent">Event</param>
        internal virtual void NotifyInvokedAction(AbstractMachine machine, MethodInfo action, Event receivedEvent)
        {
            // No-op in production, except for logging.
            if (this.Configuration.Verbose <= 1)
            {
                return;
            }

            if (machine is Machine)
            {
                string machineState = (machine as Machine).CurrentStateName;

                this.Log($"<ActionLog> Machine '{machine.Id}' invoked action " +
                    $"'{action.Name}' in state '{machineState}'.");
            }
            else if (machine is Monitor)
            {
                string monitorState = (machine as Monitor).CurrentStateName;

                this.Log($"<MonitorLog> Monitor '{machine.GetType().Name}' executed " +
                    $"action '{action.Name}' in state '{monitorState}'.");
            }

        }

        /// <summary>
        /// Notifies that a machine dequeued an event.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal virtual void NotifyDequeuedEvent(Machine machine, EventInfo eventInfo)
        {
            // No-op in production, except for logging.
            this.Log($"<DequeueLog> Machine '{machine.Id}' dequeued " +
                $"event '{eventInfo.EventName}'.");
        }

        /// <summary>
        /// Notifies that a machine called Pop.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="fromState">Top of the stack state</param>
        /// <param name="toState">Next to top state of the stack</param>
        internal virtual void NotifyPop(Machine machine, Type fromState, Type toState)
        {
            // No-op in production.
        }

        /// <summary>
        /// Notifies that a machine raised an event.
        /// </summary>
        /// <param name="machine">AbstractMachine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal virtual void NotifyRaisedEvent(AbstractMachine machine, EventInfo eventInfo)
        {
            // No-op in production, except for logging.
            if (this.Configuration.Verbose <= 1)
            {
                return;
            }

            if (machine is Machine)
            {
                string machineState = (machine as Machine).CurrentStateName;
                this.Log($"<RaiseLog> Machine '{machine.Id}' raised " +
                    $"event '{eventInfo.EventName}'.");
            }
            else if (machine is Monitor)
            {
                string monitorState = (machine as Monitor).CurrentStateName;

                this.Log($"<MonitorLog> Monitor '{machine.GetType().Name}' raised " +
                    $"event '{eventInfo.EventName}'.");
            }

        }

        /// <summary>
        /// Notifies that a machine called Receive.
        /// </summary>
        /// <param name="machine">AbstractMachine</param>
        internal virtual void NotifyReceiveCalled(AbstractMachine machine)
        {
            // No-op in production.
        }

        /// <summary>
        /// Notifies that a machine handles a raised event.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal virtual void NotifyHandleRaisedEvent(Machine machine, EventInfo eventInfo)
        {
            // No-op in production.
        }

        /// <summary>
        /// Notifies that a machine is waiting to receive one
        /// or more events.
        /// </summary>
        /// <param name="machine">Machine</param>
        internal virtual void NotifyWaitEvents(Machine machine)
        {
            this.Log($"<ReceiveLog> Machine '{machine.Id}' is waiting to receive an event.");
        }

        /// <summary>
        /// Notifies that a machine received an event that it was waiting for.
        /// </summary>
        /// <param name="machine">Machine</param>
        /// <param name="eventInfo">EventInfo</param>
        internal virtual void NotifyReceivedEvent(Machine machine, EventInfo eventInfo)
        {
            this.Log($"<ReceiveLog> Machine '{machine.Id}' received " +
                $"event '{eventInfo.EventName}' and unblocked.");
        }

        /// <summary>
        /// Notifies that a machine has halted.
        /// </summary>
        /// <param name="machine">Machine</param>
        internal virtual async Task NotifyHalted(Machine machine)
        {
            this.Log($"<HaltLog> Machine '{machine.Id}' halted.");
			await this.MachineMap.Remove(machine.Id.Value);
        }

        /// <summary>
        /// Notifies that a default handler has been used.
        /// </summary>
        internal virtual void NotifyDefaultHandlerFired()
        {
            // No-op in production.
        }

        #endregion

        #region private methods

        /// <summary>
        /// Initializes various components of the runtime.
        /// </summary>
        private void Initialize()
        {
            this.MachineMap = new AsyncDictionary<int, Machine>();
            //this.TaskMap = new ConcurrentDictionary<int, Machine>();
            this.Monitors = new AsyncList<Monitor>();
        }

        /// <summary>
		/// Runs a new asynchronous machine event handler.
		/// This is a fire and forget invocation.
		/// </summary>
		/// <param name="machine">Machine</param>
		/// <param name="e">Event</param>
		/// <param name="isFresh">Is a new machine</param>
		private void RunMachineEventHandler(Machine machine, Event e = null, bool isFresh = false)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (isFresh)
                    {
                        await machine.GotoStartState(e);
                    }

                    await machine.RunEventHandler();
                }
                catch (Exception ex)
                {
                    // TODO: DEBUG
                    Console.WriteLine("<<<<<<<<<< EXCEPTION in RunMachineEventHandler >>>>>>>>>>");
                    Console.WriteLine(ex);
                    if (this.Configuration.ThrowInternalExceptions)
                    {
                        throw ex;
                    }
                }
                finally
                {
                    //this.TaskMap.TryRemove(Task.CurrentId.Value, out machine);
                }
            });

            //this.TaskMap.TryAdd(task.Id, machine);
        }

        #endregion
    }
}
