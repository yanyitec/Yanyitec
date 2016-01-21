using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Unitest.Mocks
{
    using Yanyitec.Injecting;
    [Injecting.Injectable]
    public class DbContext
    {
        [Injecting.Injectable]
        public DbContext(string connname) { this.ConnName = connname; }

        public string ConnName { get; private set; }
        [Injecting.Injectable]
        public int? MaxConnecionCount { get; set; }
    }
    [Injecting.Injectable]
    public class UserProvider
    {
        public DbContext DbContext { get; set; }


    }
    [Injecting.Injectable]
    public class UserBusiness
    {
        public UserProvider DbProvider { get; set; }

        [Injectable]
        public string BusinessId { get; set; }

    }

    public class UserController
    {
        public UserBusiness Business { get; set; }

        [Injectable]
        public HttpMethods AcceptMethod { get; set; }
    }

    
}
