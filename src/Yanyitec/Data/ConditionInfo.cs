using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class ConditionInfo<T>
    {
        public class Table {
            public ReferenceInfo ReferenceInfo { get; set; }
            public string Alias { get; set; }
        }
        public ConditionInfo(Criteria<T> cri, EntityInfo entityInfo) {
            this.EntityInfo = entityInfo;
            if (typeof(T) != entityInfo.EntityType) throw new ArgumentException("EntityInfo's type ["+entityInfo.EntityType.FullName+"] is not T's type " + typeof(T).FullName);
            this.Tables = new JoinTables(entityInfo);
            ParseExpression(cri.Expression, entityInfo, this.Tables, _where);
        }

        public JoinTables Tables { get; private set; }
        
        public EntityInfo EntityInfo { get; private  set; }
        readonly StringBuilder _where = new StringBuilder();

        public string ToSql() {
                return _where.ToString();
        }

        static void ParseExpression(Expression expr, EntityInfo rootEntity, JoinTables tables, StringBuilder where) {
            BinaryExpression binaryExpr = null;
            switch (expr.NodeType) {
                case ExpressionType.And:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append(") AND (");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.OrElse:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append(") OR (");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.Equal:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append("=");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.NotEqual:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append("<>");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.GreaterThan:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append(">");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append(">=");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.LessThan:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append("<");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.LessThanOrEqual:
                    binaryExpr = expr as BinaryExpression;
                    where.Append('(');
                    ParseExpression(binaryExpr.Left, rootEntity, tables, where);
                    where.Append("<=");
                    ParseExpression(binaryExpr.Right, rootEntity, tables, where);
                    where.Append(')');
                    break;
                case ExpressionType.MemberAccess:
                    ParseMemberExpression(expr as MemberExpression, rootEntity, tables, where);
                    break;
            }

            
        }
        static void ParseMemberExpression(MemberExpression expr, EntityInfo rootEntity, JoinTables tables, StringBuilder where)
        {
            var targetExpr = expr.Expression;

            List<PropertyInfo> temps = new List<PropertyInfo>();
            while (true) {
                if (targetExpr.NodeType == ExpressionType.Parameter) {
                    
                    break;
                }
                if (targetExpr.NodeType == ExpressionType.Constant) {
                    ParseConstExpression(targetExpr as ConstantExpression, rootEntity, temps,where);
                    return;
                }
                if (targetExpr.NodeType == ExpressionType.MemberAccess) {
                    var memberExpr = targetExpr as MemberExpression;
                    targetExpr = memberExpr.Expression;
                    var memberInfo = memberExpr.Member as PropertyInfo;
                    temps.Add(memberInfo);
                }
                
                //a.User.Id = 1
            }
            var entityInfo = rootEntity;
            ReferenceInfo rel;
            JoinTable tb = null;
            for (var i = temps.Count - 1; i >= 0; i--)
            {
                rel = entityInfo.GetReference(temps[i].Name);
                if (rel == null) throw new InvalidOperationException(temps[i].Name + " is not defined as reference.");
                tb = tables.GetOrCreate(rel,tb);
                
                entityInfo = tb.ReferenceInfo.ReferenceEntityInfo;
            }

            if (tb == null)
            {
                where.Append("__tmp_table_0");
            }
            else {
                where.Append(tb.Alias);
            }
            var field = entityInfo.GetField(expr.Member.Name);
            if (field == null) throw new ArgumentException(expr.Member.DeclaringType.FullName + "." + expr.Member.Name + " is not defined as field.");
            where.Append('.').Append(field.Fieldname);
            //a.userid == 1

        }

        static void ParseConstExpression(ConstantExpression expr, EntityInfo rootEntity, List<PropertyInfo> props, StringBuilder where) {
            var valueObj = expr.Value;
            for (var i = props.Count - 1; i >= 0; i--) {
                valueObj = props[i].GetValue(valueObj);
            }

            if (valueObj == null) {
                where.Append("NULL"); return;
            }
            var valueType = expr.Type;
            var value = NullableExtension.GetValue(valueObj);
            if (value == null) {
                where.Append("NULL");return;
            }
            valueType = value.GetType();
            if (value is string) {
                where.Append('\'').Append(value.ToString().Replace("'", "''")).Append('\'');
                return;
            }
            if (
                valueType == typeof(int)
                || valueType == typeof(uint)
                || valueType == typeof(byte)
                || valueType == typeof(short)
                || valueType == typeof(ushort)
                || valueType == typeof(long)
                || valueType == typeof(ulong)
                || valueType == typeof(decimal)
                || valueType == typeof(float)
                || valueType == typeof(double)
                )
            {
                where.Append(value); return;
            }
            where.Append('\'').Append(value.ToString()).Append('\'');

        }
    }
}
