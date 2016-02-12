using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public class DealResult
    {
        public DealResult(DateTime reDealTime) {
            this.RetryTime = reDealTime;
        }

        public DealResult(long tick)
        {
            this.RetryTime = DateTime.Now.AddTicks(tick);
        }

        public DealResult(bool isRunning) {
            if (!isRunning) RetryTime = DateTime.MaxValue;
            else RetryTime = DateTime.MinValue;
        }

        public DateTime RetryTime { get; private set; }

        public readonly static DealResult Running = new DealResult(true);

        public readonly static DealResult Suspended = new DealResult(false);

        public static DealResult Delay(long tick) { return new DealResult(tick); }

        public static DealResult DelayTo(DateTime time) { return new DealResult(time); }
    }
}
