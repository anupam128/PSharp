//-----------------------------------------------------------------------
// <copyright file="FairTaskScheduler.cs">
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

namespace Microsoft.PSharp.Threading.Scheduling
{
    /// <summary>
    /// Class implementing a fair task scheduler for P#.
    /// </summary>
    internal sealed class FairTaskScheduler : TaskScheduler
    {
        #region fields

        /// <summary>
        /// The P# runtime.
        /// </summary>
        private PSharpRuntime Runtime;

        /// <summary>
        /// Map from task ids to machines.
        /// </summary>
        private ConcurrentDictionary<int, Machine> TaskMap;

        /// <summary>
        /// The scheduled tasks.
        /// </summary>
        private List<Task> Tasks;

        internal List<Tuple<MachineId, int>> Schedule = new List<Tuple<MachineId, int>>();

        #endregion

        #region constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="runtime">PSharpRuntime</param>
        /// <param name="taskMap">Map from task ids to machines</param>
        internal FairTaskScheduler(PSharpRuntime runtime,
            ConcurrentDictionary<int, Machine> taskMap)
        {
            this.Runtime = runtime;
            this.TaskMap = taskMap;
            this.Tasks = new List<Task>();
        }

        #endregion

        #region override methods

        /// <summary>
        /// Queues the specified task to the scheduler.
        /// </summary>
        /// <param name="task">Task</param>
        protected override void QueueTask(Task task)
        {
            this.Runtime.Log("<ScheduleDebug> Enqueuing task {0}.", task.Id);

            if (this.TaskMap[task.Id].Id.Type.Equals("Mailbox.Mailbox"))
            {
                new Thread(state => base.TryExecuteTask((Task)state))
                {
                    IsBackground = true
                }.Start(task);
                return;
            }

            lock (this.Tasks)
            {
                this.Tasks.Add(task);
                this.NotifyThreadPoolOfPendingWork();

                //ThreadPool.QueueUserWorkItem(_ =>
                //{
                //    base.TryExecuteTask(task);
                //    this.Tasks.Remove(task);

                //}, null);
            }
        }

        /// <summary>
        /// Attempts to dequeue a task that was previously
        /// queued to this scheduler.
        /// </summary>
        /// <param name="task">Task</param>
        /// <returns>Boolean</returns>
        protected override bool TryDequeue(Task task)
        {
            this.Runtime.Log("<ScheduleDebug> Trying to dequeue task {0}.", task.Id);
            return base.TryDequeue(task);
        }

        /// <summary>
        /// Determines whether the provided task can be
        /// executed synchronously in this call, and if
        /// it can, executes it.
        /// </summary>
        /// <param name="task">Task</param>
        /// <param name="taskWasPreviouslyQueued">Boolean</param>
        /// <returns>Boolean</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;// base.TryExecuteTask(task);
        }

        /// <summary>
        /// For debugger support only, generates an enumerable
        /// of task instances currently queued to the scheduler
        /// waiting to be executed.
        /// </summary>
        /// <returns>Scheduled tasks</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            var tasks = new List<Task>();
            bool lockTaken = false;
            
            try
            {
                // Gets all tasks. We use TryEnter to hang the debugger
                // if the lock is held by a frozen thread.
                System.Threading.Monitor.TryEnter(this.Tasks, ref lockTaken);

                if (lockTaken)
                {
                    tasks.AddRange(this.Tasks.ToArray());
                }

                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken)
                {
                    System.Threading.Monitor.Exit(this.Tasks);
                }
            }

            return tasks;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Notifies the thread pool of pending work.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                //while (true)
                //{
                    Task task = null;
                    lock (this.Tasks)
                    {
                        //task = this.Tasks.OrderByDescending(val =>
                        //    this.TaskMap.ContainsKey(val.Id) ?
                        //    this.TaskMap[val.Id].InboxSize : 0).FirstOrDefault();

                        //task = this.Tasks.Where(val
                        //    => this.TaskMap[val.Id].Id.Type.Equals("Mailbox.Node")).
                        //    FirstOrDefault();

                        //task = this.Tasks.Where(val
                        //    => this.TaskMap[val.Id].Id.Type.Equals("Mailbox.Mailbox")).
                        //    FirstOrDefault();

                        task = this.Tasks.Where(val
                            => this.TaskMap[val.Id].InboxSize > 10).
                            FirstOrDefault();

                        if (task == null)
                        {
                            task = this.Tasks.FirstOrDefault();
                        }

                        if (task == null)
                        {
                            return;
                        }

                        this.Tasks.Remove(task);
                    }

                    //if (this.TaskMap.ContainsKey(val.Id))
                    //{
                        this.Schedule.Add(Tuple.Create(this.TaskMap[task.Id].Id,
                            this.TaskMap[task.Id].InboxSize));
                    //}

                    base.TryExecuteTask(task);
                //}
                
            }, null);
        }

        #endregion
    }
}
