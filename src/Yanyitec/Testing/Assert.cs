


namespace Yanyitec.Testing
{
    using System;
    using System.Reflection;
    public static class Assert
    {
        public static Yanyitec.Logging.ILogger Log = Yanyitec.Logging.ConsoleLogger.Default;

        public static bool ThrowException = true;

        public static void True(bool value) {
            if(value!=true) throw new AssertBoolException(true);
        }

        public static void False(bool value)
        {
            if (value != false) throw new AssertBoolException(false);
        }

        
        public static void Equal(object expect, object actual) {
            if (expect != actual) throw new AssertEqualException(expect, actual);
        }

        public static void Null(object target) {
            if(target!=null) throw new AssertNullException(true);
        }

        public static void NotNull(object target)
        {
            if (target == null) throw new AssertNullException(false);
        }


        public static void Equal<T>(T expect, T actual) {

            if (!expect.Equals(actual)) throw new AssertEqualException(expect,actual);
        }

        public static void InstanceOf(object obj, Type type) {
            if (obj == null) throw new AssertInstanceException(obj,type);
            if (type!=obj.GetType() && !type.IsAssignableFrom(obj.GetType())) throw new AssertInstanceException(obj, type);
        }

        public static void InstanceOf<T>(object obj)
        {
            InstanceOf(obj,typeof(T));
        }
    }
}
