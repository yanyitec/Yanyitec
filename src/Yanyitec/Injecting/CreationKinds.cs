using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    [Flags]
    public enum CreationKinds
    {
        /// <summary>
        /// 会在运行期创建
        /// </summary>
        RuntimeNew = 1,
        /// <summary>
        /// 单例，每次CreateInstance只会得到一个值。
        /// </summary>
        Singleon = 1<<1,
        
        /// <summary>
        /// 容器，createInstance根本不能使用
        /// </summary>
        Group =1<<2,

        /// <summary>
        /// 配置项，会在运行期查找合适的项
        /// </summary>
        Config = 1<<3,
        
        /// <summary>
        /// 固定值，在Register时就规定好了值(可能由constantValue传入，或者在SetItem时就运行instanceFactory)
        /// 
        /// </summary>
        Constant = 1<<8 | Singleon,

        /// <summary>
        /// 每次创建只创建一次
        /// </summary>
        NewOnce = 1 << 9 | RuntimeNew,

        /// <summary>
        /// 全局的只创建一次
        /// </summary>
        NewOnlyOnce = NewOnce | Singleon,
        
        
        /// <summary>
        /// 总是重新创建
        /// </summary>
        AlwaysNew = 1<<10 | RuntimeNew,
        /// <summary>
        /// 动态创建.生成的代码不是new T(),而是 dependentItem.CreateInstance();
        /// </summary>
        Create = 1<<11 | RuntimeNew,
        /// <summary>
        /// 创建代理,从AgencyItem创建
        /// </summary>
        Agency,
    }
}
