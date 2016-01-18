using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Injecting
{
    [Flags]
    public enum InjectionKinds
    {
        
        /// <summary>
        /// 单例，每次CreateInstance只会得到一个值。
        /// </summary>
        Singleon = 1<<1,
        
        /// <summary>
        /// 容器，createInstance根本不能使用
        /// </summary>
        Container =1<<2,

        
        
        /// <summary>
        /// 固定值，在Register时就规定好了值(可能由constantValue传入，或者在SetItem时就运行instanceFactory)
        /// 
        /// </summary>
        Constant = 1<<8 ,

        /// <summary>
        /// 每次创建只创建一次
        /// </summary>
        NewOnce = 1 << 9 ,

        
        
        
        /// <summary>
        /// 总是重新创建
        /// </summary>
        AlwaysNew = 1<<10 ,
        /// <summary>
        /// 动态创建.生成的代码不是new T(),而是 dependentItem.CreateInstance();
        /// </summary>
        Create = 1<<11 
        
    }
}
