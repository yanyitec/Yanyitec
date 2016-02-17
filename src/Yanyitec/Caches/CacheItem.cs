using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Caches
{
    public class CacheItem
    {
        internal object _Value;
        public object Value {
            get {  lock(this) { this._LastAccessTime = DateTime.Now; return _Value; } }
            internal set { lock(this) { _Value = value; this._LastAccessTime = DateTime.Now; } }
        }
        internal DateTime _LastAccessTime;
        public DateTime LastAccessTime {
            get {
                lock(this)return _LastAccessTime;
            }
            
        }

        internal DateTime _ExpireTime;
        public DateTime ExpireTime
        {
            get
            {
                lock (this) {  return _ExpireTime; }
            }
            set
            {
                lock (this)
                {
                    _ExpireTime = value;
                    this._LastAccessTime = DateTime.Now;
                }
            }
        }

        public string Name { get; internal set; }
        internal Action<CacheItem> _Removed;
        public event Action<CacheItem> Removed {
            add {
                lock (this) { _Removed += value; this._LastAccessTime = DateTime.Now; }
            }
            remove {
                lock (this) { _Removed -= value; this._LastAccessTime = DateTime.Now; }
            }
        }
    }
}
