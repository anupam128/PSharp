//-----------------------------------------------------------------------
// <copyright file="PSharpRuntime.cs">
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

using Microsoft.PSharp.Net;
using Microsoft.PSharp.Utilities;

namespace Microsoft.PSharp
{
	/// <summary>
	/// The P# runtime.
	/// </summary>
	public class PSharpRuntime
	{
		/// <summary>
		/// Creates a new P# runtime.
		/// </summary>
		/// <returns>Runtime</returns>
		public static IPSharpRuntime Create()
		{
			return new Runtime();
		}

		/// <summary>
		/// Creates a new P# runtime.
		/// </summary>
		/// <param name="netProvider">NetworkProvider</param>
		/// <returns>Runtime</returns>
		public static IPSharpRuntime Create(INetworkProvider netProvider)
		{
			return new Runtime(netProvider);
		}

		/// <summary>
		/// Creates a new P# runtime.
		/// </summary>
		/// <param name="configuration">Configuration</param>
		/// <returns>Runtime</returns>
		public static IPSharpRuntime Create(Configuration configuration)
		{
			return new Runtime(configuration);
		}

		/// <summary>
		/// Creates a new P# runtime.
		/// </summary>
		/// <param name="configuration">Configuration</param>
		/// <param name="netProvider">NetworkProvider</param>
		/// <returns>Runtime</returns>
		public static IPSharpRuntime Create(Configuration configuration, INetworkProvider netProvider)
		{
			return new Runtime(configuration, netProvider);
		}
	}
}
