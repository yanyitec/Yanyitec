using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class Entity<T>
    {
        public T Id { get; protected set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }

        public Guid ModifiedBy { get; set; }

        public EntityStates Status { get; set; }

        public void SetAssignableId(T newId)
        {
            this.Id = newId;
        }

        public void SetStatus(User user, EntityStates status) {
            if (status == EntityStates.Created)
            {
                this.CreatedAt = DateTime.Now;
                this.CreatedBy = user.Id;
            }
            else {
                this.ModifiedAt = DateTime.Now;
                this.ModifiedBy = user.Id;
            }
            this.Status = status;
        }
    }
}
