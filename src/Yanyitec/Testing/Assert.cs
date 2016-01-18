


namespace Yanyitec.Testing
{
    using System;
    using System.Reflection;
    public static class Assert
    {
        public static Yanyitec.Logging.ILogger Log = Yanyitec.Logging.ConsoleLogger.Default;

        public static bool ThrowException = true;

        public static void IsTrue(bool value) {
            if(value!=true) throw new AssertException("Expect true but actual is false.");
        }

        public static void IsFalse(bool value)
        {
            if (value != false) throw new AssertException("Expect true but actual is false.");
        }

        public static void IsTypeof(object obj, Type expectType) {
            if (obj == null) throw new AssertException("Expect type[" + expectType.FullName + "] but actual is null.");
            if(obj.GetType() != expectType) throw new AssertException("Expect type[" + expectType.FullName + "] is not match actual type["+obj.GetType().FullName+"].");
        }

        public static void AreEqual(object expect, object actual) {
            if (expect != actual) throw new AssertException("Expect is not match actual." );
        }

        public static void IsNull(object target) {
            if(target!=null) throw new AssertException("Should be null.");
        }

        public static void IsNotNull(object target)
        {
            if (target == null) throw new AssertException("Can not be null.");
        }


        public static void AreEqual<T>(T expect, T actual) {

            if (!expect.Equals(actual)) throw new AreEqualException(expect,actual);
        }

        public static void IsInstanceOfType(object obj, Type type) {
            if (obj == null) throw new InstanceOfTypeException(obj,type);
            if (type!=obj.GetType() && !type.IsAssignableFrom(obj.GetType())) throw new InstanceOfTypeException(obj, type);
        }
    }
}
