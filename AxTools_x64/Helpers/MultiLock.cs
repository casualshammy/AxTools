using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AxTools.Helpers
{
    internal class MultiLock
    {
        private object internalLock = new object();
        private Dictionary<Guid, object> lockObjects = new Dictionary<Guid, object>();

        /// <summary>
        /// Get lock. This instance of <see cref="MultiLock"/> will be in signaled state.
        /// You can get multiple locks on one instance of <see cref="MultiLock"/>.
        /// To release lock, use <see cref="ReleaseLock"/> method.
        /// </summary>
        /// <returns></returns>
        internal Guid GetLock()
        {
            lock (internalLock)
            {
                Guid guid = Guid.NewGuid();
                lockObjects.Add(guid, new object());
                return guid;
            }
        }

        /// <summary>
        /// Release lock. If <paramref name="guid"/> is not found, <see cref="KeyNotFoundException"/> will be raised
        /// </summary>
        /// <param name="guid"></param>
        internal void ReleaseLock(Guid guid)
        {
            lock (internalLock)
            {
                if (lockObjects.ContainsKey(guid))
                {
                    lockObjects.Remove(guid);
                }
                else
                {
                    throw new KeyNotFoundException("This GUID is not registered");
                }
            }
        }

        internal bool HasLocks
        {
            get
            {
                return lockObjects.Count > 0;
            }
        }

        /// <summary>
        /// Wait for all locks to be released
        /// </summary>
        /// <param name="timeoutMs"></param>
        internal void WaitForLocks(long timeoutMs = Int64.MaxValue)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (lockObjects.Count > 0 && stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                Thread.Sleep(5);
            }
        }

        internal Task WaitForLocksAsync(long timeoutMs = Int64.MaxValue)
        {
            return Task.Run(() => WaitForLocks(timeoutMs));
        }
    }
}