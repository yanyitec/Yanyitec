using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class StringRegexExtension
    {
        public static bool Like(this string self, string likeExpression) {
            if (self == null) {
                if (string.IsNullOrWhiteSpace(likeExpression)) return true;
                return false;
            }
            if (likeExpression == null) {
                return false;
            }
            var compareString = likeExpression;
            bool start = false;bool end = false;
            if (likeExpression.StartsWith("%")) {
                start = true;
                compareString = likeExpression.Substring(1);
            }
            if (compareString.EndsWith("%")) {
                var len = compareString.Length - 1;
                if (len >= 0) {
                    end = true;
                    compareString = compareString.Substring(0, compareString.Length - 1);
                }                
            }
            if (compareString.Length == 0) return true;
            if (start)
            {
                if (end) return self.Contains(compareString);
                else return self.EndsWith(compareString);
            }
            else if (end)
            {
                return self.StartsWith(compareString);
            }
            else {
                return self == compareString;
            }

        }
    }
}
