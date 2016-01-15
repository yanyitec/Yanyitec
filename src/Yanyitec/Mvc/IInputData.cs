using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    public interface IInputData
    {
        string this[string key] { get; }
        /// <summary>
        /// 根据类型得到上下文相关变量的值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object GetContextValue(Type type);

        HttpMethods? HttpMethod { get; }

    }
}
