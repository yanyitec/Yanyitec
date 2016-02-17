namespace Yanyitec.Unitest
{
    using System;
    using System.Threading.Tasks;
    using Caches;
    using Yanyitec.Testing;
    [Testing.Test]
    public class Cache_Unittest
    {
        [Test]
        public async Task GetSetItem()
        {
            var cache = new Yanyitec.Caches.MemCache(2000, 1000);
            //添加一个缓存
            cache["user"] = "yiy";
            //200毫秒后，不应该消失
            await Task.Delay(200);
            var username = cache["user"];
            Assert.Equal("yiy", username);

            DateTime t0 = DateTime.Now;
            var item = cache.GetItem("user");
            DateTime removeTime = DateTime.MinValue;
            CacheItem removedItem = null;
            item.Removed += (removed) => { removeTime = DateTime.Now; removedItem = removed; };
            Assert.NotNull(item);
            Assert.Equal("user", item.Name);
            //最后访问时间应该比上次的时间大，比现在小
            var accessTime = item.LastAccessTime;
            Assert.True(accessTime >= t0);
            Assert.True(accessTime <= DateTime.Now);
            Assert.Equal("yiy", item.Value.ToString());
            DateTime t1 = DateTime.Now;
            //3000毫秒后，应该消失
            await Task.Delay(3000);
            //应该触发了remove事件
            Assert.NotNull(removedItem);
            Assert.Equal(item, removedItem);

            ////移除时间应该在访问之后
            Assert.True(removeTime > t1);

            username = cache["user"];
            Assert.Null(username);
        }

        [Test]
        public async Task GetSetSub()
        {
            var cache = new Yanyitec.Caches.MemCache(2000, 1000);
            //添加一个缓存
            var sub = cache.GetOrCreateSubCache("sub");
            sub["name"] = "yiy";
            await Task.Delay(200);
            var name = sub["name"];
            Assert.Equal("yiy", name);

            await Task.Delay(3000);
            name = sub["name"];

            Assert.Null(name);
        }
    }
}
