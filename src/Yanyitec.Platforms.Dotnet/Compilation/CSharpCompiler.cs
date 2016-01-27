

namespace Yanyitec.Compilation
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Storaging;

    public class CSharpCompiler : ICompiler
    {
        public CSharpCompiler(System.Threading.ReaderWriterLockSlim  locker=null) {
            this.SynchronizingObject = locker?? new System.Threading.ReaderWriterLockSlim();
        }

        readonly SortedDictionary<string, SyntaxTree> _syntaxTrees = new SortedDictionary<string, SyntaxTree>();

        public System.Threading.ReaderWriterLockSlim SynchronizingObject { get; private set; }

        /// <summary>
        /// 添加或替换代码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns>该代码的语法分析树</returns>
        public object AddOrReplaceCode(string key, string code, System.Threading.ReaderWriterLockSlim syncLocker = null) {
            SyntaxTree tree = null;
            if (SynchronizingObject == null || syncLocker == SynchronizingObject) {
                tree = SyntaxFactory.ParseSyntaxTree(code);
                if (_syntaxTrees.ContainsKey(key)) _syntaxTrees[key] = tree;
                else _syntaxTrees.Add(key,tree);
                return tree;
            }
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                tree = SyntaxFactory.ParseSyntaxTree(code);
                if (_syntaxTrees.ContainsKey(key)) _syntaxTrees[key] = tree;
                else _syntaxTrees.Add(key, tree);
                return tree;
            }
            finally {
                this.SynchronizingObject.ExitWriteLock();
            }
        }

        readonly List<CompileReference> _references = new List<CompileReference>();
        #region add references
        bool InternalAddReference(string assemblyLocation) {
            
            var refc = new CompileReference(assemblyLocation);
            var existed = _references.Find(p=>p.MetadataReference.Equals(refc));
            if (existed!=null) return false;
            _references.Add(refc);
            return true;
        }

        public bool AddReference(string assemblyLocation,System.Threading.ReaderWriterLockSlim locker = null) {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(assemblyLocation);
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                return this.InternalAddReference(assemblyLocation);
            }
            finally {
                this.SynchronizingObject.ExitWriteLock();
            }
        }

        bool InternalAddReference(Assembly assembly)
        {

            var refc = new CompileReference(assembly);
            var existed = _references.Find(p => p.MetadataReference.Equals(refc));
            if (existed != null) return false;
            _references.Add(refc);
            return true;
        }

        public bool AddReference(Assembly assembly, System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(assembly);
            this.SynchronizingObject.EnterWriteLock();
                try
                {
                    return this.InternalAddReference(assembly);
                }
                finally {
                    this.SynchronizingObject.ExitWriteLock();
                }
        }

        public bool AddReference(IArtifact artifact, System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(artifact);
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                return this.InternalAddReference(artifact);
            }
            finally
            {
                this.SynchronizingObject.ExitWriteLock();
            }
        }

        bool InternalAddReference(IArtifact artifact) {
            var refc = new CompileReference(artifact);
            var existed = _references.Find(p => p.MetadataReference.Equals(refc));
            if (existed != null) return false;
            _references.Add(refc);
            return true;
        }


        public bool AddReference(Type keytype, System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(keytype.Assembly);
            this.SynchronizingObject.EnterWriteLock();
            try
            {
                return this.InternalAddReference(keytype.Assembly);
            }
            finally {
                this.SynchronizingObject.ExitWriteLock();
            }
        }
        #endregion


        #region location
        public IStorageFile Location {
            get { return this.GetLocation(); }
            set { this.SetLocation(value); }
        }
        IStorageFile _location;
        public IStorageFile GetLocation(System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return _location;
            lock(this.SynchronizingObject)
            {
                return _location;
            }
            
        }

        public void SetLocation(IStorageFile file, System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) _location = file;
            else lock(this.SynchronizingObject)
            {
                _location = file;
            }
            
        }

        public void SetLocation(string file, System.Threading.ReaderWriterLockSlim locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) _location = new Yanyitec.Storaging.StorageFile(file);
            else lock(this.SynchronizingObject)
            {
                _location = new StorageFile(file);
            }
            
        }
        #endregion

        public Assembly Compile(string name, System.Threading.ReaderWriterLockSlim locker = null) {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalCompile(name);
            lock(this.SynchronizingObject)
            {
                return this.InternalCompile(name);
            }
            
        }

        Assembly InternalCompile(string name) {
            List<MetadataReference> refs = new List<MetadataReference>();
            foreach (var refc in this._references) refs.Add(refc.MetadataReference);
            
            
            //MetadataReference.
            var compilation = CSharpCompilation.Create(name, this._syntaxTrees.Values,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: refs);

            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                var compileResult = compilation.Emit(stream);
                var buffer = stream.GetBuffer();
                compiledAssembly = Assembly.Load(buffer);
                if (_location != null)
                {
                    using (var filestream = _location.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                return compiledAssembly;
            }      
        }
    }
}
