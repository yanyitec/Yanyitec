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


        public System.Threading.ReaderWriterLockSlim SynchronizingObject { get; private set; }

        public Injection(Injection parent, ItemInfo itemInfo) {
            _itemInfo = itemInfo;
            this.Parent = parent;
            if (parent != null) this.SynchronizingObject = parent.SynchronizingObject;
            this.SynchronizingObject = new System.Threading.ReaderWriterLockSlim();
        }

        #region load methods
        void LoadFromDefines(Json.JObject obj, ItemCollection items) {
            foreach (var pair in obj) {
                var sub = this.ConvertToInjection(pair.Key, pair.Value);
                if (_namedItems.ContainsKey(sub.Name))
                {
                    _namedItems[sub.Name] = sub;
                }
                else
                {
                    this._namedItems.Add(sub.Name, sub);
                }
            }
        }

        Injection ConvertToInjection(string key, Json.JToken define) {
            Injection result = null;
            ItemInfo info = null;
            if (define.ValueType != Json.ValueType.Object)
            {
                info = new ItemInfo() {
                    isDefination = true,
                    Name = key,
                    ConstValue = define.ToString()
                };
                return new Injection(this,info);
            }
            info = new ItemInfo() { Name = Name };
            var valueToken = define["$value"];
            if (!valueToken.IsUndefined)
            {
                info.ConstValue = valueToken.ToString();
                info.Kind = InjectionKinds.Constant;
                return new Injection(this,info);
            }
            var typename = define["$type"]?.ToString();
            string kindstr = define["$kind"]?.ToString();

            if (!string.IsNullOrEmpty(typename))
            {
                info.TypeName = typename;
                if (string.IsNullOrEmpty(kindstr))
                {
                    InjectionKinds kind = InjectionKinds.Container;
                    if (Enum.TryParse<InjectionKinds>(kindstr, out kind))
                    {
                        info.Kind = kind;
                    }
                    else info.Kind = InjectionKinds.NewOnce;
                }
                else info.Kind = InjectionKinds.NewOnce;
            }
            else info.Kind = InjectionKinds.Container;
            result = new Injection(this,info);
            foreach (var pair in define as Json.JObject) {
                if (pair.Key == "$type" || pair.Key == "$value" || pair.Key == "$kind") continue;
                var sub = result.ConvertToInjection(pair.Key, pair.Value);
                if (_namedItems.ContainsKey(sub.Name)) {
                    _namedItems[sub.Name] = sub;
                } else {
                    result._namedItems.Add(sub.Name, sub);
                }
            }
            return result;
            
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
                this.SynchronizingObject.EnterWriteLock();
                var evt = new InjectionChangedEventArgs(this,this.SynchronizingObject,e.ChangeKind);
               
                try {
                    foreach (var injection in added) {
                        injection._instanceFactory = null;
                        injection._changed(injection,evt);
                    }
                } finally {
                    this.SynchronizingObject.ExitWriteLock();
                }
            };
            #endregion
        }

        #endregion

        #region properties

        ItemInfo _itemInfo;
        protected internal ItemInfo ItemInfo { get; private set; }
        Action<Injection,InjectionChangedEventArgs> _changed;
        public event Action<Injection,InjectionChangedEventArgs> Changed;
        

        public Injection Parent { get; set; }


        public string Name {
            get {
                return this._itemInfo.Name ;
            }
        }

        public Type TokenType {
            get { return this._itemInfo.TokenType ?? this.ItemInfo.InjectionType; }
        }

        Type _injectionType;

        internal Type GetOrFindInjectionType() {
            if (_injectionType == null)
            {
                if (this._itemInfo.InjectionType != null) return this._injectionType = this._itemInfo.InjectionType;
                var typename = this._itemInfo.isDefination ? this._itemInfo.ConstValue?.ToString() : this._itemInfo.TypeName;
                if (!string.IsNullOrEmpty(typename)) {
                    Injection item = this.FindDepedence(typename);
                    if (item != null) {

                    }
                }
            }
            return _injectionType;
        }

        public Type InjectionType {
            get {
                this.SynchronizingObject.EnterReadLock();
                try {
                    return this.GetOrFindInjectionType();
                } finally {
                    this.SynchronizingObject.ExitReadLock();
                }
                
            }
        }

        public InjectionKinds Kind {
            get { return this._itemInfo.Kind; }
        }

        #endregion

        #region container
        
        public void Register(Type tokenType, Type injectionType, InjectionKinds kind = InjectionKinds.NewOnce){
            if (tokenType == null) tokenType = injectionType;
            if (!tokenType.IsAssignableFrom(injectionType))
            {
                throw new ArgumentException("Cant assign [" + injectionType?.FullName + "] to [" + tokenType?.FullName + "]");
            }
            this.SynchronizingObject.EnterWriteLock();
            try {
                Injection item = null;
                if (this._typedItems.TryGetValue(tokenType.GetHashCode(), out item))
                {
                    item._injectionType = injectionType;
                    item._instanceFactory = null;
                    item.Changed(item, new InjectionChangedEventArgs(this,this.SynchronizingObject, ChangeKinds.Updated) );
                }
                else
                {
                    var info = new ItemInfo()
                    {
                        TokenType = tokenType,
                        InjectionType = injectionType,
                        Kind = kind
                    };
                    item = new Injection(this, info);
                    this._typedItems.Add(tokenType.GetHashCode(), item);
                }
                Injection named = null;
                if (this._namedItems.TryGetValue(tokenType.FullName, out named))
                {
                    named._injectionType = injectionType;
                    named._instanceFactory = null;
                    named.Changed(named, new InjectionChangedEventArgs(this,this.SynchronizingObject, ChangeKinds.Updated));
                }
                else
                {
                    this._namedItems.Add(tokenType.FullName, item);
                }
            }
            finally {
                this.SynchronizingObject.ExitWriteLock();
            }
            
        }
        public void Register(Type injectionType, InjectionKinds kind = InjectionKinds.NewOnce) {
            this.Register(null,injectionType,kind);
            
        }

        public void Register<T>(InjectionKinds kind = InjectionKinds.NewOnce) {
            this.Register(typeof(T),kind);
        }

        public void Register<TToken,TInjection>(InjectionKinds kind = InjectionKinds.NewOnce){
            this.Register(typeof(TToken),typeof(TInjection), kind);
        }

        public void Register(string name, object value) {
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                Injection item = null;
                if (this._namedItems.TryGetValue(name, out item))
                {
                    item._itemInfo.ConstValue = value;
                    item._instanceFactory = null;
                    item.Changed(item, new InjectionChangedEventArgs(this,this.SynchronizingObject, ChangeKinds.Updated));
                }
                else {
                    var info = new ItemInfo()
                    {
                        Name = name,
                        ConstValue = value,
                        Kind = InjectionKinds.Constant
                    };
                    item = new Injection(this,info);
                    this._namedItems.Add(name, item);
                }
            }
            finally
            {
                this.SynchronizingObject.ExitWriteLock();
            }
        }

        public void Define(string name, string value)
        {
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                Injection item = null;
                if (!this._namedItems.TryGetValue(name, out item))
                {
                   
                    var info = new ItemInfo()
                    {
                        Name = name,
                        ConstValue = value,
                        isDefination = true
                    };
                    item = new Injection(this, info);
                    this._namedItems.Add(name, item);
                }
            }
            finally
            {
                this.SynchronizingObject.ExitWriteLock();
            }
        }

        public void Register<T>(Func<object> instanceFactory, InjectionKinds kind = InjectionKinds.Create)
        {
            this.SynchronizingObject.EnterWriteLock();
            var tokenType = typeof(T);
            try
            {
                Injection item = null;
                if (this._typedItems.TryGetValue(tokenType.GetHashCode(), out item))
                {
                    item._itemInfo.InstanceFactory = instanceFactory;
                    item._itemInfo.TokenType = tokenType;
                    item._instanceFactory = null;
                    item.Changed(item, new InjectionChangedEventArgs(this,this.SynchronizingObject, ChangeKinds.Updated));
                }
                else
                {
                    var info = new ItemInfo()
                    {
                        TokenType = tokenType,
                        InstanceFactory = instanceFactory
                    };
                    item = new Injection(this, info);
                    this._typedItems.Add(tokenType.GetHashCode(), item);
                }

                Injection named = null;
                if (this._namedItems.TryGetValue(tokenType.FullName, out named))
                {
                    named._itemInfo.TokenType = tokenType;
                    named._itemInfo.InstanceFactory = instanceFactory;
                    named._instanceFactory = null;
                    named.Changed(named, new InjectionChangedEventArgs(this,this.SynchronizingObject, ChangeKinds.Updated));
                }
                else
                {
                    this._namedItems.Add(tokenType.FullName, item);
                }
            }
            finally
            {
                this.SynchronizingObject.ExitWriteLock();
            }
        }

    



    public Injection GetByName(string name,object locker=null) {
            if (locker==this.SynchronizingObject) {
                Injection result = null;
                this._namedItems.TryGetValue(name, out result);
                return result;
            }
            this.SynchronizingObject.EnterReadLock();
            try
            {
                Injection result = null;
                this._namedItems.TryGetValue(name, out result);
                return result;
            }
            finally {
                this.SynchronizingObject.ExitReadLock();
            }
            
        }

        public Injection GetByType(Type type,object locker=null) {
            if (locker==this.SynchronizingObject) {
                Injection result = null;
                this._typedItems.TryGetValue(type.GetHashCode(), out result);
                return result;
            }
            this.SynchronizingObject.EnterReadLock();
            try
            {
                Injection result = null;
                this._typedItems.TryGetValue(type.GetHashCode(), out result);
                return result;
            }
            finally
            {
                this.SynchronizingObject.ExitReadLock();
            }
        }

        public Injection Find(string name,object locker=null) {
            if (locker == this.SynchronizingObject) return FindByName(name,this);
            this.SynchronizingObject.EnterReadLock();
            try {
                return FindByName(name,this);
            } finally {
                this.SynchronizingObject.ExitReadLock();
            }

        }

        static Injection FindByName(string key, Injection initInjection) {
            System.Collections.Generic.Queue<Injection> queue = new Queue<Injection>();
            queue.Enqueue(initInjection);
            Injection current = null;
            while ((current = queue.Dequeue()) != null) {
                foreach (var item in current._namedItems.Values) {
                    if (item.Name == key) return item;
                    if (item._namedItems.Count > 0 && item.Kind == InjectionKinds.Container) queue.Enqueue(item);
                }
            }
            return null;
        }

        public Injection Find(Type type, object locker = null) {
            if (locker == this.SynchronizingObject) return FindByType(type.GetHashCode(), this);
            this.SynchronizingObject.EnterReadLock();
            try
            {
                return FindByType(type.GetHashCode(), this);
            }
            finally
            {
                this.SynchronizingObject.ExitReadLock();
            }
        }
        static Injection FindByType(int key, Injection initInjection)
        {
            System.Collections.Generic.Queue<Injection> queue = new Queue<Injection>();
            queue.Enqueue(initInjection);
            Injection current = null;
            while ((current = queue.Dequeue()) != null)
            {
                foreach (var itemPair in current._typedItems)
                {
                    if (itemPair.Key == key) return itemPair.Value;
                    if (itemPair.Value._typedItems.Count > 0 && itemPair.Value.Kind == InjectionKinds.Container) queue.Enqueue(itemPair.Value);
                }
            }
            return null;
        }

        #endregion

        #region create instance

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
            if (locker == this.SynchronizingObject) return GetOrCreateInstanceFunc(force);
            this.SynchronizingObject.EnterWriteLock();
            try {
                return this.GetOrCreateInstanceFunc(force);
            } finally {
                this.SynchronizingObject.ExitWriteLock();
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
            if (this._itemInfo.isDefination) {
                return this.GenDefineFunc();
            }
            return this._instanceFactory = new InstanceFactoryGenerator(this).Generate();
            
        }

        Func<object> GenConstValueFunc() {
            object constValue = null;
            if (this._itemInfo.ConstValue != null) {
                constValue = this._itemInfo.ConstValue;
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

        #endregion
    }
}
