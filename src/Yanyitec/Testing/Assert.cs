using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    public static class Assert
    {
        public static Yanyitec.Logging.ILogger Log = Yanyitec.Logging.ConsoleLogger.Default;

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
            
            if (!expect.Equals(actual)) throw new AssertException("Expect is not match actual.");
        }
    }
}
