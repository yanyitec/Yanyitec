using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class ActivityInfo
    {
        public ActivityInfo()
        {
        }
        public ActivityInfo(string alias,Func<Activity, object> dealDelegate) {
            this.DealDelegate = dealDelegate;
            this.StartMode = ExecutionModes.Manual;
            this.FinishMode = ExecutionModes.Automatic;
            this.Alias = alias;
        }
        public ActivityInfo(Func<Activity, object> dealDelegate):this(null,dealDelegate)
        {
            
        }



        public string Alias { get; set; }
        /// <summary>
        /// 开始模式，
        /// 如果是自动开始，引擎会自动检测StartConstraint条件
        /// 如果是手工开始，引擎不会自己开始该activity,需要client代码自己调用start函数
        /// </summary>
        public ExecutionModes StartMode { get; set; }
        /// <summary>
        /// 开始条件
        /// 只有满足前置条件才能进去
        /// </summary>
        public Func<Activity, bool> StartConstraint { get; set; }

        /// <summary>
        /// 结束条件
        /// 只有满足后置条件该Activity才能结束
        /// </summary>
        public Func<Activity, bool> FinishConstraint { get; set; }

        /// <summary>
        /// 结束模式，
        /// 如果是自动结束，引擎会自动检测FinishConstraint条件
        /// 如果是手工结束，引擎不会自己结束该activity,需要client代码自己调用finish函数
        /// </summary>
        public ExecutionModes FinishMode { get; set; }

        public Func<Activity, object> DealDelegate { get; set; }

        public static implicit operator ActivityInfo(Func<Activity, object> dealDelegate) {
            return new ActivityInfo(dealDelegate);
        }
    }
}
