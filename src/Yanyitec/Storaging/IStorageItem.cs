﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public interface IStorageItem : IEquatable<IStorageItem>
    {
        StorageTypes StorageType { get; }

        bool IsExisted { get; }

        void Delete();

        //event Action<IStorageItem, StorageChangeEventArgs> OnChange;

        string RelativeName { get; }

        string FullName { get; }

        string Name { get; }

        IStorage Root { get; }

        bool WatchingSubdirectories { get; set; }

        string ToString();
    }
}
