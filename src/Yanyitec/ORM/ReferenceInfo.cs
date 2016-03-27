using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    public class ReferenceInfo
    {
        public ReferenceInfo(string name, EntityInfo primary, EntityInfo referenceEntity, ReferenceKinds kind) {
            this.PrimaryEntityInfo = primary;
            this.ReferenceEntityInfo = referenceEntity;
            this.Kind = kind;
            this.Name = name;
        }

        public string Name { get; private set; }
        public EntityInfo PrimaryEntityInfo { get;private set; }

        public EntityInfo ReferenceEntityInfo { get; private set; }

        public FieldInfo PrimaryField { get; set; }

        public FieldInfo ReferenceField { get; set; }
        public string InternalTablename { get; set; }

        public ReferenceKinds Kind { get; internal protected set; }
    }
}
