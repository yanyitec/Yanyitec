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
        [Test]
        public void Storage() {
            var storage = new Storaging.Storage(BasePath);
            //storage 必须存在，如果没有目录就创建一个
            //Storage must existed.If not , the construtor will create it
            Assert.IsTrue(storage.IsExisted);
            //写一个文件
            //write text to a file
            storage.PutText("access/test.txt","hello");
            //读取一个文件文本
            //read text from a file
            var content = storage.GetText("access/test.txt");
            Assert.AreEqual("hello", content);
            //获取Storage Item
            // Get a storage item
            var textItem = storage.GetItem("access/test.txt");
            Assert.AreEqual(textItem.StorageType, StorageTypes.File);
            //Full name is the absolute path
            //FullName 是文件系统的全名
            Assert.AreEqual(BasePath + "/access/test.txt", textItem.FullName);
            //Relative name is the name relate on the storage base path
            //相对名是相对Storage 的名字
            Assert.AreEqual("access/test.txt", textItem.RelativeName);
            Assert.AreEqual(textItem.Root, storage);
            //Name is the name of this item (file name or directory name)
            //名字就文件名或目录名
            Assert.AreEqual("test.txt", textItem.Name);

            Assert.AreEqual(textItem.Parent.RelativeName,"access");
            Assert.AreEqual(textItem.Parent.FullName,BasePath + "/access");

            var subitem = textItem.Parent.CreateItem("sub/testdir");
            Assert.AreEqual(subitem.StorageType , StorageTypes.Directory);
            Assert.AreEqual(subitem.FullName,BasePath + "/access/sub/testdir");
            Assert.AreEqual(subitem.RelativeName,"access/sub/testdir");

            var accessItem = storage.GetItem("access") as IDirectory;
            var list = accessItem.ListItems(false);
            Assert.AreEqual(2,list.Count);
            Assert.IsTrue(list[1].Equals(textItem));

            var all = accessItem.ListItems(true);
            Assert.AreEqual(3,all.Count);
            Assert.AreEqual(all[0].RelativeName, "access/sub");
            Assert.AreEqual(all[1].RelativeName,"access/sub/testdir");
            
            Assert.AreEqual(all[2].RelativeName,"access/test.txt");
        }
    }
}
