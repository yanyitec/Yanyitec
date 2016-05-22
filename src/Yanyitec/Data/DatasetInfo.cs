using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Yanyitec.Data
{
    public class DatasetInfo
    {
        public DatasetInfo() {

        }

        
        readonly SortedDictionary<int, EntityInfo> _entities = new SortedDictionary<int, EntityInfo>();
        public IReadOnlyDictionary<int,EntityInfo> Entities { get { return _entities.AsReadonly(); } }

        

        internal EntityInfo GetOrCreateEntityInfo(Type entityType) {
            EntityInfo result=null;
            if (_entities.TryGetValue(entityType.GetHashCode(), out result)) return result;
            _entities.Add(entityType.GetHashCode(), result = new EntityInfo(entityType));
            return result;
        }

        public EntityInfo GetEntityInfo(int typeid) {
            EntityInfo result = null;
            if (_entities.TryGetValue(typeid, out result)) return result;
            return null;
        }

        public EntityDefine<T> Define<T>() {
            return new EntityDefine<T>(this);
        }

        public IRepository<TEnity, TPrimary> CreateRepository<TEnity, TPrimary>() {
            return null;
        }

        void Initialize(DbCommandBuilderFactory dbHelpper)
        {

            foreach (var entityInfo in this._entities.Values) {
                var props = entityInfo.EntityType.GetProperties();
                foreach (var prop in props)
                {
                    InitializeProperty(entityInfo, prop, dbHelpper);
                }
                if (entityInfo.PrimaryFieldInfo == null) {
                    entityInfo.PrimaryFieldInfo = entityInfo.TryGetField("Id","Id",entityInfo.EntityType.Name + "Id" ,"id");
                    if (entityInfo.PrimaryFieldInfo == null) throw new InvalidOperationException(entityInfo.EntityType.FullName + " is not define primary key");
                }
                InitializeReferences(entityInfo);
            }
        }

        static void InitializeProperty(EntityInfo entityInfo, PropertyInfo prop, DbCommandBuilderFactory helper) {
            var attr = prop.GetCustomAttribute(typeof(NonFieldAttribute));
            if (attr != null) return;
            if (prop.PropertyType.IsByRef) return;
            var field = entityInfo.GetOrCreateField(prop);
            if (prop.PropertyType.FullName.StartsWith("System.Nullable`"))
            {
                field.IsNullable = true;
                field.FieldType = prop.PropertyType.GetGenericArguments()[0];
            }
            else {
                field.FieldType = prop.PropertyType;
            }
        }

        static void InitializeReferences(EntityInfo entityInfo) {
            foreach (var rel in entityInfo.References) {

            }
        }
    }
}
