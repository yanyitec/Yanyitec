using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Storaging;
using Yanyitec.Testing;

namespace Yanyitec.Unitest
{
    [Test]
    public class Storage_Unittest
    {
        const string BasePath = "d:/yanyi_test/basic";
        [Test("相对目录操作")]
        public void Relative() {
            var storage = new Storaging.Storage(BasePath);
            //storage 必须存在，如果没有目录就创建一个
            //Storage must existed.If not , the construtor will create it
            Assert.True(storage.IsExisted);
            //写一个文件
            //write text to a file
            storage.PutText("access/test.txt","hello");
            //读取一个文件文本
            //read text from a file
            var content = storage.GetText("access/test.txt");
            Assert.Equal("hello", content);
            //获取Storage Item
            // Get a storage item
            var textItem = storage.GetItem("access/test.txt");
            Assert.Equal(textItem.StorageType, StorageTypes.File);
            //Full name is the absolute path
            //FullName 是文件系统的全名
            Assert.Equal(BasePath + "/access/test.txt", textItem.FullName);
            //Relative name is the name relate on the storage base path
            //相对名是相对Storage 的名字
            Assert.Equal("access/test.txt", textItem.RelativeName);
            Assert.Equal(textItem.Root, storage);
            //Name is the name of this item (file name or directory name)
            //名字就文件名或目录名
            Assert.Equal("test.txt", textItem.Name);

            Assert.Equal(textItem.Parent.RelativeName,"access");
            Assert.Equal(textItem.Parent.FullName,BasePath + "/access");

            var subitem = textItem.Parent.CreateItem("sub/testdir");
            Assert.Equal(subitem.StorageType , StorageTypes.Directory);
            Assert.Equal(subitem.FullName,BasePath + "/access/sub/testdir");
            Assert.Equal(subitem.RelativeName,"access/sub/testdir");

            var accessItem = storage.GetItem("access") as IStorageDirectory;
            var list = accessItem.ListItems(false);
            Assert.Equal(2,list.Count);
            Assert.True(list[1].Equals(textItem));

            var all = accessItem.ListItems(true);
            Assert.Equal(3,all.Count);
            Assert.Equal(all[0].RelativeName, "access/sub");
            Assert.Equal(all[1].RelativeName,"access/sub/testdir");
            
            Assert.Equal(all[2].RelativeName,"access/test.txt");
        }
        [Test("绝对目录操作")]
        public void Absolute() {
            var storage = new Storaging.Storage(BasePath);
            var sub = storage.CreateItem("abs") as IStorageDirectory;
            var sub1 = sub.GetItem("/abs");
            Assert.Equal(sub.FullName,sub1.FullName);
        }
    }
}
