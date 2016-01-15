
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    using System;
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

        public static char? ToChar(this string self)
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

        public static short? ToShort(this string self)
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

        

        public static int? ToInt(this string self) {
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

        public static long? ToLong(this string self)
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

        public static double? ToDouble(this string self)
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

        public static decimal? ToDecimal(this string self)
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

        public static DateTime? ToDateTime(this string self)
        {
            DateTime result = DateTime.MinValue;
            if (DateTime.TryParse(self, out result)) return result;
            long tick = 0;
            if (long.TryParse(self, out tick)) return new DateTime(tick);
            return null;
        }

        public static object ToEnum(this string self, Type enumType) {
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

        static readonly SortedDictionary<int, Func<string, object>> convertors = new SortedDictionary<int, Func<string,object>>() {
            { typeof(bool).GetHashCode(),(text)=>StringConvertExtension.ToBoolean(text) }
            ,  { typeof(bool).GetHashCode(),(text)=>StringConvertExtension.ToBoolean(text) }
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

        public static object ConvertTo(this string self, Type type,bool includeEnum=true) {
            Func<string, object> convertor = null;

            if (convertors.TryGetValue(type.GetHashCode(), out convertor)) return convertor(self);

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum) return StringConvertExtension.ToEnum(self, type);
            if (includeEnum) {
                if (type.FullName.IndexOf("System.Nullable") == 0)
                {
                    var innerType = type.GetGenericArguments()[0];
                    typeInfo = innerType.GetTypeInfo();
                    if (typeInfo.IsEnum) return StringConvertExtension.ToEnum(self,innerType);
                }
            }
            
            return null;
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
