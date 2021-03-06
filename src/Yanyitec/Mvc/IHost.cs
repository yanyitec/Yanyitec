﻿using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    public interface IHost
    {
        IStorage SharePath { get;  }

        IStorage ModulePath { get;  }

        IStorage DataPath { get;  }


        IRWLocker SynchronizingObject { get;  }

    }
}
