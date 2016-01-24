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

        

        
        /// <summary>
        /// 运行类的测试方法
        /// </summary>
        /// <param name="methodName">null 表示所有的方法， 可以用 like表达式</param>
        /// <param name="result">方法名，结果</param>
        /// <returns></returns>
        public void RunMethods(string methodName = null) {
            methodName = methodName?.Trim();
            
            foreach (var methodInfo in this.TestMethodInfos.Values) {
                if (methodName == null || methodInfo.Name.Like(methodName))
                {
                    methodInfo.Run();

                }
            }
        }

        public async Task<IDictionary<string, AssertException>> RunMethodsAsync(string methodName = null) {
            var tasks = new List<Task>();
            Dictionary<string, AssertException> result = new Dictionary<string, AssertException>();
            this.RunMethodsInTask(tasks,result,  methodName);
            await Task.WhenAll(tasks.ToArray());
            return result;
        }

        internal protected void RunMethodsInTask(IList<Task> tasks,IDictionary<string,AssertException> result, string methodName) {
            methodName = methodName?.Trim();

            foreach (var methodInfo in this.TestMethodInfos.Values)
            {
                if (methodName == null || methodInfo.Name.Like(methodName))
                {
                    var task = methodInfo.RunAsync(result);
                    tasks.Add(task);
                    task.Start();
                }
            }
        }
        

    }
}
