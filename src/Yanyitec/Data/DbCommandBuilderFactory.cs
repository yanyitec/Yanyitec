using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class DbCommandBuilderFactory
    {
        public virtual string Escape(string field) {
            return "[" + field + "]";
        }
        public virtual object DbNull { get; set; }

        static List<FieldInfo> GetFieldInfos(EntityInfo entityInfo, string fields)
        {
            List<FieldInfo> infos = new List<FieldInfo>();
            if (fields == null)
            {
                foreach (var fieldInfo in entityInfo.Fields.Values)
                {
                    infos.Add(fieldInfo);
                }
            }
            else
            {
                var fieldstrs = fields.Split(',');
                foreach (var fieldname in fieldstrs)
                {
                    FieldInfo fieldInfo = null;
                    if (!entityInfo.Fields.TryGetValue(fieldname, out fieldInfo)) throw new ArgumentException("Cannot find " + fieldname + " in " + entityInfo.EntityType.FullName);
                    infos.Add(fieldInfo);
                }
            }
            return infos;
        }
        public readonly static Type IDbCommandType = typeof(IDbCommand);
        readonly static MethodInfo SetParameterMethodInfo = IDbCommandType.GetMethod("SetParameter");
        readonly static MethodInfo ExecuteNonQueryMethodInfo = IDbCommandType.GetMethod("ExecuteNonQuery");
        readonly static PropertyInfo CommandTextPropertyInfo = IDbCommandType.GetProperty("CommandText");
        readonly static MethodInfo GetReaderItemMethodInfo = typeof(IDbReader).GetMethod("get_Item", new Type[] { typeof(int) });
        public virtual Action<IDbCommand, TEntity> GenInsertBuilder<TEntity>(EntityInfo entityInfo,string fields = null)
        {
            var infos = GetFieldInfos(entityInfo, fields);
            var fieldnames = string.Empty;
            var values = string.Empty;
            var cmdExpr = Expression.Parameter(typeof(IDbCommand), "cmd");
            var entityExpr = Expression.Parameter(typeof(TEntity),"entity");
            IList<Expression> exprs = new List<Expression>();
            foreach (var info in infos)
            {
                if (fieldnames != string.Empty)
                {
                    fieldnames += ","; values += ",";
                }
                fieldnames += this.Escape(info.Fieldname);
                var paramname = "@" + info.Fieldname;
                values += paramname;
                var memberExpr = Expression.Property(entityExpr,info.PropertyInfo);
                exprs.Add(Expression.Call(cmdExpr, SetParameterMethodInfo,Expression.Constant(paramname),memberExpr));
            }
            var sql = "INSERT INTO " + this.Escape(entityInfo.Tablename) + "(" + fieldnames + ") VALUES (" + values + ")";
            exprs.Add(Expression.Assign(Expression.Property(cmdExpr , CommandTextPropertyInfo),Expression.Constant(sql)));
                        

            Expression block = Expression.Block(exprs);
            if (block.CanReduce)
            {
                block = block.ReduceAndCheck();
            }
            var lamda = Expression.Lambda<Action<IDbCommand, TEntity>>(block);

            var result = lamda.Compile();
            return result;

        }

        public virtual Action<IDbCommand, TEntity> GenUpdateBuilder<TEntity>(EntityInfo entityInfo, string fields = null)
        {
            var infos = GetFieldInfos(entityInfo, fields);
            var sets = string.Empty;
            
            var cmdExpr = Expression.Parameter(typeof(IDbCommand), "cmd");
            var entityExpr = Expression.Parameter(typeof(TEntity), "entity");
            IList<Expression> exprs = new List<Expression>();
            foreach (var info in infos)
            {
                if (sets != string.Empty)
                {
                    sets += ","; 
                }
                sets += this.Escape(info.Fieldname);
                var paramname = "@" + info.Fieldname;
                sets += "=" + paramname;
                var memberExpr = Expression.Property(entityExpr, info.PropertyInfo);
                exprs.Add(Expression.Call(cmdExpr, SetParameterMethodInfo, Expression.Constant(paramname), memberExpr));
            }
            var sql = "UPDATE " + this.Escape(entityInfo.Tablename) + " SET " + sets;
            exprs.Add(Expression.Assign(Expression.Property(cmdExpr, CommandTextPropertyInfo), Expression.Constant(sql)));


            Expression block = Expression.Block(exprs);
            if (block.CanReduce)
            {
                block = block.ReduceAndCheck();
            }
            var lamda = Expression.Lambda<Action<IDbCommand, TEntity>>(block);

            var result = lamda.Compile();
            return result;

        }

        public virtual SelectInfo<TEntity> GenSelectInfo<TEntity>(EntityInfo entityInfo,string fields,string tableAlias) {
            var infos = GetFieldInfos(entityInfo, fields);
            var fieldnames = string.Empty;
            var tbAlias = tableAlias;
            if (!string.IsNullOrEmpty(tbAlias))tbAlias = this.Escape(tbAlias);
            var readerExpr = Expression.Parameter(typeof(IDbReader),"reader");
            var startExpr = Expression.Parameter(typeof(int),"start");
            var entityExpr = Expression.Parameter(typeof(TEntity));
            var idValueExpr = Expression.Parameter(typeof(object));

            var labelTarget = Expression.Label(typeof(TEntity));
            List<Expression> fillEntityCodes = new List<Expression>();
            var fillEntityExpr = Expression.Block(new List<ParameterExpression>() { entityExpr }, fillEntityCodes);
            var instExpr = Expression.Assign(entityExpr,Expression.New(typeof(TEntity)));
            fillEntityCodes.Add(instExpr);
            foreach (var info in infos)
            {
                if (fieldnames != string.Empty) fieldnames += ",";
                var fname = this.Escape(info.Fieldname);
                if (!string.IsNullOrEmpty(tbAlias)) fieldnames += tbAlias + "." + fname + " AS " + tbAlias + fname;
                else fieldnames += fname;
                if (info.IsNullable) {
                    var valExpr = Expression.Call(readerExpr, GetReaderItemMethodInfo, Expression.Increment(startExpr));
                    var nullExpr = Expression.Constant(this.DbNull);
                    var condExpr = Expression.NotEqual(valExpr, nullExpr);

                    var right = Expression.Call(readerExpr, GetReaderItemMethodInfo, startExpr);
                    var left = Expression.Property(entityExpr, info.PropertyInfo);
                    var assignExpr = Expression.Assign(left, right);

                    var expr = Expression.IfThen(condExpr, assignExpr);
                    fillEntityCodes.Add(expr);
                }
                else {
                    //entity.Name = reader[start++];
                    var right = Expression.Call(readerExpr,GetReaderItemMethodInfo ,Expression.Increment(startExpr));
                    var left = Expression.Property(entityExpr,info.PropertyInfo);
                    var expr = Expression.Assign(left, right);
                    fillEntityCodes.Add(expr);
                }
            }
            fillEntityCodes.Add(Expression.Return(labelTarget, entityExpr));

            var firstValueExpr = Expression.Call(readerExpr, GetReaderItemMethodInfo, startExpr);
            var chkExpr = Expression.Condition(Expression.Equal(firstValueExpr, Expression.Constant(this.DbNull)), Expression.Return(labelTarget), fillEntityExpr);

            var codes = new List<Expression>();
            codes.Add(chkExpr);
            var targetExpr = Expression.Label(labelTarget);
            codes.Add(targetExpr);

            var lamda = Expression.Lambda<Func<IDbReader, int,TEntity>>(Expression.Block(codes),readerExpr,startExpr);

            var fillFunc = lamda.Compile();

            return new SelectInfo<TEntity>() {
                FieldCount  = infos.Count,
                Fields = fields,
                Fieldnames = fieldnames,
                Fill = fillFunc,
                TableAlias = tableAlias
            };
        }

        //public virtual Action<IDbCommand> GenSelectBuilder(EntityInfo entityInfo,string fields,string alias) {
        //    var infos = GetFieldInfos(entityInfo, fields);
        //    var fieldnames = string.Empty;
        //    foreach (var info in infos)
        //    {
        //        if (fieldnames != string.Empty) fieldnames += ",";
        //        fieldnames += this.Escape(info.Fieldname);
        //    }
        //    var sql = "SELECT ";
        //}
    }
}
