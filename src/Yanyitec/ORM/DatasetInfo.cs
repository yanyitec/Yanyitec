using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Yanyitec.ORM
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

    }
}
