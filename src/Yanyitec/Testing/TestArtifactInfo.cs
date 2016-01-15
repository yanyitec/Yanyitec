using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Testing
{
    using Yanyitec.Runtime;
    public class TestArtifactInfo
    {
        public TestArtifactInfo(IArtifact artifact) {
            var attr = artifact.GetAttribute<TestAttribute>();
            if (attr != null) {
                this.Description = attr.Description;
                this.IsValid = true;
                this.Artifact = artifact;
                artifact.OnChange += (sender,eventArgs) => {
                    lock (this) {
                        if (eventArgs.ChangeType == ArtifactChangeTypes.Deleted)
                        {
                            this.IsValid = false;
                            this._testClassInfos = null;
                        }
                        else {
                            this._testClassInfos = null;
                        }
                    }
                };
            }
        }

        public bool IsValid { get; private set; }

        public string Name {
            get { return this.Artifact.Name; } }

        public string Description { get; private set; }

        public IArtifact Artifact { get; private set; }

        Dictionary<string, TestClassInfo> GetTestClassInfos()
        {
            if (!this.IsValid) throw new ObjectDisposedException("Invalid TestArtifactInfo. The Artifact's assembly is properly deleted.");
            var result = new Dictionary<string, TestClassInfo>();
            var types = this.Artifact.GetTypeInfos();
            foreach (var type in types)
            {
                var clsInfo = new TestClassInfo(type);
                if (clsInfo.IsValid)
                {
                    result.Add(clsInfo.Name, clsInfo);
                }
            }
            return result;
        }

        IReadOnlyDictionary<string, TestClassInfo> _testClassInfos;
        public IReadOnlyDictionary<string, TestClassInfo> TestClassInfos
        {
            get
            {
                if (_testClassInfos == null)
                {
                    lock (this)
                    {
                        if (_testClassInfos == null)
                        {
                            _testClassInfos = GetTestClassInfos().AsReadonly();
                        }
                    }
                }
                return _testClassInfos;
            }
        }

        public IDictionary<string,IDictionary<string,AssertException>> TestMethods(string clsName=null,string methodName=null, Dictionary<string, IDictionary<string, AssertException>> result = null) {
             result = result ?? new Dictionary<string, IDictionary<string, AssertException>>();
            foreach (var pair in TestClassInfos) {
                if (clsName == null || pair.Key.Like(clsName)) {
                    var clsResult = new Dictionary<string, AssertException>();
                    pair.Value.RunMethods(methodName,clsResult);
                    result.Add(pair.Key,clsResult);

                }
            }
            return result;
            


        }

    }
}
