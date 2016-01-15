using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class RLocker : IDisposable
    {
        public RLocker(IRWLocker locker) {
            this.RWLocker = locker;
            this.RWLocker.EnterReadLock();
        }

        public IRWLocker RWLocker { get; private set; }

        

        public void UpgradeToWriteLock() {
            this.RWLocker.UpgradeToWriteLock();
        }

        public void Dispose()
        {
            this.RWLocker.ExitLock();
        }
    }
}
