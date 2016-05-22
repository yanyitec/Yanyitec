using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public interface IDbReader:IDisposable
    {
        object this[string key]{get;}
        object this[int index] { get; }

        bool Read();

        bool Next();

    }
}
