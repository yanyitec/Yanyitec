using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    public class EntityInfo
    {
        public EntityInfo(Type entityType) {
            this.EntityType = entityType;
        }
        public Type EntityType { get; private set; }

        public string Tablename { get; internal protected set; }

        readonly SortedDictionary<string,FieldInfo> _fields = new SortedDictionary<string, FieldInfo>();
        public IReadOnlyDictionary<string,FieldInfo> Fields { get { return _fields.AsReadonly(); } }

        public FieldInfo GetOrCreateField(PropertyInfo prop)
        {
            if (prop.DeclaringType != this.EntityType) throw new ArgumentException("prop 's declare type must be " + this.EntityType.FullName);
            foreach (var fieldInfo in this._fields.Values)
            {
                if (fieldInfo.PropertyInfo == prop) return fieldInfo;
            }
            var result = new FieldInfo(prop);
            this._fields.Add(prop.Name, result);
            return result;
        }
        public void SetFieldname(PropertyInfo prop, string fieldname) {
            var field = this.GetOrCreateField(prop);
            if (fieldname == null) field.Fieldname = prop.Name;
            else field.Fieldname = fieldname;
        }

        readonly List<ReferenceInfo> _references = new List<ReferenceInfo>();
        public IReadOnlyList<ReferenceInfo> References { get { return _references; } }

        public ReferenceInfo GetReference(string name) {
            foreach (var rel in _references) if (rel.Name == name) return rel;
            return null;
        }

        public ReferenceInfo GetReference(EntityInfo referenceEntity, ReferenceKinds kind)
        {
            foreach (var rel in _references)
            {
                if (rel.ReferenceEntityInfo == referenceEntity)
                {
                    if (((int)rel.Kind & (int)kind) > 0) return rel;
                }
            }

            return null;
        }

        public ReferenceInfo GetOrCreateReference(string name, EntityInfo referenceEntity,ReferenceKinds kind) {
            var result = this.GetReference(referenceEntity,kind);
            if (result == null) {
                result = new ReferenceInfo(name, this, referenceEntity, kind);
                this._references.Add(result);
            }
            
            return result;
        }
        
    }
}
