namespace Yanyitec.Unitest
{
    using System;
    using Yanyitec.Testing;
    [Testing.Test]
    public class StringConvertExtension_Unittest
    {
        [Test]
        public void ToInt() {
            string input = "123";
            object val = input.ToInt();
            var nullable = (int?)val;
            Assert.InstanceOf(nullable,typeof(Nullable<int>));
            Assert.True(nullable.HasValue);
            Assert.Equal("123",nullable.Value);
        }
    }
}
