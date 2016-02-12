
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    public static class StringConvertExtension
    {
        public static bool? ToBoolean(this string self)
        {
            if (self == null) return null;
            bool result = true;
            if (bool.TryParse(self, out result)) return result;
            if (self.Length <= 5)
            {
                var up = self.ToLower();
                if (up == "true" || up == "on") return true;
                if (up == "false" || up == "off" || up == "0") return false;
            }
            int x = 0;
            if (int.TryParse(self, out x)) return x!=0;
            
            return null;
        }

        public static object ToBooleanObject(this string self)
        {
            if (self == null) return null;
            bool result = true;
            if (bool.TryParse(self, out result)) return result;
            if (self.Length <= 5)
            {
                var up = self.ToLower();
                if (up == "true" || up == "on") return true;
                if (up == "false" || up == "off" || up == "0") return false;
            }
            int x = 0;
            if (int.TryParse(self, out x)) return x != 0;

            return null;
        }


        public static char? ToChar(this string self)
        {
            char result = '\0';
            if (char.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToCharObject(this string self)
        {
            char result = '\0';
            if (char.TryParse(self, out result)) return result;
            return null;
        }

        public static byte? ToByte(this string self)
        {
            byte result = 0;
            if (byte.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToByteObject(this string self)
        {
            byte result = 0;
            if (byte.TryParse(self, out result)) return result;
            return null;
        }

        public static short? ToShort(this string self)
        {
            short result = 0;
            if (short.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToShortObject(this string self)
        {
            short result = 0;
            if (short.TryParse(self, out result)) return result;
            return null;
        }

        public static ushort? ToUShort(this string self)
        {
            ushort result = 0;
            if (ushort.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToUShortObject(this string self)
        {
            ushort result = 0;
            if (ushort.TryParse(self, out result)) return result;
            return null;
        }



        public static int? ToInt(this string self) {
            int result = 0;
            if (int.TryParse(self, out result)) return new Nullable<int>(result);
            return null;
        }
        public static object ToIntObject(this string self)
        {
            int result = 0;
            if (int.TryParse(self, out result)) return result;
            return null;
        }

        public static uint? ToUInt(this string self)
        {
            uint result = 0;
            if (uint.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToUIntObject(this string self)
        {
            uint result = 0;
            if (uint.TryParse(self, out result)) return result;
            return null;
        }

        public static long? ToLong(this string self)
        {
            long result = 0;
            if (long.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToLongObject(this string self)
        {
            long result = 0;
            if (long.TryParse(self, out result)) return result;
            return null;
        }
        public static ulong? ToULong(this string self)
        {
            ulong result = 0;
            if (ulong.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToULongObject(this string self)
        {
            ulong result = 0;
            if (ulong.TryParse(self, out result)) return result;
            return null;
        }

        public static double? ToDouble(this string self)
        {
            double result = 0;
            if (double.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToDoubleObject(this string self)
        {
            double result = 0;
            if (double.TryParse(self, out result)) return result;
            return null;
        }

        public static float? ToFloat(this string self)
        {
            float result = 0;
            if (float.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToFloatObject(this string self)
        {
            float result = 0;
            if (float.TryParse(self, out result)) return result;
            return null;
        }

        public static decimal? ToDecimal(this string self)
        {
            decimal result = 0;
            if (decimal.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToDecimalObject(this string self)
        {
            decimal result = 0;
            if (decimal.TryParse(self, out result)) return result;
            return null;
        }

        public static Guid? ToGuid(this string self)
        {
            Guid result = Guid.Empty;
            if (Guid.TryParse(self, out result)) return result;
            return null;
        }

        public static object ToGuidObject(this string self)
        {
            Guid result = Guid.Empty;
            if (Guid.TryParse(self, out result)) return result;
            return null;
        }

        public static DateTime? ToDateTime(this string self)
        {
            DateTime result = DateTime.MinValue;
            if (DateTime.TryParse(self, out result)) return result;
            long tick = 0;
            if (long.TryParse(self, out tick)) return new DateTime(tick);
            return null;
        }

        public static object ToDateTimeObject(this string self)
        {
            DateTime result = DateTime.MinValue;
            if (DateTime.TryParse(self, out result)) return result;
            long tick = 0;
            if (long.TryParse(self, out tick)) return new DateTime(tick);
            return null;
        }

        public static object ToEnumObject(this string self, Type enumType) {
            try {
                var result = Enum.Parse(enumType, self);
                return result;
            }
            catch {
                int x = 0;
                if (int.TryParse(self, out x)) return x;
                return null;
            }

        }

        public static Nullable<T> ToEnum<T>(this string self) where T : struct {
            T result = default(T);
            if (Enum.TryParse<T>(self, out result)) return result;
            return new Nullable<T>();
        }

        static readonly SortedDictionary<int, Func<string, object>> _nullableConvertors = new SortedDictionary<int, Func<string,object>>() {
            { typeof(bool).GetHashCode(),(text)=>StringConvertExtension.ToBoolean(text) }
            ,  { typeof(byte).GetHashCode(),(text)=>StringConvertExtension.ToByte(text) }
            ,  { typeof(char).GetHashCode(),(text)=>StringConvertExtension.ToChar(text) }
            ,  { typeof(short).GetHashCode(),(text)=>StringConvertExtension.ToShort(text) }
            ,  { typeof(ushort).GetHashCode(),(text)=>StringConvertExtension.ToUShort(text) }
            ,  { typeof(int).GetHashCode(),(text)=>StringConvertExtension.ToInt(text) }
            ,  { typeof(uint).GetHashCode(),(text)=>StringConvertExtension.ToUInt(text) }
            ,  { typeof(long).GetHashCode(),(text)=>StringConvertExtension.ToLong(text) }
            ,  { typeof(ulong).GetHashCode(),(text)=>StringConvertExtension.ToULong(text) }
            ,  { typeof(float).GetHashCode(),(text)=>StringConvertExtension.ToFloat(text) }
            ,  { typeof(double).GetHashCode(),(text)=>StringConvertExtension.ToDouble(text) }
            ,  { typeof(decimal).GetHashCode(),(text)=>StringConvertExtension.ToDecimal(text) }
            ,  { typeof(Guid).GetHashCode(),(text)=>StringConvertExtension.ToGuid(text) }
            ,  { typeof(DateTime).GetHashCode(),(text)=>StringConvertExtension.ToDateTime(text) }

            ,  { typeof(bool?).GetHashCode(),(text)=>StringConvertExtension.ToBoolean(text) }
            ,  { typeof(byte?).GetHashCode(),(text)=>StringConvertExtension.ToByte(text) }
            ,  { typeof(char?).GetHashCode(),(text)=>StringConvertExtension.ToChar(text) }
            ,  { typeof(short?).GetHashCode(),(text)=>StringConvertExtension.ToShort(text) }
            ,  { typeof(ushort?).GetHashCode(),(text)=>StringConvertExtension.ToUShort(text) }
            ,  { typeof(int?).GetHashCode(),(text)=>StringConvertExtension.ToInt(text) }
            ,  { typeof(uint?).GetHashCode(),(text)=>StringConvertExtension.ToUInt(text) }
            ,  { typeof(long?).GetHashCode(),(text)=>StringConvertExtension.ToLong(text) }
            ,  { typeof(ulong?).GetHashCode(),(text)=>StringConvertExtension.ToULong(text) }
            ,  { typeof(float?).GetHashCode(),(text)=>StringConvertExtension.ToFloat(text) }
            ,  { typeof(double?).GetHashCode(),(text)=>StringConvertExtension.ToDouble(text) }
            ,  { typeof(decimal?).GetHashCode(),(text)=>StringConvertExtension.ToDecimal(text) }
            ,  { typeof(Guid?).GetHashCode(),(text)=>StringConvertExtension.ToGuid(text) }
            ,  { typeof(DateTime?).GetHashCode(),(text)=>StringConvertExtension.ToDateTime(text) }
        };

        static readonly SortedDictionary<int, Func<string, object>> _objectConvertors = new SortedDictionary<int, Func<string, object>>() {
            { typeof(bool).GetHashCode(),(text)=>StringConvertExtension.ToBooleanObject(text) }
            ,  { typeof(byte).GetHashCode(),(text)=>StringConvertExtension.ToByteObject(text) }
            ,  { typeof(char).GetHashCode(),(text)=>StringConvertExtension.ToCharObject(text) }
            ,  { typeof(short).GetHashCode(),(text)=>StringConvertExtension.ToShortObject(text) }
            ,  { typeof(ushort).GetHashCode(),(text)=>StringConvertExtension.ToUShortObject(text) }
            ,  { typeof(int).GetHashCode(),(text)=>StringConvertExtension.ToIntObject(text) }
            ,  { typeof(uint).GetHashCode(),(text)=>StringConvertExtension.ToUIntObject(text) }
            ,  { typeof(long).GetHashCode(),(text)=>StringConvertExtension.ToLongObject(text) }
            ,  { typeof(ulong).GetHashCode(),(text)=>StringConvertExtension.ToULongObject(text) }
            ,  { typeof(float).GetHashCode(),(text)=>StringConvertExtension.ToFloatObject(text) }
            ,  { typeof(double).GetHashCode(),(text)=>StringConvertExtension.ToDoubleObject(text) }
            ,  { typeof(decimal).GetHashCode(),(text)=>StringConvertExtension.ToDecimalObject(text) }
            ,  { typeof(Guid).GetHashCode(),(text)=>StringConvertExtension.ToGuidObject(text) }
            ,  { typeof(DateTime).GetHashCode(),(text)=>StringConvertExtension.ToDateTimeObject(text) }

            ,  { typeof(bool?).GetHashCode(),(text)=>StringConvertExtension.ToBoolean(text) }
            ,  { typeof(byte?).GetHashCode(),(text)=>StringConvertExtension.ToByte(text) }
            ,  { typeof(char?).GetHashCode(),(text)=>StringConvertExtension.ToChar(text) }
            ,  { typeof(short?).GetHashCode(),(text)=>StringConvertExtension.ToShort(text) }
            ,  { typeof(ushort?).GetHashCode(),(text)=>StringConvertExtension.ToUShort(text) }
            ,  { typeof(int?).GetHashCode(),(text)=>StringConvertExtension.ToInt(text) }
            ,  { typeof(uint?).GetHashCode(),(text)=>StringConvertExtension.ToUInt(text) }
            ,  { typeof(long?).GetHashCode(),(text)=>StringConvertExtension.ToLong(text) }
            ,  { typeof(ulong?).GetHashCode(),(text)=>StringConvertExtension.ToULong(text) }
            ,  { typeof(float?).GetHashCode(),(text)=>StringConvertExtension.ToFloat(text) }
            ,  { typeof(double?).GetHashCode(),(text)=>StringConvertExtension.ToDouble(text) }
            ,  { typeof(decimal?).GetHashCode(),(text)=>StringConvertExtension.ToDecimal(text) }
            ,  { typeof(Guid?).GetHashCode(),(text)=>StringConvertExtension.ToGuid(text) }
            ,  { typeof(DateTime?).GetHashCode(),(text)=>StringConvertExtension.ToDateTime(text) }
        };

        static readonly SortedDictionary<int, Func<string, object>> _objectEnumConvertors = new SortedDictionary<int, Func<string, object>>();

        

        public static object ConvertTo(this string self, Type type) {
            if (self == null) return null;
            if (type == typeof(string)) return self;
            Func<string, object> convertor = null;

            if (_objectConvertors.TryGetValue(type.GetHashCode(), out convertor)) return convertor(self);
            GlobalLocker.InternalSynchronizingObject.EnterUpgradeableReadLock();
            try {
                if (_objectEnumConvertors.TryGetValue(type.GetHashCode(), out convertor)) return convertor(self);
                GlobalLocker.InternalSynchronizingObject.EnterWriteLock();
                try {
                    if (_objectEnumConvertors.TryGetValue(type.GetHashCode(), out convertor)) return convertor(self);
                    convertor = GenEnumConverter(type);
                    if (convertor != null) {
                        _objectConvertors.Add(type.GetHashCode(),convertor);
                        return convertor(self);
                    }
                    return null;
                } finally {
                    GlobalLocker.InternalSynchronizingObject.ExitWriteLock();
                }
            } finally {
                GlobalLocker.InternalSynchronizingObject.ExitUpgradeableReadLock();
            }
            
            
            //return null;
        }
        static Func<string, object> GenEnumConverter(Type targetType) {
            bool isNullable = targetType.FullName.StartsWith("System.Nullable");
            var type = targetType;
            if (isNullable) type = targetType.GetGenericArguments()[0];
            if (!type.GetTypeInfo().IsEnum) {
                return null;
            }
            
            if (isNullable)
            {
                return GenNullableEnumConverter(type);
            }
            else {
                return (input) => StringConvertExtension.ToEnumObject(input, type);
            }
        }

        static MethodInfo ToEnumObjectMethodInfo = typeof(StringConvertExtension).GetMethod("ToEnumObject");
        static Func<string,object> GenNullableEnumConverter(Type enumType) {
            var parsedValueExpr = Expression.Parameter(enumType,"value");
            var inputExpr = Expression.Parameter(typeof(string),"input");
            var callParseExpr = Expression.Call(ToEnumObjectMethodInfo, inputExpr);
            var assignExpr = Expression.Assign(parsedValueExpr,callParseExpr);
            var nullableType = typeof(Nullable<>).MakeGenericType(enumType);
            var emptyExpr = Expression.New(nullableType);
            ConstructorInfo ctor = null;
            var ctors = nullableType.GetConstructors();
            foreach (var c in ctors) {
                if (c.GetParameters().Length == 1) {
                    ctor = c; break;
                }
            }
            var valueExpr = Expression.New(ctor, parsedValueExpr);
            var condExpr = Expression.Condition(Expression.Equal(parsedValueExpr,Expression.Constant(null)),valueExpr,emptyExpr);

            var labelTarget = Expression.Label(typeof(object));
            var returnValueExpr = Expression.Convert(condExpr, typeof(object));
            var retExpr = Expression.Return(labelTarget, returnValueExpr);
            var labelExpr = Expression.Label(labelTarget, Expression.Constant(null));


            Expression block = Expression.Block(
                new List<ParameterExpression>() { parsedValueExpr }
                , new List<Expression>() { retExpr, labelExpr }
                );
            if (block.CanReduce)
            {
                block = block.ReduceAndCheck();
            }
            var lamda = Expression.Lambda<Func<string,object>>(block,inputExpr);
            return lamda.Compile();
        }
        
        public static Nullable<T> To<T>(this string self) where T : struct {
            var type = typeof(T);
            var typeInfo = typeof(T).GetTypeInfo();
            if (typeInfo.IsEnum) return StringConvertExtension.ToEnum<T>(self);
            var result = StringConvertExtension.ConvertTo(self, typeof(T));
            if (result == null) return null;
            return (Nullable<T>)result;

        }

    }
}
