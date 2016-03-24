using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Hierarchies.Entities
{
    public class RoleEntity : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid InheritRoleId { get; set; }

        public Guid? DepartmentId { get; set; }
    }
}
