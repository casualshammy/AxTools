using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AxTools.Helpers
{
    internal class MultiLock
    {
        private object internalLock = new object();
        private Dictionary<Guid, object> lockObjects = new Dictionary<Guid, object>();

        internal Guid GetLock()
        {
            lock (internalLock)
            {
                Guid guid = Guid.NewGuid();
                lockObjects.Add(guid, new object());
                return guid;
            }
        }

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

        internal void WaitForLocks(long timeoutMs = Int64.MaxValue)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (lockObjects.Count > 0 && stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                Thread.Sleep(5);
            }
        }

    }
}
