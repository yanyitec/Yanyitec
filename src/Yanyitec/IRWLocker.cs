using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    /// <summary>
    /// 读写锁的抽象接口
    /// </summary>
    public interface IRWLocker : IDisposable
    {
        /// <summary>
        /// 进入读锁临界区
        /// </summary>
        void EnterReadLock();
        /// <summary>
        /// 退出读锁临界区
        /// </summary>
        void ExitReadLock();
        /// <summary>
        /// 进入写锁临界区
        /// </summary>
        void EnterWriteLock();
        /// <summary>
        /// 退出写锁临界区
        /// </summary>
        void ExitWriteLock();
        /// <summary>
        /// 升级到写锁临界区
        /// </summary>
        void UpgradeToWriteLock();
        /// <summary>
        /// 降级到读锁
        /// </summary>
        void DowngradeToReadLock();
        /// <summary>
        /// 退出锁保护临界区
        /// </summary>
        void ExitLock();
        /// <summary>
        /// usage: 
        /// using(locker.ReadCritical()){...}
        /// </summary>
        /// <returns></returns>
        RLocker ReadCritical();
        /// <summary>
        /// usage: 
        /// using(locker.WriteCritical()){...}
        /// </summary>

        /// <returns></returns>
        WLocker WriteCritical();
    }
}
