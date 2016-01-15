using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public interface IConvert
    {
        T Parse<T>(string jsonString);
        string ToJson<T>(T obj);
    }
}
