using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    public class EntityDefine<TEntity>
    {
        public EntityDefine(DatasetInfo ds) {
            this.DatasetInfo = ds;
            this.EntityInfo = ds.GetOrCreateEntity(typeof(TEntity));
        }
        public DatasetInfo DatasetInfo { get; set; }

        public EntityInfo EntityInfo { get; set; }
        public EntityDefine<TEntity> Table(string tablename) {
            EntityInfo.Tablename = tablename;
            return this;
        }

        public EntityDefine<TEntity> Field(Expression<Func<TEntity, object>> property, string fieldname=null) {
            var member = property.Body as MemberExpression;
            if (member == null) throw new ArgumentException("property must be member access expression.");
            this.EntityInfo.SetFieldname(member.Member as PropertyInfo,fieldname);
            return this;
        }
        /// <summary>
        /// Role HasOne( Role.Department,Role.DepartmentId);
        /// means-> Department HasOne Role or Department has many role
        /// </summary>
        /// <typeparam name="TForeign"></typeparam>
        /// <param name="property"></param>
        /// <param name="foreignKeyExpression"></param>
        /// <returns></returns>
        public EntityDefine<TEntity> HasOne(Expression<Func<TEntity, object>> referenceExpression, Expression<Func<TEntity,object>> primaryExpression=null) {
            var referenceMember = referenceExpression.Body as MemberExpression;
            var referenceProp = referenceMember.Member as PropertyInfo;
            var referenceEntityInfo = this.DatasetInfo.GetOrCreateEntity(referenceProp.DeclaringType);
            var relation = this.EntityInfo.GetOrCreateReference(referenceEntityInfo, ReferenceKinds.ManyToOne);
            if (primaryExpression != null) {
                var primaryMember = primaryExpression.Body as MemberExpression;
                var primaryProp = referenceMember.Member as PropertyInfo;
                relation.PrimaryField = this.EntityInfo.GetOrCreateField(primaryProp);
            }
            
            return this;
        }

        public EntityDefine<TEntity> HasOne<TReference>(Expression<Func<TEntity, TReference>> referenceExpression, Expression<Func<TReference, object>> referenceFieldExpression = null)
        {
            var referenceMember = referenceExpression.Body as MemberExpression;
            var referenceProp = referenceMember.Member as PropertyInfo;
            var referenceEntityInfo = this.DatasetInfo.GetOrCreateEntity(referenceProp.DeclaringType);
            var relation = this.EntityInfo.GetOrCreateReference(referenceEntityInfo, ReferenceKinds.ManyToOne);
            if (referenceFieldExpression != null)
            {
                var fieldMember = referenceFieldExpression.Body as MemberExpression;
                
                var fieldProp = fieldMember.Member as PropertyInfo;
                relation.ReferenceField = referenceEntityInfo.GetOrCreateField(fieldProp);
            }

            return this;
        }
        /// <summary>
        /// Department.HasMany(d.Roles);
        /// </summary>
        /// <typeparam name="TReference"></typeparam>
        /// <param name="referenceExpression"></param>
        /// <param name="referenceFieldExpression"></param>
        /// <returns></returns>
        public EntityDefine<TEntity> HasMany<TReference>(Expression<Func<TEntity, TReference>> referenceExpression, Expression<Func<TReference, object>> referenceFieldExpression = null)
        {
            var referenceMember = referenceExpression.Body as MemberExpression;
            var referenceProp = referenceMember.Member as PropertyInfo;
            var referenceEntityInfo = this.DatasetInfo.GetOrCreateEntity(referenceProp.DeclaringType);
            var relation = this.EntityInfo.GetOrCreateReference(referenceEntityInfo, ReferenceKinds.OneToMany);
            if (referenceFieldExpression != null)
            {
                var fieldMember = referenceFieldExpression.Body as MemberExpression;

                var fieldProp = fieldMember.Member as PropertyInfo;
                relation.ReferenceField = referenceEntityInfo.GetOrCreateField(fieldProp);
            }

            return this;
        }

        /// <summary>
        /// Department.HasMany(d.Roles);
        /// </summary>
        /// <typeparam name="TReference"></typeparam>
        /// <param name="referenceExpression"></param>
        /// <param name="referenceFieldExpression"></param>
        /// <returns></returns>
        public EntityDefine<TEntity> HasMany<TReference>(Expression<Func<TEntity, TReference>> referenceExpression,string internalTablname, Expression<Func<TReference, object>> primaryFieldExpression, Expression<Func<TReference, object>> referenceFieldExpression)
        {
            var referenceMember = referenceExpression.Body as MemberExpression;
            var referenceProp = referenceMember.Member as PropertyInfo;
            var referenceEntityInfo = this.DatasetInfo.GetOrCreateEntity(referenceProp.DeclaringType);
            var relation = this.EntityInfo.GetOrCreateReference(referenceEntityInfo, ReferenceKinds.ManyToMany);
            var referenceFieldMember = referenceFieldExpression.Body as MemberExpression;
            var referenceFieldProp = referenceFieldMember.Member as PropertyInfo;
            var primaryFieldMember = primaryFieldExpression.Body as MemberExpression;
            var primaryFieldProp = primaryFieldMember.Member as PropertyInfo;
            relation.PrimaryField = this.EntityInfo.GetOrCreateField(primaryFieldProp);
            relation.ReferenceField = referenceEntityInfo.GetOrCreateField(referenceFieldProp);
            relation.InternalTablename = internalTablname;
            return this;
        }
    }
}
