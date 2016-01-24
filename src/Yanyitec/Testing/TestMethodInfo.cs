using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    public class TestMethodInfo
    {
        public TestMethodInfo(MethodInfo info) {
            if (!info.IsPublic) return;
            var attr = info.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            if (attr == null) return;
            var argCount = info.GetParameters().Length;
            if (argCount != 0) return;

            this.IsValid = true;
            this.Description = attr.Description;
            this.MethodInfo = info;
        }

        public bool IsValid { get; private set; }

        public string Name
        {
            get { return this.MethodInfo.Name; }
        }

        public string Description { get; private set; }

        public MethodInfo MethodInfo { get; private set; }

        bool _hasInvoker;
        Action _methodInvoker;
        Func<Task> _methodAsyncInvoker;

        static MethodInfo TaskRunMethodInfo = typeof(Task).GetMethods().FirstOrDefault(p=>p.Name == "RunSynchronously" && p.GetParameters().Length==0);

        private void GetOrCreateInvoker() {
            if (!_hasInvoker)
            {
                lock (this)
                {
                    if (!_hasInvoker)
                    {
                        var dele = GenInvoker();
                        if (this.MethodInfo.ReturnType == typeof(Task))
                        {
                            _methodAsyncInvoker = dele as Func<Task>;
                        }
                        else _methodInvoker = dele as Action;
                        _hasInvoker = true;
                    }
                }
            }
        }
        public void Run() {
            this.GetOrCreateInvoker();
            if (_methodInvoker != null) { _methodInvoker(); }
            else _methodAsyncInvoker().Wait();
            return;
        }
        public async Task RunAsync(IDictionary<string,AssertException> result) {
            await Task.Run(async () => {
                try
                {
                    if (_methodInvoker != null) { _methodInvoker(); return; }
                    else await _methodAsyncInvoker();
                }
                catch (Exception ex)
                {
                    lock (result)
                    {
                        var assertExcepition = ex as AssertException;
                        if (assertExcepition == null) {
                            assertExcepition = new AssertUnhandledException(ex);
                        }
                        result.Add(this.MethodInfo.Name, assertExcepition);
                    }
                }

            });
            
        }

        private Delegate GenInvoker() {
            
            var instanceExpr = Expression.New(this.MethodInfo.DeclaringType);
            var callExpr = Expression.Call(instanceExpr, this.MethodInfo);
            if (this.MethodInfo.ReturnType != typeof(Task))
            {
                var lamda = Expression.Lambda<Action>(callExpr);
                return lamda.Compile();
            }
            else {
                var lamda = Expression.Lambda<Func<Task>>(callExpr);
                return lamda.Compile();
            }
            
        }
    }
}
