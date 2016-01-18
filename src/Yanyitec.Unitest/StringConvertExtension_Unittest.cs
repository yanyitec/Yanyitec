using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Unitest
{
    using Yanyitec.Testing;
    [Testing.Test]
    public class StringConvertExtension_Unittest
    {
        [Test]
        public void ToInt() {
            string input = "123";
            object val = input.ToInt();
            var nullable = (int?)val;
            Assert.IsTypeof(nullable,typeof(Nullable<int>));
            Assert.AreEqual(nullable.HasValue,true);
            Assert.AreEqual(nullable.Value,123);
        }
    }
}
