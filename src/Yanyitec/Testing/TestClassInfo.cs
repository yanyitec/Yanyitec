using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    using System.Reflection;
    using Yanyitec;
    public class TestClassInfo
    {
        public TestClassInfo(TypeInfo info) {
 
            if (!info.IsPublic || !info.IsClass) return;
            var attr = info.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            if (attr != null) {
                this.IsValid = true;
                this.TestClassType = info;
                this.Description = attr.Description;
            }
            
        }
        public bool IsValid { get; private set; }

        public TypeInfo TestClassType { get; private set; }

        public string Name {
            get { return this.TestClassType.FullName; }
        }

        public string Description { get; private set; }

        Dictionary<string, TestMethodInfo> GetTestMethodInfos() {
            var result = new Dictionary<string, TestMethodInfo>();
            foreach (var method in this.TestClassType.DeclaredMethods) {
                var methodTester = new TestMethodInfo(method);
                if (methodTester.IsValid) {
                    result.Add(methodTester.Name,methodTester);
                }
            }
            return result;
        }

        IReadOnlyDictionary<string, TestMethodInfo> _testMethodInfos;
        public IReadOnlyDictionary<string, TestMethodInfo> TestMethodInfos {
            get {
                if (_testMethodInfos == null) {
                    lock (this) {
                        if (_testMethodInfos == null) {
                            _testMethodInfos = GetTestMethodInfos().AsReadonly();
                        }
                    }
                }
                return _testMethodInfos;
            }
        }

        

        private AssertException Call(TestMethodInfo methodInfo,bool? throwException = null) {
            bool throwEx = throwException.HasValue ? throwException.Value : Assert.ThrowException;
            if (!throwEx)
            {
                try
                {
                    methodInfo.Run();
                    return AssertException.Success;
                }
                catch (AssertException ex)
                {
                    return ex;
                }
            }
            else {
                methodInfo.Run();
                return AssertException.Success;
            }
            
        }
        /// <summary>
        /// 运行类的测试方法
        /// </summary>
        /// <param name="methodName">null 表示所有的方法， 可以用 like表达式</param>
        /// <param name="result">方法名，结果</param>
        /// <returns></returns>
        public IDictionary<string, AssertException> RunMethods(string methodName = null,IDictionary<string,AssertException> result = null,bool? throwException=null) {
            methodName = methodName?.Trim();
            if (result == null) {
                result = new Dictionary<string, AssertException>();
            }
            
            foreach (var methodInfo in this.TestMethodInfos.Values) {
               if(methodName == null || methodInfo.Name.Like(methodName)) result.Add(methodInfo.Name,Call(methodInfo,throwException));
            }
            return result;
        }

        
    }
}
