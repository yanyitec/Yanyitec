﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Hierarchies.Entities
{
    public class DepartmentEntity : Entity<Guid>
    {
        public string Name { get; set; }


        public string Description { get; set; }

        public Guid ParentId { get; set; }
        

        
    }
}
