using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yanyitec.Storaging;

namespace Yanyitec.Runtime
{
    public class ArtifactLoader : IArtifactLoader
    {
        public ArtifactLoader(ArtifactLoader parent , IStorageDirectory packageDirectory) {
            this.SynchronizingObject = parent.SynchronizingObject;
            this.PackageDirectory = packageDirectory;
            this.OutputDirectory = parent.OutputDirectory;
            this._cached = new Dictionary<string, IArtifact>();
        }

        public ArtifactLoader(IStorageDirectory packageDirectory,IStorageDirectory outputDirectory)
        {
            this.SynchronizingObject = new System.Threading.ReaderWriterLockSlim();
            this.PackageDirectory = packageDirectory;
            this.OutputDirectory = outputDirectory;
            this._cached = new Dictionary<string, IArtifact>();
        }
        public System.Threading.ReaderWriterLockSlim SynchronizingObject { get; private set; }

        /// <summary>
        /// 运行时包的位置
        /// </summary>
        public IStorageDirectory PackageDirectory { get; set; }
        /// <summary>
        /// 动态编译的输出位置
        /// </summary>
        public IStorageDirectory OutputDirectory { get; set; }

        public ArtifactLoader Parent { get; set; }

        Dictionary<string, IArtifact> _cached ;

        public IArtifact Load(ArtifactLoaderOptions opt,System.Threading.ReaderWriterLockSlim syncLocker = null) {
            if (!string.IsNullOrWhiteSpace(opt.Assembly)) {
                return new PrecompiledArtifact(new StorageFile(opt.Assembly.Trim()), syncLocker?? this.SynchronizingObject);
            }
            return this.Load(opt.Name, opt.Version,syncLocker);
        }

        IArtifact InternalLoad(string name, string version, string compareName, System.Threading.ReaderWriterLockSlim synchronizingObject) {
            var result = this.LoadFromCache(compareName);
            if (result != null) return result;
            result = this.LoadFromPackageDir(name, version, compareName, synchronizingObject);
            if (result != null)
            {
                var cacheName = result.GetCacheName(synchronizingObject);
                _cached.Add(cacheName, result);
            }
            if (result == null && this.Parent !=null)
            {
                return Parent.InternalLoad(name, version, compareName, synchronizingObject);
            }
            return result;
        }

        public IArtifact Load(string name ,string version=null, System.Threading.ReaderWriterLockSlim syncObject = null) {
            var compareName = name;
            if (version != null)
            {
                if (version.EndsWith("*")) compareName = name + "." + version.TrimEnd('*') + "%";
            }
            else compareName = name + "%";
            if (this.SynchronizingObject == syncObject) return InternalLoad(name, version, compareName, syncObject);
            this.SynchronizingObject.EnterUpgradeableReadLock();
            try {
                var result = this.LoadFromCache(compareName);
                if (result != null) return result;
                this.SynchronizingObject.EnterWriteLock();
                try {
                    return InternalLoad(name, version, compareName, this.SynchronizingObject);
                } finally {
                    this.SynchronizingObject.ExitWriteLock();
                }
            } finally {
                this.SynchronizingObject.ExitUpgradeableReadLock();
            }
            
            
        }

        public IArtifact LoadProject(IStorageDirectory projectDir, System.Threading.ReaderWriterLockSlim synchonizingObject = null)
        {
            var projDefine = projectDir.GetItem("project.json", StorageTypes.File);
            if (projDefine != null) return new ProjectArtifact(synchonizingObject, projectDir, this.OutputDirectory, this as IArtifactLoader);
            return null;
        }

        public IArtifact LoadPackage(string name,string version=null, System.Threading.ReaderWriterLockSlim synchonizingObject = null) {
            var fullname = name +"." + (version == null ? string.Empty : version);
            var packDir = this.PackageDirectory.GetDirectory(fullname);
            if (packDir == null) {
                var dirs = this.PackageDirectory.ListItems(false, StorageTypes.Directory).Where(p => p.Name.StartsWith(name)).OrderByDescending(p=>p.Name);
                packDir = dirs.FirstOrDefault() as IStorageDirectory;
                if (packDir == null) return null;
            }
            bool defaultRequired = false;
            var libDir = packDir.GetDirectory("lib");
            
            if (libDir != null) {
                var asmFile = libDir.GetFile(Platform.DotnetVersion + "/" + name + ".dll");
                if (asmFile != null) return new PrecompiledArtifact(asmFile, synchonizingObject);
                asmFile = libDir.GetFile(Platform.DotnetVersion + "/_._");
                if (asmFile != null) {
                    asmFile = libDir.GetFile( "dotnet/" + name + ".dll");
                    if (asmFile != null) return new PrecompiledArtifact(asmFile, synchonizingObject);
                    defaultRequired = true;
                }
            }
            libDir = packDir.GetDirectory("ref");
            if (libDir != null)
            {
                var asmFile = libDir.GetFile(Platform.DotnetVersion + "/" + name + ".dll");
                if (asmFile != null) return new PrecompiledArtifact(asmFile, synchonizingObject);
                asmFile = libDir.GetFile(Platform.DotnetVersion + "/_._");
                if (asmFile != null || defaultRequired)
                {
                    asmFile = libDir.GetFile("dotnet/" + name + ".dll");
                    if (asmFile != null) return new PrecompiledArtifact(asmFile, synchonizingObject);
                    defaultRequired = true;
                }
            }
            return null;
        }

        protected IArtifact LoadFromCache(string compareName) {
            foreach (var pair in this._cached)
            {
                if (pair.Key.Like(compareName)) return pair.Value;
            }
            
            return null;
        }

        

        protected IArtifact LoadFromPackageDir(string name,string version, string compairName,System.Threading.ReaderWriterLockSlim synchonizingObjct = null)
        {
            var items = this.PackageDirectory.ListItems(false);
            var file1 = name + ".dll"; var file2 = name + ".exe";
            foreach (var item in items) {
                if (item.StorageType.IsFile()) {
                    if (item.FullName == file1 || item.FullName == file2) {
                        return new PrecompiledArtifact(item as IStorageFile, synchonizingObjct);
                    }
                }
                if (item.Name.Like(compairName) && item.StorageType.IsDirectory()) {
                    if (compairName.Length < item.Name.Length && item.Name[compairName.Length] != '.') continue;
                    var packDir = item as IStorageDirectory;
                    var artifact = this.LoadProject(packDir, synchonizingObjct);
                    if (artifact != null) return artifact;
                    artifact = LoadPackage(item.Name,null, synchonizingObjct);
                    if (artifact != null) return artifact;
                }
            }
            return null;
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
