using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public class ArtifactLoader
    {
        public System.Threading.ReaderWriterLockSlim SynchronizingObject { get; private set; }

        
        public IStorageDirectory PackageDirectory { get; set; }

        public IStorageDirectory OutputDirectory { get; set; }

        public ArtifactLoader Parent { get; set; }

        Dictionary<string, IArtifact> _cached ;

        public IArtifact Load(string name ,string version=null) {
            if (version != null)
            {
                if (version.EndsWith("*")) version = version.TrimEnd('*') + "%";
            }
            else version = "%";
            var fullname =name + "." + version;
            this.SynchronizingObject.EnterUpgradeableReadLock();
            try {
                
                this.SynchronizingObject.EnterWriteLock();
                try {

                } finally {
                    this.SynchronizingObject.ExitWriteLock();
                }
            } finally {
                this.SynchronizingObject.ExitUpgradeableReadLock();
            }
            
            return null;
        }

        protected IArtifact LoadFromCache(string compareName) {
            foreach (var pair in this._cached)
            {
                if (pair.Key.Like(compareName)) return pair.Value;
            }
            if (this.Parent != null) return this.Parent.LoadFromCache(compareName);
            return null;
        }

        protected IArtifact LoadFromPackage(string name, string compairName)
        {
            var items = this.PackageDirectory.ListItems(false);
            var file1 = name + ".dll"; var file2 = name + ".exe";
            foreach (var item in items) {
                if (item.StorageType.IsFile()) {
                    if (item.FullName == file1 || item.FullName == file2) {
                        return new PrecompiledArtifact(item as IStorageFile);
                    }
                }
                if (item.Name.Like(compairName) && item.StorageType.IsDirectory()) {
                    var packDir = item as IStorageDirectory;
                    var projItem = packDir.GetItem("project.json", StorageTypes.File);
                    if (projItem != null) return new ProjectArtifact(this.SynchronizingObject, packDir, this.OutputDirectory, this as IArtifactLoader);
                    var refItem = packDir.GetItem(Platform.DotnetVersion + "/" + name + ".dll");
                    if (refItem != null) return new PrecompiledArtifact(refItem as IStorageFile);
                    var ref1Item = packDir.GetItem(Platform.DotnetVersion + "/_._");
                    if (ref1Item == null) continue;
                    refItem = packDir.GetItem("dotnet/" + name + ".dll");
                    if (refItem != null) return new PrecompiledArtifact(refItem as IStorageFile);
                }
            }
            return null;
        }

        static IStorageFile GetAssemblyLocation(IStorageItem item) {
            if (item.StorageType.IsFile()) return item as IStorageFile;
            var packDir = item as IStorageDirectory;
            var refDir = packDir.GetDirectory("ref");
            var platformDir = refDir.GetDirectory(Platform.DotnetVersion);
            var subItems = platformDir.ListItems(false, StorageTypes.File);
            var file = subItems.FirstOrDefault(p=>p.Name.EndsWith(".dll"));
            if (file != null) return file as IStorageFile;
            platformDir = refDir.GetDirectory("dotnet");
            return platformDir.ListItems(false, StorageTypes.File).FirstOrDefault(p=>p.Name.EndsWith(".dll")) as IStorageFile;
        }

        private class AssemblyNameComparer : IComparer<AssemblyName>, IEqualityComparer<AssemblyName>
        {
            public static AssemblyNameComparer Ordinal = new AssemblyNameComparer();

            public int Compare(AssemblyName x, AssemblyName y)
            {
                int rs = x.Name.CompareTo(y.Name);
                if (rs == 0) {
                    rs = x.CultureName.CompareTo(y.CultureName);
                    
                }
                return rs;
            }

            public bool Equals(AssemblyName x, AssemblyName y)
            {
                return
                    string.Equals(x.Name, y.Name, StringComparison.Ordinal) &&
                    string.Equals(x.CultureName ?? "", y.CultureName ?? "", StringComparison.Ordinal);
            }

            public int GetHashCode(AssemblyName obj)
            {
                var hashCode = 0;
                if (obj.Name != null)
                {
                    hashCode ^= obj.Name.GetHashCode();
                }

                hashCode ^= (obj.CultureName ?? "").GetHashCode();
                return hashCode;
            }
        }
    }
}
