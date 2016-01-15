using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Collections
{
    public class Diamond<TX,TY,TValue>
    {
        Dictionary<TX, Dictionary<TY, TValue>> data;
       
        public TValue this[TX x, TY y] {
            get {
                return data[x][y];
            }
            set {
                Dictionary<TY,TValue> axisYLine = null;
                if (!data.TryGetValue(x, out axisYLine)) {
                    axisYLine = new Dictionary<TY, TValue>();
                    data[x] = axisYLine;
                }
                axisYLine[y] = value; 
            }
        }

        public bool TryGetValue(TX x, TY y, out TValue value) {
            Dictionary<TY, TValue> axisYLine = null;
            if (data.TryGetValue(x, out axisYLine))
            {
                return (axisYLine.TryGetValue(y, out value)) ;
            }
            value = default(TValue);
            return false;
        }
    }
}
