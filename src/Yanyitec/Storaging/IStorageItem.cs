using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public interface IStorageItem : IEquatable<IStorageItem>
    {
        StorageTypes StorageType { get; }

        bool IsExisted { get; }

       

        //event Action<IStorageItem, StorageChangeEventArgs> OnChange;

        string RelativeName { get; }

        string FullName { get; }

        string Name { get; }

        IStorage Root { get; }

        
        IDirectory Parent { get; }
        string ToString();
    }
}
