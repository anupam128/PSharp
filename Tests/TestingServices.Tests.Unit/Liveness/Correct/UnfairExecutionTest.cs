//-----------------------------------------------------------------------
// <copyright file="UnfairExecutionTest.cs">
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

using System.Threading.Tasks;

using Microsoft.PSharp.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.PSharp.TestingServices.Tests.Unit
{
    [TestClass]
    public class UnfairExecutionTest
    {
        class Unit : Event { }

        class E : Event
        {
            public MachineId A;

            public E(MachineId a)
            {
                this.A = a;
            }
        }

        class M : Machine
        {
            MachineId N;

            [Start]
            [OnEntry(nameof(SOnEntry))]
            [OnEventGotoState(typeof(Unit), typeof(S2))]
            class S : MachineState { }

            async Task SOnEntry()
            {
                this.N = await this.CreateMachine(typeof(N));
                await this.Send(this.N, new E(this.Id));
                this.Raise(new Unit());
            }

            [OnEntry(nameof(S2OnEntry))]
            [OnEventGotoState(typeof(Unit), typeof(S2))]
            [OnEventGotoState(typeof(E), typeof(S3))]
            class S2 : MachineState { }

            async Task S2OnEntry()
            {
                await this.Send(this.Id, new Unit());
            }

            [OnEntry(nameof(S3OnEntry))]
            class S3 : MachineState { }

            async Task S3OnEntry()
            {
                await this.Monitor<LivenessMonitor>(new E(this.Id));
                this.Raise(new Halt());
            }
        }

        class N : Machine
        {
            [Start]
            [OnEventDoAction(typeof(Unit), nameof(Foo))]
            class S : MachineState { }

            async Task Foo()
            {
                await this.Send((this.ReceivedEvent as E).A, new E(this.Id));
            }
        }

        class LivenessMonitor : Monitor
        {
            [Start]
            [Hot]
            [OnEventGotoState(typeof(E), typeof(S2))]
            class S : MonitorState { }

            [Cold]
            class S2 : MonitorState { }
        }

        public static class TestProgram
        {
            [Test]
            public static async Task Execute(IPSharpRuntime runtime)
            {
                await runtime.RegisterMonitorAsync(typeof(LivenessMonitor));
                await runtime.CreateMachineAsync(typeof(M));
            }
        }

        [TestMethod]
        public void TestUnfairExecution()
        {
            var configuration = Configuration.Create();
            configuration.SuppressTrace = true;
            configuration.Verbose = 3;
            configuration.LivenessTemperatureThreshold = 150;
            configuration.SchedulingStrategy = SchedulingStrategy.PCT;
            configuration.MaxSchedulingSteps = 300;

            IO.Debugging = true;

            var engine = TestingEngineFactory.CreateBugFindingEngine(
                configuration, TestProgram.Execute);
            engine.Run();
            
            Assert.AreEqual(0, engine.TestReport.NumOfFoundBugs);
        }
    }
}
