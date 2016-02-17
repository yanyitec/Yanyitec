using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Caches
{
    public class MemCache : ICache
    {
        System.Threading.ReaderWriterLockSlim _syncObject;
        
        

        private MemCache(MemCache parent) {
            this.Parent = parent;
            this._syncObject = parent._syncObject;
        }

        public MemCache(int expireMilliseconds = 6000000, int checkIntervalMilliseconds = 3000000)
        {
            _syncObject = new System.Threading.ReaderWriterLockSlim();
            _expireMilliseconds = expireMilliseconds<=0? 6000000: expireMilliseconds;
            _checkIntervalMilliseconds = checkIntervalMilliseconds<=0? 3000000: checkIntervalMilliseconds;
        }

        public MemCache Parent { get; private set; } 
        public System.Threading.ReaderWriterLockSlim SynchronizingObject {
            get { return _syncObject; }
        }

        int _expireMilliseconds;
        public int ExpireMilliseconds {
            get {
                if (Parent != null) return Parent.ExpireMilliseconds;
                this._syncObject.EnterReadLock();
                try {
                    return _expireMilliseconds;
                } finally {
                    this._syncObject.ExitReadLock();
                }
            }
            set {
                if (Parent != null) Parent.ExpireMilliseconds = value;
                this._syncObject.ExitWriteLock();
                try
                {
                    if (value <= 0) _expireMilliseconds = 600000;
                    else _expireMilliseconds = value;
                    
                }
                finally
                {
                    this._syncObject.ExitWriteLock();
                }
            }
        }

        int _checkIntervalMilliseconds;

        public int CheckIntervalMilliseconds
        {
            get
            {
                if (Parent != null) return this.Parent.CheckIntervalMilliseconds;
                this._syncObject.EnterReadLock();
                try
                {
                    return _checkIntervalMilliseconds;
                }
                finally
                {
                    this._syncObject.ExitReadLock();
                }
            }
            set
            {
                if (Parent != null) this.Parent.CheckIntervalMilliseconds = value;
                this._syncObject.ExitWriteLock();
                try
                {
                    if (_checkIntervalMilliseconds <= 0)
                        _checkIntervalMilliseconds = 300000;
                    else _checkIntervalMilliseconds = value;
                }
                finally
                {
                    this._syncObject.ExitWriteLock();
                }
            }
        }

        #region items
        SortedDictionary<string, CacheItem> _items = new SortedDictionary<string, CacheItem>();

        public int Count {
            get {
                this._syncObject.EnterReadLock();
                try {
                    return _items.Count;
                } finally {
                    this._syncObject.ExitReadLock();
                }
            }
        }

        
        
        public CacheItem GetItem(string name) {
            this._syncObject.EnterReadLock();
            try {
                CacheItem result = null;
                if (_items.TryGetValue(name, out result)) {
                    result._LastAccessTime = DateTime.Now;
                }
                
                return result;
            } finally {
                this._syncObject.ExitReadLock();
            }
        }

        public CacheItem SetItem(string name,object value)
        {
            return SetItem(name,value, default(DateTime));
        }

        public CacheItem SetItem(string name, object value,DateTime expireTime)
        {
            this._syncObject.EnterUpgradeableReadLock();
            try
            {
                CacheItem result = null;
                if (!_items.TryGetValue(name, out result))
                {
                    this._syncObject.EnterWriteLock();
                    try
                    {
                        if (!_items.TryGetValue(name, out result))
                        {
                            result = new CacheItem()
                            {
                                Name = name,
                                _Value = value,
                                _ExpireTime = expireTime,
                                _LastAccessTime = DateTime.Now
                            };
                            _items.Add(name, result);
                            TryStartTask();
                            return result;
                        }
                        else
                        {
                            
                        }
                    }
                    finally
                    {
                        this._syncObject.ExitWriteLock();
                    }
                }
                lock (result) {
                    result._LastAccessTime = DateTime.Now;
                    result._Value = value;
                    result._ExpireTime = expireTime;
                }
                return result;
            }
            finally
            {
                this._syncObject.ExitUpgradeableReadLock();
            }
        }

        public object this[string name] {
            get {
                var item = this.GetItem(name);
                if (item != null) return item.Value;
                return null;
            }
            set {
                this.SetItem(name, value);
            }
        }
        #endregion

        #region Subs
        SortedDictionary<string, MemCache> _subCaches = new SortedDictionary<string, MemCache>();

        public ICache GetOrCreateSubCache(string name) {
            this._syncObject.EnterUpgradeableReadLock();
            try {
                MemCache result = null;
                if (!_subCaches.TryGetValue(name, out result)) {
                    this._syncObject.EnterWriteLock();
                    try
                    {
                        if (!_subCaches.TryGetValue(name, out result))
                        {
                            result = new MemCache(this);
                            this._subCaches.Add(name,result);
                            TryStartTask();
                        }
                    }
                    finally
                    {
                        this._syncObject.ExitWriteLock();
                    }
                }
                return result;
            } finally {
                this._syncObject.ExitUpgradeableReadLock();
            }
        }
        #endregion

        #region remove expired
        public IList<CacheItem> RemoveExpired(DateTime? now = null) {
            if (now == null) now = DateTime.Now;
            this._syncObject.EnterWriteLock();
            try {
                var result = InternalRemoveExpired(now.Value);
                
                return result;
            } finally {
                this._syncObject.ExitWriteLock();
            }
        }

        protected internal IList<CacheItem> InternalRemoveExpired(DateTime now) {
            DateTime expireTime = DateTime.Now.AddMilliseconds(-this._expireMilliseconds);
            var removed = new List<CacheItem>();
            RemoveExpired(this,now, expireTime, removed);
            
            return removed;
        }

        

        static void RemoveExpired(MemCache cache, DateTime now, DateTime expireTime, List<CacheItem> removed)
        {
            cache._items = RemoveExpiredItems(now, expireTime, cache._items, removed);
            var newSubs = new SortedDictionary<string, MemCache>();
            foreach (var pair in cache._subCaches) {
                var sub = pair.Value;
                RemoveExpired(sub,now,expireTime,removed);
                if (sub._items.Count != 0 || sub._subCaches.Count != 0) {
                    newSubs.Add(pair.Key,sub);
                }
            }
            cache._subCaches = newSubs;
        }

        static SortedDictionary<string, CacheItem> RemoveExpiredItems(DateTime now, DateTime expireTime,SortedDictionary<string, CacheItem> items, List<CacheItem> removed) {
            var newItems = new SortedDictionary<string, CacheItem>();
            
            foreach (var item in items.Values)
            {
                var itemExpireTime = item.ExpireTime;
                if (itemExpireTime != default(DateTime))
                {
                    #region 指定了过期时间的
                    if (itemExpireTime <= now)
                    {
                        removed.Add(item);
                    }
                    else
                    {
                        newItems.Add(item.Name, item);
                    }
                    #endregion
                }
                else
                {
                    #region 没指定过期时间的
                    var itemLastAccessTime = item.LastAccessTime;
                    if (itemLastAccessTime <= expireTime)
                    {
                        removed.Add(item);
                    }
                    else {
                        newItems.Add(item.Name,item);
                    }
                    #endregion
                }
            }
            return newItems;
        }
        #endregion

        #region thread
        Task _removeExpiredTask;
        bool InternalHasCacheItems() {
            if (this._items.Count > 0) return true;
            if (this._subCaches.Count == 0) return false;
            foreach (var sub in this._subCaches.Values) {
                if (sub.InternalHasCacheItems()) return true;
            }
            return false;
        }
        bool TryStartTask() {
            if (this.Parent != null) return this.Parent.TryStartTask();
            if (this._removeExpiredTask != null) return true;
            if (!this.InternalHasCacheItems()) return false;
            _removeExpiredTask = new Task(async () => { await RemoveExpired(); });
            _removeExpiredTask.Start();
            return true;
        }

        async Task RemoveExpired() {
            IList<CacheItem> removed = null;
            while (true) {
                DateTime now = DateTime.Now;
                int sleepMillsecounds = 0;
                this._syncObject.EnterWriteLock();
                removed = null;
                try
                {
                    sleepMillsecounds = this._checkIntervalMilliseconds;
                    removed = InternalRemoveExpired(now);
                    //没有缓存了
                    if (!InternalHasCacheItems())
                    {
                        _removeExpiredTask = null;
                        
                        break;
                    }
                }
                finally
                {
                    this._syncObject.ExitWriteLock();
                }
                #region 新线程 处理已经被移除的项目
                if (removed.Count > 0) {
                    new Task(()=> {
                        foreach (var item in removed) {
                            try {
                                if (item._Removed != null) item._Removed(item);
                            } catch { }
                        }
                    }).Start();
                }
                #endregion
                
                await Task.Delay(sleepMillsecounds);
            }
            if (removed != null && removed.Count > 0) {
                foreach (var item in removed)
                {
                    try
                    {
                        if (item._Removed != null) item._Removed(item);
                    }
                    catch { }
                }
            }
        }


        #endregion
        #region enumerator
        public IEnumerator<CacheItem> GetEnumerator()
        {
            this._syncObject.EnterReadLock();
            try {
                var list = new List<CacheItem>();
                foreach (var pair in this._items) list.Add(pair.Value);
                return list.GetEnumerator();
            } finally {
                this._syncObject.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }

}
