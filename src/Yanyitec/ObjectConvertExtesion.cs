using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class ObjectConvertExtesion
    {
        public static int? ToInt(this object self) {
            if (self == null) return null;
            var t = self.GetType();
            if (t == typeof(bool)) return (bool)self ? 1 : 0;
            if (t == typeof(bool?)) {
                bool? boolValue = (bool?)self;
                return boolValue.HasValue ?new Nullable<int>(boolValue.Value ? 1 : 0) : null;
            }
            if (t == typeof(char)) return (int)self;
            if (t == typeof(char?)) return (int?)self;
            if (t == typeof(byte)) return (int)self;
            if (t == typeof(byte?)) return (int?)self;
            if (t == typeof(short)) return (int)self;
            if (t == typeof(short?)) return (int?)self;
            if (t == typeof(ushort)) return (int)self;
            if (t == typeof(ushort?)) return (int?)self;
            if (t == typeof(int)) return (int)self;
            if (t == typeof(int?)) return (int?)self;
            if (t == typeof(uint)) return (int)self;
            if (t == typeof(uint?)) return (int?)self;
            if (t == typeof(long)) return (int)(long)self;
            if (t == typeof(long?)) return (int?)(long?)self;
            if (t == typeof(ulong)) return (int)(long)self;
            if (t == typeof(ulong?)) return (int?)(long?)self;
            if (t == typeof(double)) return (int)(double)self;
            if (t == typeof(double?)) return (int?)(double?)self;
            if (t == typeof(float)) return (int)(float)self;
            if (t == typeof(float?)) return (int?)(float?)self;
            if (t == typeof(decimal)) return (int)(decimal)self;
            if (t == typeof(decimal?)) return (int?)(decimal?)self;
            if (t == typeof(DateTime)) return (int)((DateTime)self).Ticks;
            if (t == typeof(DateTime?))
            {
                DateTime? boolValue = (DateTime?)self;
                return boolValue.HasValue ? new Nullable<int>((int)boolValue.Value.Ticks) : null;
            }
            int result = 0;
            if (int.TryParse(self.ToString(), out result)) return result;
            return null;
        }

        public static uint? ToUInt(this object self)
        {
            if (self == null) return null;
            var t = self.GetType();
            if (t == typeof(bool)) return (uint)((bool)self ? 1 : 0);
            if (t == typeof(bool?))
            {
                bool? boolValue = (bool?)self;
                return boolValue.HasValue ? new Nullable<uint>((uint)(boolValue.Value ? 1 : 0)) : null;
            }
            if (t == typeof(char)) return (uint)(char)self;
            if (t == typeof(char?)) return (uint?)(char?)self;
            if (t == typeof(byte)) return (uint)(byte)self;
            if (t == typeof(byte?)) return (uint)(byte?)self;
            if (t == typeof(short)) return (uint)self;
            if (t == typeof(short?)) return (uint?)(short?)self;
            if (t == typeof(short)) return (uint)(ushort)self;
            if (t == typeof(short?)) return (uint?)(ushort?)self;
            if (t == typeof(int)) return (uint)self;
            if (t == typeof(int?)) return (uint?)self;
            if (t == typeof(uint)) return (uint)self;
            if (t == typeof(uint?)) return (uint?)self;
            if (t == typeof(long)) return (uint)(long)self;
            if (t == typeof(long?)) return (uint?)(long?)self;
            if (t == typeof(ulong)) return (uint)(ulong)self;
            if (t == typeof(ulong?)) return (uint?)(ulong?)self;
            if (t == typeof(double)) return (uint)(double)self;
            if (t == typeof(double?)) return (uint?)(double?)self;
            if (t == typeof(float)) return (uint)(float)self;
            if (t == typeof(float?)) return (uint?)(float?)self;
            if (t == typeof(decimal)) return (uint)(decimal)self;
            if (t == typeof(decimal?)) return (uint?)(decimal?)self;
            if (t == typeof(DateTime)) return (uint)((DateTime)self).Ticks;
            if (t == typeof(DateTime?))
            {
                DateTime? boolValue = (DateTime?)self;
                return boolValue.HasValue ? new Nullable<uint>((uint)boolValue.Value.Ticks) : null;
            }
            uint result = 0;
            if (uint.TryParse(self.ToString(), out result)) return result;
            return null;
        }

        public static long? ToLong(this object self)
        {
            if (self == null) return null;
            var t = self.GetType();
            if (t == typeof(bool)) return (bool)self ? 1 : 0;
            if (t == typeof(bool?))
            {
                bool? boolValue = (bool?)self;
                return boolValue.HasValue ? new Nullable<long>(boolValue.Value ? 1 : 0) : null;
            }
            if (t == typeof(char)) return (long)self;
            if (t == typeof(char?)) return (long?)self;
            if (t == typeof(byte)) return (long)self;
            if (t == typeof(byte?)) return (long?)self;
            if (t == typeof(short)) return (long)self;
            if (t == typeof(short?)) return (long?)self;
            if (t == typeof(ushort)) return (long)self;
            if (t == typeof(ushort?)) return (long?)self;
            if (t == typeof(int)) return (long)self;
            if (t == typeof(int?)) return (long?)self;
            if (t == typeof(uint)) return (long)self;
            if (t == typeof(uint?)) return (long?)self;
            if (t == typeof(long)) return (long)self;
            if (t == typeof(long?)) return (long?)self;
            if (t == typeof(ulong)) return (long)self;
            if (t == typeof(ulong?)) return (long?)self;
            if (t == typeof(double)) return (long)(double)self;
            if (t == typeof(double?)) return (long?)(double?)self;
            if (t == typeof(float)) return (long)(float)self;
            if (t == typeof(float?)) return (long?)(float?)self;
            if (t == typeof(decimal)) return (long)(decimal)self;
            if (t == typeof(decimal?)) return (long?)(decimal?)self;
            if (t == typeof(DateTime)) return ((DateTime)self).Ticks;
            if (t == typeof(DateTime?))
            {
                DateTime? boolValue = (DateTime?)self;
                return boolValue.HasValue ? new Nullable<long>(boolValue.Value.Ticks) : null;
            }
            long result = 0;
            if (long.TryParse(self.ToString(), out result)) return result;
            return null;
        }
    }
}
