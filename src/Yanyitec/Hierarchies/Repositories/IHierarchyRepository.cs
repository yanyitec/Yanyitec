using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Hierarchies.Repositories
{
    using System.Linq.Expressions;
    using Yanyitec.Hierarchies.Entities;
    public interface IHierarchyRepository
    {
        DepartmentEntity GetDepartmentById(Guid deptId);

        IList<DepartmentEntity> ListDepartments(Expression<Func<DepartmentEntity, bool>> condition);

        bool AddDepartment(DepartmentEntity entity);

        bool DeleteDepartment(Guid deptId);

        int DeleteDepartments(Expression<Func<DepartmentEntity, bool>> condition);

        bool SaveDepartment(DepartmentEntity entity);

        int UpdateDepartments(string members ,DepartmentEntity value, Expression<Func<DepartmentEntity, bool>> condition);
    }
}
