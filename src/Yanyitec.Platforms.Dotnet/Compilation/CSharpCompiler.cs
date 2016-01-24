

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
        public CSharpCompiler(object  locker=null) {
            this.SynchronizingObject = locker;
        }

        readonly SortedDictionary<string, SyntaxTree> _syntaxTrees = new SortedDictionary<string, SyntaxTree>();

        public object SynchronizingObject { get; private set; }

        /// <summary>
        /// 添加或替换代码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns>该代码的语法分析树</returns>
        public object AddOrReplaceCode(string key, string code, object syncLocker = null) {
            SyntaxTree tree = null;
            if (SynchronizingObject == null || syncLocker == SynchronizingObject) {
                tree = SyntaxFactory.ParseSyntaxTree(code);
                if (_syntaxTrees.ContainsKey(key)) _syntaxTrees[key] = tree;
                else _syntaxTrees.Add(key,tree);
                return tree;
            }
            lock(this.SynchronizingObject) {
                tree = SyntaxFactory.ParseSyntaxTree(code);
                if (_syntaxTrees.ContainsKey(key)) _syntaxTrees[key] = tree;
                else _syntaxTrees.Add(key, tree);
                return tree;
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

        public bool AddReference(string assemblyLocation,object locker = null) {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(assemblyLocation);
            lock(this.SynchronizingObject) {
                return this.InternalAddReference(assemblyLocation);
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

        public bool AddReference(Assembly assembly, object locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(assembly);
            lock (this.SynchronizingObject)
            {
                return this.InternalAddReference(assembly);
            }
        }

        
        public bool AddReference(Type keytype, object locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return this.InternalAddReference(keytype.Assembly);
            lock (this.SynchronizingObject)
            {
                return this.InternalAddReference(keytype.Assembly);
            }
        }
        #endregion


        #region location
        public IStorageFile Location {
            get { return this.GetLocation(); }
            set { this.SetLocation(value); }
        }
        IStorageFile _location;
        public IStorageFile GetLocation(object locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) return _location;
            lock(this.SynchronizingObject)
            {
                return _location;
            }
            
        }

        public void SetLocation(IStorageFile file, object locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) _location = file;
            else lock(this.SynchronizingObject)
            {
                _location = file;
            }
            
        }

        public void SetLocation(string file, object locker = null)
        {
            if (this.SynchronizingObject == null || locker == this.SynchronizingObject) _location = new Yanyitec.Storaging.StorageFile(file);
            else lock(this.SynchronizingObject)
            {
                _location = new StorageFile(file);
            }
            
        }
        #endregion

        public Assembly Compile(string name, object locker = null) {
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
