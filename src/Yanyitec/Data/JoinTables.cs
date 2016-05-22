using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public class JoinTables
    {
        readonly List<JoinTable> _tables = new List<JoinTable>();
        public EntityInfo EntityInfo { get; private set; }

        public JoinTables(EntityInfo info) { this.EntityInfo = info; }
        public JoinTable this[int i] {
            get { return this._tables[i]; }
        }
        public JoinTable GetOrCreate(ReferenceInfo info,JoinTable prev=null) {
            JoinTable result = null;
            for (var i = 0; i < this._tables.Count; i++) {
                result = this._tables[i];
                if (result.ReferenceInfo == info && result.JoinFrom == prev) return result;
            }
            result = new JoinTable("_TMP_Tb" + (this._tables.Count + 1).ToString(),info,prev);
            return result;
        }

        public int Count {
            get { return this._tables.Count; }
        }

        public string ToSql(DbCommandBuilderFactory cmdFactory) {
            var sql = new StringBuilder(this.EntityInfo.Tablename);
            sql.Append(" AS ").Append("_TMP_Tb0");
            for (var i = 0; i < this._tables.Count; i++) {
                var tb = this._tables[i];
                sql.Append(" LEFT JOIN ")
                    .Append(cmdFactory.Escape(tb.ReferenceInfo.ReferenceEntityInfo.Tablename))
                    .Append(" AS ").Append(tb.Alias)
                    .Append(" ON ");
                if (tb.JoinFrom == null) {
                    sql.Append("_TMP_Tb0.");
                } else {
                    sql.Append(tb.JoinFrom.Alias);
                }
                sql.Append(tb.ReferenceInfo.PrimaryField == null ? cmdFactory.Escape(tb.ReferenceInfo.PrimaryEntityInfo.PrimaryFieldInfo.Fieldname): cmdFactory.Escape(tb.ReferenceInfo.PrimaryField.Fieldname));
                sql.Append('=');
                sql.Append(tb.Alias).Append(tb.ReferenceInfo.ReferenceField.Fieldname);
            }
            return sql.ToString();
            
        }
    }
}
