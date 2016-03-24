using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Hierarchies.Entities
{
    public class MemberEntity : Entity<Guid>
    {
        public Guid? DepartmentId { get; set; }

        

        public Guid? RoleId { get; set; }



        public string Alias { get; set; }

        public Guid UserId { get; set; }
    }
}
