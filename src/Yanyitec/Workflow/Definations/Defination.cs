using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Workflow.Definations
{
    public class Defination
    {
        public Guid Id { get; set; }

        public int RuntimeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IDictionary<string, string> Extras { get; set; }
    }
}
