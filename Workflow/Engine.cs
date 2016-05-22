using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class Engine
    {
        public IRepository Repository { get; private set; }
        public Guid CreateActivity(ActivityInfo info) { return Guid.Empty; }
        /// <summary>
        /// 根据定义创建一个活动
        /// </summary>
        /// <param name="definationId"></param>
        /// <returns></returns>
        public Guid CreateActivity(Guid definationId) { return Guid.Empty; }

        public object CreateProcess(Guid activitydefinationId,string desciption) {

            var processId = this.Repository.CreateProcess(activitydefinationId, desciption);
            return null;
        }

        public object ExecuteProcess(Guid processId) { return null; }

        public IActivity ExecuteActivity(Guid activityId) { return null; }

        public IList<Guid> ListProcessIds() {
            return null;
        }

        public Activity LoadActivity(Guid activityId, Guid processId) {

        }

    }
}
