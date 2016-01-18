using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    using System.Reflection;

    public class Injection
    {
        Func<object> _instanceFactory;
        readonly SortedDictionary<string, Injection> _namedItems = new SortedDictionary<string, Injection>();
        readonly SortedDictionary<int, Injection> _typedItems = new SortedDictionary<int, Injection>();


        public System.Threading.ReaderWriterLockSlim AsyncLocker { get; private set; }

        public Injection(Injection parent, ItemInfo itemInfo) {
            _itemInfo = itemInfo;
            this.Parent = parent;
            if (parent != null) this.AsyncLocker = parent.AsyncLocker;
            this.AsyncLocker = new System.Threading.ReaderWriterLockSlim();
        }

        
        void LoadFromDefines(Json.JObject obj, ItemCollection items) {

        }
        /// <summary>
        /// 该函数不是线程安全安全的，没有用Lock保护
        /// </summary>
        /// <param name="artifact"></param>
        /// <param name="items"></param>
        void LoadFromArtifact(IArtifact artifact, ItemCollection items) {
            List<Injection> added = new List<Injection>();
            foreach (var info in artifact.GetTypeInfos()) {
                if (info.GetCustomAttribute(typeof(InjectableAttribute)) == null) continue;
                //看看是否被name注册过
                var defined = items.GetByName(info.FullName, false);
                #region 如果已经 在{"TypeName":""}中定义过，就重写原来的

                if (defined != null)
                {
                    defined._itemInfo.InjectionType = info.AsType();
                    defined._instanceFactory = null;
                    added.Add(defined);
                    continue;
                }
                #endregion
                //看看是否用 Register<>注册过
                defined = items.GetByType(info.AsType(), false);
                #region 已经用Register<>注册过了就不填了

                if (defined == null) {
                    var itemInfo = new ItemInfo() { InjectionType = info.AsType(), Kind = InjectionKinds.NewOnce };
                    var newInjection = new Injection(this, itemInfo);
                    items.AddByType(newInjection);
                    added.Add(defined);
                }
                #endregion
            }
            #region assembly发生了变化，所有的这些_instance都要重写，他们的依赖项也要重写
            artifact.Changed += (sender, e) => {
                this.AsyncLocker.EnterWriteLock();
                var evt = new InjectionChangedEventArgs(true);
                evt.Source = e;
                evt.LockedObject = this;
                try {
                    foreach (var injection in added) {
                        injection._instanceFactory = null;
                        injection._changed(injection,evt);
                    }
                } finally {
                    this.AsyncLocker.ExitWriteLock();
                }
            };
            #endregion
        }

        

        ItemInfo _itemInfo;
        protected internal ItemInfo ItemInfo { get; private set; }
        Action<Injection,InjectionChangedEventArgs> _changed;
        public event Action<Injection,InjectionChangedEventArgs> Changed;
        

        public Injection Parent { get; set; }


        public string Name {
            get {
                return this._itemInfo.Name;
            }
        }

        public Type TokenType {
            get { return this._itemInfo.TokenType ?? this.ItemInfo.InjectionType; }
        }

        Type _injectionType;

        public Type InjectionType {
            get {
                return _injectionType ?? this._itemInfo.InjectionType;
            }
        }

        public InjectionKinds Kind {
            get { return this._itemInfo.Kind; }
        }


        

        public Injection GetByName(string name,object locker=null) {
            if (locker==this.AsyncLocker) {
                Injection result = null;
                this._namedItems.TryGetValue(name, out result);
                return result;
            }
            this.AsyncLocker.EnterReadLock();
            try
            {
                Injection result = null;
                this._namedItems.TryGetValue(name, out result);
                return result;
            }
            finally {
                this.AsyncLocker.ExitReadLock();
            }
            
        }

        public Injection GetByType(Type type,object locker=null) {
            if (locker==this.AsyncLocker) {
                Injection result = null;
                this._typedItems.TryGetValue(type.GetHashCode(), out result);
                return result;
            }
            this.AsyncLocker.EnterReadLock();
            try
            {
                Injection result = null;
                this._typedItems.TryGetValue(type.GetHashCode(), out result);
                return result;
            }
            finally
            {
                this.AsyncLocker.ExitReadLock();
            }
        }

        public Injection Search(string name,object locker=null) {
            if (locker == this.AsyncLocker) return InternalSearchByName(name,this);
            this.AsyncLocker.EnterReadLock();
            try {
                return InternalSearchByName(name,this);
            } finally {
                this.AsyncLocker.ExitReadLock();
            }

        }

        static Injection InternalSearchByName(string key, Injection initInjection) {
            System.Collections.Generic.Queue<Injection> queue = new Queue<Injection>();
            queue.Enqueue(initInjection);
            Injection current = null;
            while ((current = queue.Dequeue()) != null) {
                foreach (var item in current._namedItems.Values) {
                    if (item.Name == key) return item;
                    if (item._namedItems.Count > 0) queue.Enqueue(item);
                }
            }
            return null;
        }

        public Injection Search(Type type, object locker = null) {
            if (locker == this.AsyncLocker) return InternalSearchByType(type.GetHashCode(), this);
            this.AsyncLocker.EnterReadLock();
            try
            {
                return InternalSearchByType(type.GetHashCode(), this);
            }
            finally
            {
                this.AsyncLocker.ExitReadLock();
            }
        }
        static Injection InternalSearchByType(int key, Injection initInjection)
        {
            System.Collections.Generic.Queue<Injection> queue = new Queue<Injection>();
            queue.Enqueue(initInjection);
            Injection current = null;
            while ((current = queue.Dequeue()) != null)
            {
                foreach (var itemPair in current._typedItems)
                {
                    if (itemPair.Key == key) return itemPair.Value;
                    if (itemPair.Value._typedItems.Count > 0) queue.Enqueue(itemPair.Value);
                }
            }
            return null;
        }


        protected internal Injection FindDepedence(string name) {
            Injection result = null;
            if (this._namedItems.TryGetValue(name, out result)) return result;
            if (this.Parent != null) return this.Parent.FindDepedence(name);
            return null;

        }

        protected internal Injection FindDepedence(Type type)
        {
            Injection result = null;
            if (this._typedItems.TryGetValue(type.GetHashCode(), out result)) return result;
            if (this._namedItems.TryGetValue(type.FullName, out result))
            {
                if (type.IsAssignableFrom(result.TokenType)) return result;
                return result;
            }
            if (this.Parent != null) return this.Parent.FindDepedence(type);
            return null;

        }

        protected internal void ResetInstanceFactory() {
            this._instanceFactory = null;
        }

        public object GetOrCreateInstance() {
            if (_instanceFactory == null) {this.GetOrCreateInstanceFunc(true,null);}
            return _instanceFactory();
        }

        protected internal Func<object> GetOrCreateInstanceFunc(bool force = false, object locker=null) {
            if (locker == this.AsyncLocker) return GetOrCreateInstanceFunc(force);
            this.AsyncLocker.EnterWriteLock();
            try {
                return this.GetOrCreateInstanceFunc(force);
            } finally {
                this.AsyncLocker.ExitWriteLock();
            }
        }

        Func<object> GetOrCreateInstanceFunc(bool force=false) {
            if (!force && this._instanceFactory != null) return this._instanceFactory;
            if (this._itemInfo.Kind == InjectionKinds.Container) {
                return new Func<object>(() => { throw new InvalidOperationException("Cannot use create instance while this item is Container"); });
            }
            if (this._itemInfo.Kind == InjectionKinds.Constant) {
                return this.GenConstValueFunc();
            }
            if (this._itemInfo.Kind == InjectionKinds.Define) {
                return this.GenDefineFunc();
            }
            return this._instanceFactory = new InstanceFactoryGenerator(this).Generate();
            
        }

        Func<object> GenConstValueFunc() {
            object constValue = null;
            if (this._itemInfo.ConstValue != null) {
                
                constValue = this.ItemInfo.ConstValue;
                if (constValue != null)
                {
                    this._injectionType = constValue.GetType();
                }
                else {
                    this._injectionType = this._itemInfo.InjectionType;
                }
                return ()=>constValue;
            }
            if (this._itemInfo.InstanceFactory != null) {
                constValue = this._itemInfo.InstanceFactory();
                if (this._itemInfo.InjectionType == null && constValue!=null)
                {
                    this._injectionType = constValue.GetType();
                }
                return ()=>constValue;
            }
            var func = new InstanceFactoryGenerator(this).Generate();
            constValue = func();
            if (this._itemInfo.InjectionType == null && constValue != null)
            {
                this._injectionType = this._itemInfo.ConstValue.GetType();
            }
            return () => constValue;
        }

        Func<object> GenDefineFunc() {
            Injection actualInjection = null;
            if (this._itemInfo.TypeName!=null) {
                actualInjection = this.FindDepedence(this._itemInfo.ConstValue.ToString());
                if (actualInjection != this)
                {
                    return actualInjection.GetOrCreateInstanceFunc(false);
                }
            }

            var constValue = this._itemInfo.ConstValue;
            if (constValue != null) {
                this._injectionType = constValue.GetType();
            }
            return () => this.ItemInfo.ConstValue;
        }
    }
}
