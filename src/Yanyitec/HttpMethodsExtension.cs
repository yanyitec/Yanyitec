using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    public static class HttpMethodsExtension
    {
        public static int AcceptCount(this HttpMethods self) 
        {
            int c = 0; 
            if (((int)self & (int)HttpMethods.GET) != 0) c++;
            if (((int)self & (int)HttpMethods.POST) != 0) c++;
            if (((int)self & (int)HttpMethods.PUT) != 0) c++;
            if (((int)self & (int)HttpMethods.DELETE) != 0) c++;
            return c;
        }

        public static bool Is(this HttpMethods self, HttpMethods method) {
            return ((int)self & (int)method) != 0;
        }

        public static bool IsGET(this HttpMethods self) 
        {
            return ((int)self & (int)HttpMethods.GET) !=0;
        }

        public static bool IsPOST(this HttpMethods self)
        {
            return ((int)self & (int)HttpMethods.POST) != 0;
        }

        public static bool IsPUT(this HttpMethods self)
        {
            return ((int)self & (int)HttpMethods.PUT) != 0;
        }

        public static bool IsDELETE(this HttpMethods self)
        {
            return ((int)self & (int)HttpMethods.DELETE) != 0;
        }
    }
}
