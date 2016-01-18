using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    public class ModuleBuilder
    {
        public IList<string> CodePaths { get; private set; }

        public virtual IModule CreateModule() {
            return null;
        }
    }
}
