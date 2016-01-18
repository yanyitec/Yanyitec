using Yanyitec.Compilation;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Json
{
    public class JsonConvert {
        public ITypeCompiler Compiler { get; private set; }

        public IRWLocker RWLocker { get; private set; }

        protected SortedDictionary<int, IConverter> Converters { get; private set; }

        public JToken Parse(string jsonString)
        {
            var parser = new Parser();
            return parser.Parse(jsonString,0);
        }

        public T Parse<T>(string jsonString)
        {
            var converter = this.GetConverter(typeof(T)) as IConverter<T>;
            return converter.Parse(jsonString); 
        }

        

        public string ToJson<T>(T obj)
        {
            var converter = this.GetConverter(typeof(T)) as IConverter<T>;
            return converter.ToJson(obj);
        }
        #region private
        protected IConverter GetConverter(Type type)
        {
            IConverter converter = null;

            var typeHashCode = type.GetHashCode();
            try
            {
                RWLocker.EnterReadLock();

                if (!this.Converters.TryGetValue(typeHashCode, out converter))
                {
                    RWLocker.UpgradeToWriteLock();
                    if (!this.Converters.TryGetValue(typeHashCode, out converter))
                    {
                        //自动生成一个Converter
                        //TODO : converter = GenConverter(type);
                        this.Converters.Add(typeHashCode, converter);
                    }

                }
            }
            finally
            {
                RWLocker.ExitLock();
            }
            return converter;
        }
        #endregion
    }
}
