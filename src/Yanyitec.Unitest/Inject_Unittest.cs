using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Unitest
{
    using Yanyitec.Injecting;
    using Testing;
    using Yanyitec.Unitest.Mocks;

    [Test]
    public class Inject_Unittest
    {
        [Test]
        public void Basic()
        {
            var container = new Injection(null, null);
            var connstr = "my database connection string";
            container.Register("connname", connstr);
            container.Register("MaxConnecionCount", "5");
            container.Register("AcceptMethod", "GET");
            container.Register<DbContext>();
            container.Register<UserProvider>();
            container.Register<UserBusiness>();
            container.Register<UserController>();
            var item = container.Find(typeof(UserController));
            Assert.NotNull(item);
            var controller = item.GetOrCreateInstance() as UserController;
            
            Assert.NotNull(controller);
            Assert.NotNull(controller.Business);
            Assert.NotNull(controller.Business.DbProvider);
            Assert.NotNull(controller.Business.DbProvider.DbContext);
            Assert.Equal(connstr,controller.Business.DbProvider.DbContext.ConnName);
            Assert.Equal(5,controller.Business.DbProvider.DbContext.MaxConnecionCount);
            Assert.Equal(HttpMethods.GET,controller.AcceptMethod);
            Assert.Null(controller.Business.BusinessId);
        }
    }
}
