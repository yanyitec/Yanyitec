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

        Action _methodInvoker;

        public void Run() {
            if (_methodInvoker == null) {
                lock (this) {
                    if (_methodInvoker == null) {
                        _methodInvoker = GenInvoker();
                    }
                }
            }
            _methodInvoker();
        }

        private Action GenInvoker() {
            var instanceExpr = Expression.New(this.MethodInfo.DeclaringType);
            var callExpr = Expression.Call(instanceExpr, this.MethodInfo);
            var lamda = Expression.Lambda<Action>(callExpr);
            return lamda.Compile();
        }
    }
}
