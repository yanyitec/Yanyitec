using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class WLocker : IDisposable
    {
        public WLocker(IRWLocker locker)
        {
            this.RWLocker = locker;
            this.RWLocker.EnterWriteLock();
        }

        public IRWLocker RWLocker { get; private set; }



        public void DowngradeToReadLock()
        {
            this.RWLocker.DowngradeToReadLock();
        }

        public void Dispose()
        {
            this.RWLocker.ExitLock();
        }
    }
}
