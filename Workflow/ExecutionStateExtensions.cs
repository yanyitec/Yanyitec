using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow
{
    public static class ExecutionStateExtensions
    {
        public static bool Is(this ExecutionStates self, ExecutionStates target) {
            return ((int)self & (int)target) != 0 || self == target ;
        }

        public static bool IsActived(this ExecutionStates self) {
            return ((int)self & (int)ExecutionStates.Actived) != 0 || self == ExecutionStates.Actived;
        }

        public static bool IsInactive(this ExecutionStates self)
        {
            return ((int)self & (int)ExecutionStates.Inactive) != 0 || self == ExecutionStates.Inactive;
        }
    }
}
