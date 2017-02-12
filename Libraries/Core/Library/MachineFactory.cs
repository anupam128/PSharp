//-----------------------------------------------------------------------
// <copyright file="MachineFactory.cs">
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PSharp
{
	/// <summary>
	/// Factory for creating P# machines.
	/// </summary>
	internal static class MachineFactory
	{
		#region static fields

		/// <summary>
		/// Cache storing machine constructors.
		/// </summary>
		private static Dictionary<Type, Func<Machine>> MachineConstructorCache;

		/// <summary>
		/// Semaphore used to synchronize access to the cache.
		/// </summary>
		private static SemaphoreSlim Lock;

		#endregion

		#region constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static MachineFactory()
		{
			MachineConstructorCache = new Dictionary<Type, Func<Machine>>();
			Lock = new SemaphoreSlim(1, 1);
		}

		#endregion

		#region methods

		/// <summary>
		/// Creates a new P# machine of the specified type.
		/// </summary>
		/// <param name="type">Type</param>
		/// <returns>Machine</returns>
		public static async Task<Machine> Create(Type type)
		{
			await Lock.WaitAsync();
			try
			{
				Func<Machine> constructor;
				if (!MachineConstructorCache.TryGetValue(type, out constructor))
				{
					constructor = Expression.Lambda<Func<Machine>>(
						Expression.New(type.GetConstructor(Type.EmptyTypes))).Compile();
					MachineConstructorCache.Add(type, constructor);
				}

				return constructor();
			}
			finally
			{
				Lock.Release();
			}
		}

		/// <summary>
		/// Checks if a machine of the specified type has been previously created.
		/// </summary>
		/// <param name="type">Type</param>
		/// <returns>Boolean</returns>
		public static async Task<bool> Created(Type type)
		{
			await Lock.WaitAsync();
			try
			{
				return MachineConstructorCache.ContainsKey(type);
			}
			finally
			{
				Lock.Release();
			}
		}

		#endregion
	}
}
