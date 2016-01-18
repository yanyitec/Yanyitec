using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Storaging
{
    public interface IDirectory :IStorageItem
    {
        void Delete();
        void AppendText(string path, string text, Encoding encoding = null);
        Task AppendTextAsync(string path, string text, Encoding encoding = null);
        IStorageItem CreateItem(string path, StorageTypes itemType = StorageTypes.Directory);
        Task<IStorageItem> CreateItemAsync(string path, StorageTypes itemType = StorageTypes.Directory);
        byte[] GetBytes(string path);
        Task<byte[]> GetBytesAsync(string path);
        IStorageItem GetItem(string path, StorageTypes itemType = StorageTypes.All);
        Task<IStorageItem> GetItemAsync(string path, StorageTypes itemType = StorageTypes.All);
        string GetText(string path, Encoding encoding = null);
        Task<string> GetTextAsync(string path, Encoding encoding = null);
        IList<IStorageItem> ListItems(string path, StorageTypes itemType = StorageTypes.All);
        IList<IStorageItem> ListItems(bool includeSubs = false, StorageTypes itemType = StorageTypes.All);
        Task<IList<IStorageItem>> ListItemsAsync(string path, StorageTypes itemType = StorageTypes.All);
        Task<IList<IStorageItem>> ListItemsAsync(bool includeSubs = false, StorageTypes itemType = StorageTypes.All);
        void PutBytes(string path, byte[] bytes);
        Task PutBytesAsync(string path, byte[] bytes);
        void PutText(string path, string text, Encoding encoding = null);
        Task PutTextAsync(string path, string text, Encoding encoding = null);
    }
}