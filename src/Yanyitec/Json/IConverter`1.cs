using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public interface IConverter<T> : IConverter
    {
        string ToJson(T obj);

        T Parse(string jsonString);
    }
}
