using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Runtime
{
    /// <summary>
    /// 源代码类型
    /// </summary>
    [Flags]
    public enum SourceKinds
    {
        Static =1,
        CSharpCode = 1<<2,

        Controller = 1<<16 | CSharpCode,
        Language = 1<<32
    }
}
