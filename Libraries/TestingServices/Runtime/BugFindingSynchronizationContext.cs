//-----------------------------------------------------------------------
// <copyright file="BugFindingSynchronizationContext.cs">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.PSharp.Utilities;

namespace Microsoft.PSharp.TestingServices.Threading
{
    /// <summary>
    /// The P# bug-finding synchronization context.
    /// </summary>
    internal sealed class BugFindingSynchronizationContext : SynchronizationContext
    {
        #region fields

        /// <summary>
        /// The P# bug-finding runtime.
        /// </summary>
        private BugFindingRuntime Runtime;

        /// <summary>
        /// Collection of machine tasks.
        /// </summary>
        private ConcurrentBag<Task> MachineTasks;

        /// <summary>
        /// Map from task ids to machines.
        /// </summary>
        private ConcurrentDictionary<int, Machine> TaskMap;

        /// <summary>
        /// The wrapped in a machine user tasks.
        /// </summary>
        private ConcurrentBag<Task> WrappedTasks;

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="runtime">BugFindingRuntime</param>
        /// <param name="machineTasks">Machine tasks</param>
        /// <param name="taskMap">Task map</param>
        internal BugFindingSynchronizationContext(BugFindingRuntime runtime, ConcurrentBag<Task> machineTasks,
            ConcurrentDictionary<int, Machine> taskMap)
        {
            this.Runtime = runtime;
            this.MachineTasks = machineTasks;
            this.TaskMap = taskMap;
            this.WrappedTasks = new ConcurrentBag<Task>();
        }

        #endregion

        #region override methods

        /// <summary>
        /// Dispatches an asynchronous message.
        /// </summary>
        /// <param name="d">SendOrPostCallback</param>
        /// <param name="state">State</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            Console.WriteLine("POST: " + Runtime.BugFinder.ScheduledMachineInfo);
            Console.WriteLine("POST: " + Runtime.BugFinder.ScheduledMachineInfo.Id);
            Console.WriteLine("POST: " + this.TaskMap.ContainsKey(Runtime.BugFinder.ScheduledMachineInfo.Id));
            Machine parent = this.TaskMap[Runtime.BugFinder.ScheduledMachineInfo.Id];
            Console.WriteLine("Parent: " + parent.Id);

            //Task task = new Task(() =>
            //{
            //    try
            //    {
            //        this.Runtime.BugFinder.NotifyTaskStarted();

            //        d(state);

            //        this.Runtime.BugFinder.NotifyTaskCompleted();
            //    }
            //    catch (MachineCanceledException)
            //    {
            //        IO.Debug("<Exception> MachineCanceledException was " +
            //            $"thrown from Machine '{parent.Id}'.");
            //    }
            //    finally
            //    {
            //        this.TaskMap.TryRemove(Task.CurrentId.Value, out parent);
            //    }
            //});

            //this.MachineTasks.Add(task);
            //this.TaskMap.TryAdd(task.Id, parent);

            //this.Runtime.BugFinder.NotifyNewTaskCreated(task.Id, parent);

            //if (this.Runtime.Configuration.ScheduleIntraMachineConcurrency)
            //{
            //    task.Start(this.Runtime.TaskScheduler);
            //}
            //else
            //{
            //    task.Start();
            //}

            //this.Runtime.BugFinder.WaitForTaskToStart(task.Id);
        }

        /// <summary>
        /// Dispatches a synchronous message.
        /// </summary>
        /// <param name="d">SendOrPostCallback</param>
        /// <param name="state">State</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            Console.WriteLine("SEND: " + Runtime.BugFinder.ScheduledMachineInfo);
        }

        #endregion
    }
}
