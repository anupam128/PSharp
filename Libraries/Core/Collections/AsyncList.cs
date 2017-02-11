//-----------------------------------------------------------------------
// <copyright file="AsyncList.cs">
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
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PSharp.Collections
{
	/// <summary>
	/// Implementation of an asynchronous list.
	/// </summary>
	public sealed class AsyncList<TValue>
	{
		#region fields

		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly List<TValue> List;

		/// <summary>
		/// Semaphore used to synchronize access to the list.
		/// </summary>
		private SemaphoreSlim Lock;

		#endregion

		#region public methods

		/// <summary>
		/// Constructor.
		/// </summary>
		public AsyncList()
		{
			this.List = new List<TValue>();
			this.Lock = new SemaphoreSlim(1, 1);
		}

		/// <summary>
		/// Add a new item to the list.
		/// </summary>
		/// <param name="value">TValue</param>
		public async Task Add(TValue value)
		{
			await this.Lock.WaitAsync();
			try
			{
				this.List.Add(value);
			}
			finally
			{
				this.Lock.Release();
			}
		}

		/// <summary>
		/// Returns the item stored at the specified index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>TValue</returns>
		public async Task<TValue> Get(int index)
		{
			await this.Lock.WaitAsync();
			try
			{
				return this.List[index];
			}
			finally
			{
				this.Lock.Release();
			}
		}

		/// <summary>
		/// Returns the first item that satisfies the specified predicate.
		/// </summary>
		/// <param name="predicate">Predicate</param>
		/// <returns>TValue</returns>
		public async Task<TValue> Get(Func<TValue, bool> predicate)
		{
			await this.Lock.WaitAsync();
			try
			{
				foreach (var item in this.List)
				{
					if (predicate(item))
					{
						return item;
					}
				}

				throw new NotSupportedException();
			}
			finally
			{
				this.Lock.Release();
			}
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		/// <param name="index">Index</param>
		public async Task RemoveAt(int index)
		{
			await this.Lock.WaitAsync();
			try
			{
				this.List.RemoveAt(index);
			}
			finally
			{
				this.Lock.Release();
			}
		}

		#endregion
	}
}
