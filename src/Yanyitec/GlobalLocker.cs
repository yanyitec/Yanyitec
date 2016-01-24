using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class GlobalLocker
    {
        internal static readonly System.Threading.ReaderWriterLockSlim InternalSynchronizingObject = new System.Threading.ReaderWriterLockSlim();
        public static readonly System.Threading.ReaderWriterLockSlim Instance = new System.Threading.ReaderWriterLockSlim();
    }
}
