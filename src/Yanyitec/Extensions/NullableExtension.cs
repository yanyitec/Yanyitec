using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    using System.Reflection;
    using System.Linq.Expressions;
    public static class NullableExtension
    {
        static MethodInfo GetValueMethodInfo = typeof(Nullable<>).GetMethod("get_Value");
        static readonly SortedDictionary<int, Func<object, object>> _valueGetters = new SortedDictionary<int, Func<object, object>>() {
            
            { typeof(bool?).GetHashCode(),GenValueGetter(typeof(bool?))}
            , { typeof(char?).GetHashCode(),GenValueGetter(typeof(char?))}
            , { typeof(byte?).GetHashCode(),GenValueGetter(typeof(byte?))}
            , { typeof(short?).GetHashCode(),GenValueGetter(typeof(short?))}
            , { typeof(ushort?).GetHashCode(),GenValueGetter(typeof(ushort?))}
            , { typeof(int?).GetHashCode(),GenValueGetter(typeof(int?))}
            , { typeof(uint?).GetHashCode(),GenValueGetter(typeof(uint?))}
            , { typeof(long?).GetHashCode(),GenValueGetter(typeof(long?))}
            , { typeof(ulong?).GetHashCode(),GenValueGetter(typeof(ulong?))}
            , { typeof(float?).GetHashCode(),GenValueGetter(typeof(float?))}
            , { typeof(double?).GetHashCode(),GenValueGetter(typeof(double?))}
            , { typeof(decimal?).GetHashCode(),GenValueGetter(typeof(decimal?))}
            , { typeof(DateTime?).GetHashCode(),GenValueGetter(typeof(DateTime?))}
        };
        static readonly SortedDictionary<int, Func<object, object>> _customerValueGetters = new SortedDictionary<int, Func<object, object>>();
        public static object GetValue(object nullableValue) {
            if (nullableValue == null) return null;
            Func<object, object> getter = null;
            var type = nullableValue.GetType();
            var typeid = type.GetHashCode();
            if (_valueGetters.TryGetValue(typeid, out getter)) return getter(nullableValue);
            GlobalLocker.InternalSynchronizingObject.EnterUpgradeableReadLock();
            try {
                if (_customerValueGetters.TryGetValue(typeid, out getter)) return getter(nullableValue);
                GlobalLocker.InternalSynchronizingObject.EnterWriteLock();
                try {
                    if (_customerValueGetters.TryGetValue(typeid, out getter)) return getter(nullableValue);
                    getter = GenValueGetter(type);
                    if (getter != null)
                    {
                        _customerValueGetters.Add(typeid, getter);
                        return getter(nullableValue);
                    }
                    else return null;
                } finally {
                    GlobalLocker.InternalSynchronizingObject.ExitWriteLock();
                }
            } finally {
                GlobalLocker.InternalSynchronizingObject.ExitUpgradeableReadLock();
            }
        }
        
        public static Func<object, object> GenValueGetter(Type nullableType) {
            if (!nullableType.FullName.StartsWith("System.Nullable")) return null;
            var underlyingType = nullableType.GetGenericArguments()[0];
            if (!underlyingType.GetTypeInfo().IsValueType) return null;

            var nullable = Expression.Parameter(typeof(object));
            var nullType = typeof(Nullable<>).MakeGenericType(underlyingType);
            var toNullable = Expression.Convert(Expression.Constant(nullable), nullType);
            var getValue = Expression.Call(toNullable, GetValueMethodInfo);
            var returnExpr = Expression.Convert(getValue, typeof(object));
            var lamda = Expression.Lambda<Func<object, object>>(
            returnExpr, nullable
                );
            var func = lamda.Compile();
            return func;
        }
    }
}
