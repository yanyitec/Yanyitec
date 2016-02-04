using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class User
    {
        public User(Guid id, string username) {
            this.Id = id;
            this.Username = username;
        }
        public Guid Id { get; private set; }

        public string Username { get;private set; }
    }
}
