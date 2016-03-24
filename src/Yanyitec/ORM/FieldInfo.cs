﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yanyitec.ORM
{
    public class FieldInfo
    {
        

        public FieldInfo(PropertyInfo propInfo,string fieldname=null)
        {
            this.PropertyInfo = propInfo;
            this.Fieldname = fieldname==null? propInfo.Name: fieldname;
        }
        public EntityInfo EntityInfo { get; private set; }

        public string Fieldname { get; protected internal set; }

       

        public PropertyInfo PropertyInfo { get; set; }
    }
}
