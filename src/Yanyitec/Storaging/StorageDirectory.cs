using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public class StorageDirectory : StorageItem, IStorageDirectory
    {
        

        protected internal StorageDirectory(DirectoryInfo info, StorageDirectory parent,Storage root)
            : base(StorageTypes.Directory, info, parent,root)
        {

        }

        public StorageDirectory(DirectoryInfo info) : this(info,null,null) { }

        public StorageDirectory(string absolutePath) : this(new DirectoryInfo(absolutePath))
        {

        }
        

        public IStorageItem CreateItem(string path, StorageTypes itemType = StorageTypes.Directory)
        {
            //CreateFilePathIfNotExisted(path);
            var filename = new AbsolutePath(path,this);
            if (itemType == StorageTypes.Directory)
            {
                var dirInfo = System.IO.Directory.CreateDirectory(filename);
                return new StorageDirectory(dirInfo,null, filename.Storage);
            }
            if (itemType == StorageTypes.File)
            {
                var info = new FileInfo(filename);
                System.IO.Directory.CreateDirectory(info.DirectoryName);
                using (info.Create()) { } ;
                
                return new StorageFile(info,null,filename.Storage);
            }
            return null;
        }

        public virtual void Delete()
        {
            if (this.FileSystemInfo != null)
            {
                this.FileSystemInfo.Delete();
            }else throw new InvalidOperationException("Cannot delete the system root.");
        }

        public bool Delete(string path) {
            var absolutePath = new AbsolutePath(path, this);
            var item = this.GetItem(path);
            if (item != null) {
                if (item.StorageType == StorageTypes.Directory) (item as StorageDirectory).Delete();
                else (item as StorageFile).Delete();
                return true;
            }
            return false;
        }


        public async Task<IStorageItem> CreateItemAsync(string path, StorageTypes itemType = StorageTypes.Directory) {
            return await Task.Run(()=>this.CreateItem(path,itemType));
        }


        public IStorageItem GetItem(string path, StorageTypes itemType = StorageTypes.All,bool createIfNotExisted=false)
        {
            
            var absolutePath = new AbsolutePath(path, this);
            if (itemType.IsDirectory())
            {
                if (System.IO.Directory.Exists(absolutePath))
                {
                    return new StorageDirectory(new DirectoryInfo(absolutePath), null, absolutePath.Storage);
                }
                else if(createIfNotExisted){
                    var dir = new DirectoryInfo(absolutePath);
                    dir.Create();
                    return new StorageDirectory(dir, null, absolutePath.Storage);
                }
            }
            if (itemType.IsFile())
            {
                if (System.IO.File.Exists(absolutePath))
                {
                    return new StorageFile(new FileInfo(absolutePath), null, absolutePath.Storage);
                }
                else if(createIfNotExisted){
                    var info = new FileInfo(absolutePath);
                    if (!info.Directory.Exists) info.Directory.Create();
                    using (var s = info.Create()) { };
                    return new StorageFile(info, null, absolutePath.Storage);
                }
            }
            return null;
        }

        public async Task<IStorageItem> GetItemAsync(string path, StorageTypes itemType = StorageTypes.All,bool createIfNotExisted=false)
        {
            return await Task.Run(() => this.GetItem(path, itemType,createIfNotExisted));
        }
        public void AppendText(string path, string text, Encoding encoding = null)
        {
            var filename  =new AbsolutePath(path, this);
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
            var filename = new AbsolutePath(path, this);
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
            var filename = new AbsolutePath(path, this);
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
            
            var filename = new AbsolutePath(path , this);

            if (!System.IO.Directory.Exists(filename)) return null;
            var result = new List<IStorageItem>();
            if (itemType.IsFile())
            {
                var filenames = System.IO.Directory.GetFiles(filename);
                foreach(var name in filenames) result.Add(new StorageFile(new FileInfo(name),this,filename.Storage));
            }
            if (itemType.IsDirectory())
            {
                path += "/";
                var filenames = System.IO.Directory.GetDirectories(filename);
                foreach (var name in filenames) result.Add(new StorageDirectory(new DirectoryInfo(name),this,filename.Storage));
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
                    var subItem = new StorageDirectory(sub, dir,this.InternalStorage);
                    result.Add(subItem);
                    if(includeSubs)InternalListItems(result, subItem, includeSubs,itemType);
                }
            }

            if (itemType.IsFile())
            {
                var subInfos = dirInfo .GetFiles();
                foreach (var sub in subInfos)
                {
                    result.Add(new StorageFile(sub, dir,this.InternalStorage));
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
            var filename = new AbsolutePath(path, this);
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


        public IStorage AsStorage() {
            if (this.StorageType.IsStorage()) return this as IStorage;
            return new Storage(this.FileSystemInfo as DirectoryInfo, this);
        }

        #region watcher
        Action<IStorageDirectory, ItemChangedEventArgs> _changed;

        System.IO.FileSystemWatcher _watcher;

        public event Action<IStorageDirectory, ItemChangedEventArgs> Changed {
            add {
                lock (this.Storage.SynchronizingObject) {
                    _changed += value;
                    if (_watcher == null) _watcher = CreateWatcher();
                }
            }
            remove {
                lock (this.Storage.SynchronizingObject) {
                    _changed -= value;
                    if (_changed == null && _watcher!=null) {
                        _watcher.Dispose();
                        _watcher = null;
                    }
                }
            }
        }

        System.IO.FileSystemWatcher CreateWatcher() {
            System.IO.FileSystemWatcher watch = new FileSystemWatcher();       //初始化目录监视
                                                                               //#if DNXCORE50
                                                                               //#else
                                                                               //            watch.BeginInit();
                                                                               //#endif 
            watch.Path = this.FullName;
            watch.Filter = "*.*";
            //watch.Filter = "*.txt";                      //监视的对象，目录中监视哪些文件，默认为*.*
            //不过这里有个好玩的地方，实验证明，通配符可以用在很多地方，比如可以设置成为  watch.Filter = "*.tx*";    针对具体文件就写具体文件名
            watch.IncludeSubdirectories = true;     //包括子目录
            watch.Changed += Watch_Changed;          //文件改变事件
            watch.Created += Watch_Created;          //文件添加事件
            watch.Deleted += Watch_Deleted;          //文件删除事件
            watch.Renamed += Watch_Renamed;
            watch.Error += Watch_Error;
            //otifyFilter：获取或设置要监视的更改类型。
            //下面是csdn例子对文件属性的监视，这里有个问题，就是NotifyFilters.LastAccess | NotifyFilters.LastWrite同时设置的话 Changed事件会运行两次
            //原因就是 LastAccess 文件或文件夹上一次打开的日期。  LastWrite 上一次向文件或文件夹写入内容的日期。 打开文件修改的时候会同时激发
            //这个是要注意的第二点
            watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //要注意的第三点，如果下面不设置为true,事件是不会运行的 EnableRaisingEvents 属性是指示是否启用此组件
            watch.EnableRaisingEvents = true;
            //watch.
            //watch.
            return watch;
        }

        private void Watch_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Watch_Renamed(object sender, RenamedEventArgs e)
        {

            var item = this.Storage.GetItem(e.FullPath);
            var evt = new ItemChangedEventArgs(this, item, ChangeKinds.Renamed, e.OldName ,this.Storage.SynchronizingObject);
            if(this._changed!=null) this._changed(this,evt);
        }

        private void Watch_Deleted(object sender, FileSystemEventArgs e)
        {
            var item = this.Storage.GetItem(e.FullPath);
            var evt = new ItemChangedEventArgs(this, item, ChangeKinds.Deleted, null, this.Storage.SynchronizingObject);
            if (this._changed != null) this._changed(this, evt);
        }

        private void Watch_Created(object sender, FileSystemEventArgs e)
        {
            var item = this.Storage.GetItem(e.FullPath);
            var evt = new ItemChangedEventArgs(this, item, ChangeKinds.Created, null, this.Storage.SynchronizingObject);
            if (this._changed != null) this._changed(this, evt);
        }

        private void Watch_Changed(object sender, FileSystemEventArgs e)
        {
            var item = this.Storage.GetItem(e.FullPath);
            var evt = new ItemChangedEventArgs(this, item, ChangeKinds.Updated, null, this.Storage.SynchronizingObject);
            if (this._changed != null) this._changed(this, evt);
        }


        
        #endregion
    }
}
