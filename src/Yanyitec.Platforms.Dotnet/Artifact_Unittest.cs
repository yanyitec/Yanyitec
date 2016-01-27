using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec
{
    using Storaging;
    using Yanyitec.Testing;
    using Yanyitec.Runtime;

    [Testing.Test]
    public class Artifact_Unittest
    {
        public const string ArtifactDir = "D:/yanyi_test/artifacts";
        [Test("测试运行期编译的Artifact")]
        public void ProjectArtifact_Unittest() {
            var storage = new Storage(ArtifactDir + "/test_project");
            var tempDir = storage.GetItem("/temp",StorageTypes.Directory,true) as IStorageDirectory;
            var projLocation = storage.GetItem("/web", StorageTypes.Directory, true) as IStorageDirectory;
            var modelDir = projLocation.GetItem("models",StorageTypes.Directory,true) as IStorageDirectory;
            var modelCode = @"
public class AuthModel{
    public string Username{get;set;}
    public string Password{get;set;}
}
";
            modelDir.PutText("AuthModel.cs",modelCode);

            var controllerDir = projLocation.GetItem("controllers",StorageTypes.Directory,true) as IStorageDirectory;
            var controllerCode = @"
public class AuthController{
    public object Signin(string username,string password){
        return new AuthModel(){Username = username , Password = password};
    }
}
";
            controllerDir.PutText("AuthController.cs",controllerCode);

            var jsDir = projLocation.GetDirectory("js",true);
            var jsCode = @"
alert('11');
";
            jsDir.PutText("alert.js",jsCode);

            var artifact = new ProjectArtifact(null,projLocation, tempDir, null);
            var assembly = artifact.Assembly;
            var types = assembly.DefinedTypes;
            Assert.Equal(2, types.Count());
            var rs = artifact.GetResourceText("/js/alert.js");
            Assert.Equal(jsCode,rs);

            //测试源代码改变后是否重新编译
            var addictionCode = @"public class AuthModel{
    public string Username{get;set;}
    public string Password{get;set;}
    public int Gender{get;set;}
}";
            modelDir.PutText("AuthModel.cs",addictionCode);
            Task.Run(async ()=> {
                await Task.Delay(1000);
                var newAssembly = artifact.Assembly;
                types = newAssembly.DefinedTypes;
                Assert.Equal(2, types.Count());
                var modelType = types.First(p => p.Name.Contains("Model"));
                var genderMember  = modelType.GetMembers().FirstOrDefault(p=>p.Name=="Gender");
                Assert.NotNull(genderMember);
            }).Wait();
            
        }

        [Test("测试ArtifactLoader,要正常运行该测试需要先拷贝一个Yanyitec包到Loader目录")]
        public void ArtifactLoader_Unittest() {
            var packageDir = new Storage(ArtifactDir + "/loader");
            var outputDir = new Storage(ArtifactDir + "/.compilation");
            var code = @"
using System;
public class UserWrapper{
    public Yanyitec.SSO.Identity GetIdentity(string name){
        return new Yanyitec.SSO.Identity(Guid.NewGuid(),name);
    } 
}
";
            var name = "TestLoader";
            var projDir = packageDir.GetDirectory("TestLoader", true);
            projDir.PutText("UserWrapper.cs", code);

            var json = "{\"frameworks\":{\"net450\":{\"dependencies\":{\"Yanyitec\":\"\"}}}";
            projDir.PutText("project.json",json);

            var loader = new ArtifactLoader(packageDir, outputDir);
            var artifact = loader.Load(name);
            var type = artifact.Assembly.DefinedTypes.FirstOrDefault(p=>p.Name == "UserWrapper");
            var obj = Activator.CreateInstance(type);
            var methodInfo = type.GetMethods().FirstOrDefault(p=>p.Name== "GetIdentity");
            var result = methodInfo.Invoke(obj, new object[] { "yanyi"});

        }

    }
}
