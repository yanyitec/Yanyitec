using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public enum HttpMethods
    {
        GET = 1,
        POST = 1<<1,
        PUT = 1<<2,
        DELETE = 1<<3,
        MOVE = 1<<4,
        COPY = 1<<5,
        LOCK = 1<<6,
        MKCOL = 1<<7,
        HEAD = 1 << 8,
        OPTIONS = 1<<9,
        TRACE = 1<<10,
        ALL = GET | POST | PUT | DELETE | MOVE | COPY | LOCK | MKCOL | HEAD | OPTIONS | TRACE
    }
}
