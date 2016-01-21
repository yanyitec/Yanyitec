using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Users
{
    public class RuntimeUser
    {
        public Guid Id { get;private set; }

        public string UniqueName { get; private set; }
        public int RuntimeId { get; private set; }
        public string SessionId { get;private set; }

        static string GenSessionId(int rtId, Guid id, string userName) {
            return null;

        }
    }
}
