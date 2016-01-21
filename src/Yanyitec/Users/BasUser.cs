using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class BasUser
    {
        public Guid Id { get; private set; }

        public string UniqueName { get; private set; }
    }
}
