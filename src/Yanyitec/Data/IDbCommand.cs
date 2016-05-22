using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Data
{
    public interface IDbCommand
    {
        string CommandText { get; set; }

        int ExecuteNonQuery();

        IDbReader ExecuteReader();

        void SetParameter(string name,object value);
    }
}
