using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec
{
    public class CriteriaResult<T> : IEnumerable<T>
    {
        public Criteria<T> Criteria { get; set; }

        public long Count { get; set; }



        public IList<T> Result { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return Result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Result.GetEnumerator();
        }
    }
}
