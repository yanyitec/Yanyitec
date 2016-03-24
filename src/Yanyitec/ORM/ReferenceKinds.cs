using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    [Flags]
    public enum ReferenceKinds
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany
    }
}
