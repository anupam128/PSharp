//-----------------------------------------------------------------------
// <copyright file="IPSharpRuntime.cs">
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
using System.Threading.Tasks;

namespace Microsoft.PSharp
{
	/// <summary>
	/// The P# runtime interface.
	/// </summary>
	public interface IPSharpRuntime
	{
		#region methods

		/// <summary>
		/// Creates a new machine of the specified type and with
		/// the specified optional event. This event can only be
		/// used to access its payload, and cannot be handled.
		/// </summary>
		/// <param name="type">Type of the machine</param>
		/// <param name="e">Event</param>
		/// <returns>MachineId</returns>
		Task<MachineId> CreateMachineAsync(Type type, Event e = null);

		/// <summary>
		/// Creates a new machine of the specified type and name, and
		/// with the specified optional event. This event can only be
		/// used to access its payload, and cannot be handled.
		/// </summary>
		/// <param name="type">Type of the machine</param>
		/// <param name="friendlyName">Friendly machine name used for logging</param>
		/// <param name="e">Event</param>
		/// <returns>MachineId</returns>
		Task<MachineId> CreateMachineAsync(Type type, string friendlyName, Event e = null);

		/// <summary>
		/// Creates a new remote machine of the specified type and with
		/// the specified optional event. This event can only be used
		/// to access its payload, and cannot be handled.
		/// </summary>
		/// <param name="type">Type of the machine</param>
		/// <param name="endpoint">Endpoint</param>
		/// <param name="e">Event</param>
		/// <returns>MachineId</returns>
		Task<MachineId> RemoteCreateMachineAsync(Type type, string endpoint, Event e = null);

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
		Task<MachineId> RemoteCreateMachineAsync(Type type, string friendlyName, string endpoint, Event e = null);

		/// <summary>
		/// Gets the id of the currently executing machine. Returns null if none.
		/// <returns>MachineId</returns>
		/// </summary>
		MachineId GetCurrentMachineId();

		/// <summary>
		/// Sends an asynchronous event to a machine.
		/// </summary>
		/// <param name="target">Target machine id</param>
		/// <param name="e">Event</param>
		Task SendEventAsync(MachineId target, Event e);

		/// <summary>
		/// Sends an asynchronous event to a remote machine.
		/// </summary>
		/// <param name="target">Target machine id</param>
		/// <param name="e">Event</param>
		Task RemoteSendEventAsync(MachineId target, Event e);

		/// <summary>
		/// Blocks and waits to receive an event of the specified types.
		/// Returns the received event.
		/// </summary>
		/// <param name="eventTypes">Event types</param>
		/// <returns>Received event</returns>
		Task<Event> ReceiveEventAsync(params Type[] eventTypes);

		/// <summary>
		/// Blocks and waits to receive an event of the specified types that satisfies
		/// the specified predicate. Returns the received event.
		/// </summary>
		/// <param name="eventType">Event type</param>
		/// <param name="predicate">Predicate</param>
		/// <returns>Received event</returns>
		Task<Event> ReceiveEventAsync(Type eventType, Func<Event, bool> predicate);

		/// <summary>
		/// Blocks and waits to receive an event of the specified types that satisfy
		/// the specified predicates. Returns the received event.
		/// </summary>
		/// <param name="events">Event types and predicates</param>
		/// <returns>Received event</returns>
		Task<Event> ReceiveEventAsync(params Tuple<Type, Func<Event, bool>>[] events);

		/// <summary>
		/// Registers a new specification monitor of the specified type.
		/// </summary>
		/// <param name="type">Type of the monitor</param>
		Task RegisterMonitorAsync(Type type);

		/// <summary>
		/// Invokes the specified monitor with the specified event.
		/// </summary>
		/// <typeparam name="T">Type of the monitor</typeparam>
		/// <param name="e">Event</param>
		Task InvokeMonitorAsync<T>(Event e);

		/// <summary>
		/// Returns a nondeterministic boolean choice, that can be controlled
		/// during analysis or testing.
		/// </summary>
		/// <returns>Boolean</returns>
		bool Random();

		/// <summary>
		/// Returns a nondeterministic boolean choice, that can be controlled
		/// during analysis or testing. The value is used to generate a number
		/// in the range [0..maxValue), where 0 triggers true.
		/// </summary>
		/// <param name="maxValue">Max value</param>
		/// <returns>Boolean</returns>
		bool Random(int maxValue);

		/// <summary>
		/// Returns a nondeterministic integer choice, that can be
		/// controlled during analysis or testing. The value is used
		/// to generate an integer in the range [0..maxValue).
		/// </summary>
		/// <param name="maxValue">Max value</param>
		/// <returns>Integer</returns>
		int RandomInteger(int maxValue);

		/// <summary>
		/// Checks if the assertion holds, and if not it reports
		/// an error and exits.
		/// </summary>
		/// <param name="predicate">Predicate</param>
		void Assert(bool predicate);

		/// <summary>
		/// Checks if the assertion holds, and if not it reports
		/// an error and exits.
		/// </summary>
		/// <param name="predicate">Predicate</param>
		/// <param name="s">Message</param>
		/// <param name="args">Message arguments</param>
		void Assert(bool predicate, string s, params object[] args);

		/// <summary>
		/// Logs the specified text.
		/// </summary>
		/// <param name="s">String</param>
		/// <param name="args">Arguments</param>
		void Log(string s, params object[] args);

		/// <summary>
		/// Waits until all P# machines have finished execution.
		/// </summary>
		void Wait();

		#endregion
	}
}
