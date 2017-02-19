//-----------------------------------------------------------------------
// <copyright file="SEMOneMachine12Test.cs">
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
    public class SEMOneMachine12Test
    {
        class E1 : Event
        {
            public E1() : base(1, -1) { }
        }

        class E2 : Event
        {
            public E2() : base(1, -1) { }
        }

        class E3 : Event
        {
            public E3() : base(1, -1) { }
        }

        class Real1 : Machine
        {
            bool test = false;

            [Start]
            [OnEntry(nameof(EntryInit))]
            [OnExit(nameof(ExitInit))]
            [OnEventPushState(typeof(E1), typeof(S1))]
            [OnEventDoAction(typeof(E3), nameof(Action1))]
            class Init : MachineState { }

            async Task EntryInit()
            {
                await this.Send(this.Id, new E1());
            }

            async Task ExitInit()
            {
                await this.Send(this.Id, new E2()); // never executed
            }

            [OnEntry(nameof(EntryS1))]
            class S1 : MachineState { }

            async Task EntryS1()
            {
                test = true;
                await this.Send(this.Id, new E3());
                await this.Pop();
            }

            void Action1()
            {
                this.Assert(test == false);  // reachable
            }
        }

        public static class TestProgram
        {
            [Test]
            public static async Task Execute(IPSharpRuntime runtime)
            {
                await runtime.CreateMachineAsync(typeof(Real1));
            }
        }

        /// <summary>
        /// P# semantics test: one machine, "push" transition, inherited action
        /// is not executed in the pushed state, due to the explicit "pop".
        /// This test checks explicit popping of a state.
        /// </summary>
        [TestMethod]
        public void TestPushExplicitPop()
        {
            var configuration = Configuration.Create();
            configuration.SuppressTrace = true;
            configuration.Verbose = 2;

            var engine = TestingEngineFactory.CreateBugFindingEngine(
                configuration, TestProgram.Execute);
            engine.Run();

            Assert.AreEqual(1, engine.TestReport.NumOfFoundBugs);
        }
    }
}
