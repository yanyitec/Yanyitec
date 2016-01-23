using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class StorageDirectory : StorageItem, IDirectory
    {
        

        

        protected internal StorageDirectory(DirectoryInfo info, StorageDirectory parent,Storage root)
            : base(StorageTypes.Directory, info, parent,root)
        {

        }

        public StorageDirectory(DirectoryInfo info) : this(info,null,null) { }

        public StorageDirectory(string absolutePath) : this(new DirectoryInfo(absolutePath))
        {

        }
        const string RootRegExpression = "^[a-zA-Z]:(?:\\\\|/)[^/\\\\]?";
        static readonly System.Text.RegularExpressions.Regex RootPathRegx = new System.Text.RegularExpressions.Regex(RootRegExpression);
        public string GetAbsolutePath(string path) {
            if (RootPathRegx.IsMatch(path)) return path;
            if (path.StartsWith("/") || path.StartsWith("\\")) {
                return this.InternalRoot.FullName + path.TrimEnd('\\','/');
            }
            return this.FullName==null?path : this.FullName + "/" + path.TrimEnd('/', '\\');
        }

        public IStorageItem CreateItem(string path, StorageTypes itemType = StorageTypes.Directory)
        {
            //CreateFilePathIfNotExisted(path);
            var filename = GetAbsolutePath(path);
            if (itemType == StorageTypes.Directory)
            {
                var dirInfo = System.IO.Directory.CreateDirectory(filename);
                return new StorageDirectory(dirInfo,null,this.InternalRoot);
            }
            if (itemType == StorageTypes.File)
            {
                var info = new FileInfo(filename);
                System.IO.Directory.CreateDirectory(info.DirectoryName);
                using (info.Create()) { } ;
                
                return new StorageFile(info,null,this.InternalRoot);
            }
            return null;
        }

        public void Delete()
        {
            if (this.FileSystemInfo != null)
            {
                this.FileSystemInfo.Delete();
            }
            throw new InvalidOperationException("Cannot delete the system root.");
        }


        public async Task<IStorageItem> CreateItemAsync(string path, StorageTypes itemType = StorageTypes.Directory) {
            return await Task.Run(()=>this.CreateItem(path,itemType));
        }


        public IStorageItem GetItem(string path, StorageTypes itemType = StorageTypes.All)
        {
            
            var absolutePath =this.GetAbsolutePath(path);
            if (itemType.IsDirectory())
            {
                if (System.IO.Directory.Exists(absolutePath))
                {
                    return new StorageDirectory(new DirectoryInfo(absolutePath),null,this.InternalRoot);
                }
            }
            if (itemType.IsFile())
            {
                if (System.IO.File.Exists(absolutePath))
                {
                    return new StorageFile(new FileInfo(absolutePath),null,this.InternalRoot);
                }
            }
            return null;
        }

        public async Task<IStorageItem> GetItemAsync(string path, StorageTypes itemType = StorageTypes.All)
        {
            return await Task.Run(() => this.GetItem(path, itemType));
        }
        public void AppendText(string path, string text, Encoding encoding = null)
        {
            var filename  =this.GetAbsolutePath(path);
            var info = new FileInfo(filename);
            if (!info.Exists) {
                if (!info.Directory.Exists) info.Directory.Create();
            }
            if (encoding == null) encoding = System.Text.Encoding.GetEncoding(Constants.DefaultCodepage);
            var buffer = encoding.GetBytes(text);
            
            using (var stream = System.IO.File.OpenWrite(filename)) {
                stream.Seek(0, System.IO.SeekOrigin.End);
                stream.Write(buffer,0,buffer.Length);
            }
        }

        public async Task AppendTextAsync(string path, string text, Encoding encoding = null)
        {
            var filename = this.GetAbsolutePath(path);
            var info = new FileInfo(filename);
            if (!info.Exists)
            {
                if (!info.Directory.Exists) info.Directory.Create();
            }
            if (encoding == null) encoding = System.Text.Encoding.GetEncoding(Constants.DefaultCodepage);
            var buffer = encoding.GetBytes(text);
            using (var stream = System.IO.File.OpenWrite(filename))
            {
                stream.Seek(0, System.IO.SeekOrigin.End);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }


        public byte[] GetBytes(string path) {
            var filename = this.GetAbsolutePath(path);
            if (!System.IO.File.Exists(filename)) return null;
            return System.IO.File.ReadAllBytes(filename);
        }

        public async Task<byte[]> GetBytesAsync(string path) {
            return await Task<byte[]>.Run(()=>GetBytes(path));
        }
        

        public string GetText(string path, Encoding encoding = null)
        {
            var bytes = this.GetBytes(path);
            if (encoding == null) encoding = System.Text.Encoding.GetEncoding(Constants.DefaultCodepage);
            return encoding.GetString(bytes,0,bytes.Length);
        }

        public async Task<string> GetTextAsync(string path, Encoding encoding = null)
        {
            return await Task<string>.Run(()=>this.GetText(path,encoding));
        }

        public IList<IStorageItem> ListItems(string path, StorageTypes itemType = StorageTypes.All)
        {
            
            var filename = this.GetAbsolutePath(path);

            if (!System.IO.Directory.Exists(filename)) return null;
            var result = new List<IStorageItem>();
            if (itemType.IsFile())
            {
                var filenames = System.IO.Directory.GetFiles(filename);
                foreach(var name in filenames) result.Add(new StorageFile(new FileInfo(name),this,this.InternalRoot));
            }
            if (itemType.IsDirectory())
            {
                path += "/";
                var filenames = System.IO.Directory.GetDirectories(filename);
                foreach (var name in filenames) result.Add(new StorageDirectory(new DirectoryInfo(name),this,this.InternalRoot));
            }
            return result;
        }

        public async Task<IList<IStorageItem>> ListItemsAsync(string path, StorageTypes itemType = StorageTypes.All)
        {
            return await Task<IList<IStorageItem>>.Run(()=>this.ListItems(path,itemType));
        }

        void InternalListItems(List<IStorageItem>  result ,StorageDirectory dir, bool includeSubs = false, StorageTypes itemType = StorageTypes.All) {
            var dirInfo = (dir.FileSystemInfo as DirectoryInfo);
            if (itemType.IsDirectory()) {
                var subInfos = dirInfo.GetDirectories();
                
                foreach (var sub in subInfos) {
                    var subItem = new StorageDirectory(sub, dir,this.InternalRoot);
                    result.Add(subItem);
                    if(includeSubs)InternalListItems(result, subItem, includeSubs,itemType);
                }
            }

            if (itemType.IsFile())
            {
                var subInfos = dirInfo .GetFiles();
                foreach (var sub in subInfos)
                {
                    result.Add(new StorageFile(sub, dir,this.InternalRoot));
                }
            }
        }

        public IList<IStorageItem> ListItems(bool includeSubs = false, StorageTypes itemType = StorageTypes.All) {
            var result = new List<IStorageItem>();
            if (this.FileSystemInfo==null) throw new InvalidOperationException("Cannot list all the items for file system root.");
            InternalListItems(result,this,includeSubs,itemType);
            return result;
        }

        public async Task<IList<IStorageItem>> ListItemsAsync(bool includeSubs = false, StorageTypes itemType = StorageTypes.All) {
            return await Task<IList<IStorageItem>>.Run(()=>ListItems(includeSubs,itemType));
        }



        public void PutBytes(string path, byte[] bytes) {
            var filename =this.GetAbsolutePath(path);
            var info = new FileInfo(filename);
            if (!info.Exists)
            {
                if (!info.Directory.Exists) info.Directory.Create();
            }
            
            System.IO.File.WriteAllBytes(filename,bytes);
        }

        public async Task PutBytesAsync(string path, byte[] bytes) {
            await Task.Run(()=>PutBytes(path,bytes));
        }

        public void PutText(string path, string text, Encoding encoding = null)
        {
            
            if (encoding == null) encoding = System.Text.Encoding.GetEncoding(Constants.DefaultCodepage);
            var bytes = encoding.GetBytes(text);
            this.PutBytes(path,bytes);
        }

        public async Task PutTextAsync(string path, string text, Encoding encoding = null)
        {
            await Task.Run(()=>PutText(path,text,encoding)); 
        }
        
    }
}
