using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.SSO
{
    public class OnlineUser: User
    {
        Dictionary<string, string> _data;
        public OnlineUser(Guid uid,string uname):base(uid,uname) {

        }

        public string SessionId { get; private set; }

        public string this[string key] {
            get {lock(this) return _data[key]; }
            set { lock(this) _data[key] = value; }
        }

        
    }
}
