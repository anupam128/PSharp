//-----------------------------------------------------------------------
// <copyright file="AsyncDictionary.cs">
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
	/// Implementation of an asynchronous dictionary.
	/// </summary>
	internal sealed class AsyncDictionary<TKey, TValue>
	{
		#region fields

		/// <summary>
		/// The internal dictionary.
		/// </summary>
		private readonly Dictionary<TKey, TValue> Dictionary;

		/// <summary>
		/// Semaphore used to synchronize access to the dictionary.
		/// </summary>
		private SemaphoreSlim Lock;

        #endregion

        #region properties

        /// <summary>
        /// The dictionary keys. This is not thread safe.
        /// </summary>
        internal IEnumerable<TKey> Keys
        {
            get
            {
                return this.Dictionary.Keys;
            }
        }

        /// <summary>
        /// The dictionary values. This is not thread safe.
        /// </summary>
        internal IEnumerable<TValue> Values
        {
            get
            {
                return this.Dictionary.Values;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public AsyncDictionary()
		{
			this.Dictionary = new Dictionary<TKey, TValue>();
			this.Lock = new SemaphoreSlim(1, 1);
		}

		/// <summary>
		/// Add a new entry with the specified key and value.
		/// </summary>
		/// <param name="key">TKey</param>
		/// <param name="value">TValue</param>
		public async Task Add(TKey key, TValue value)
		{
			await this.Lock.WaitAsync();
			try
			{
				this.Dictionary.Add(key, value);
			}
			finally
			{
				this.Lock.Release();
			}
		}

		/// <summary>
		/// Returns the value stored with the specified key, if it exists.
		/// </summary>
		/// <param name="key">TKey</param>
		/// <returns>Result</returns>
		public async Task<Tuple<bool, TValue>> TryGetValue(TKey key)
		{
			await this.Lock.WaitAsync();
			try
			{
				TValue value;
				bool found = this.Dictionary.TryGetValue(key, out value);
				return Tuple.Create(found, value);
			}
			finally
			{
				this.Lock.Release();
			}
		}

		/// <summary>
		/// Checks if the dictionary contains the specified key.
		/// </summary>
		/// <param name="key">TKey</param>
		/// <returns>Boolean</returns>
		public async Task<bool> ContainsKey(TKey key)
		{
			await this.Lock.WaitAsync();
			try
			{
				return this.Dictionary.ContainsKey(key);
			}
			finally
			{
				this.Lock.Release();
			}
		}

		/// <summary>
		/// Removes the dictionary entry with the specified key.
		/// </summary>
		/// <param name="key">TKey</param>
		public async Task Remove(TKey key)
		{
			await this.Lock.WaitAsync();
			try
			{
				this.Dictionary.Remove(key);
			}
			finally
			{
				this.Lock.Release();
			}
		}

		#endregion
	}
}
