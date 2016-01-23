

namespace Yanyitec.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Reflection;
    using Yanyitec.Compilation;
    using Yanyitec.Storaging;
    using Yanyitec.Collections;
    using Yanyitec.Mvc.ParameterBinders;
    using Yanyitec.Runtime;


    public class Module : IModule
    {
        public Module(string nameOrPath,IHost host) {
            this.Host = host;
            this.Location = host.ModulePath.GetItem(nameOrPath);
            
            
        }
        public IArtifact Artifact
        {
            get; private set;
        }

        //public ICompiler CreateCompiler() { return null; }

        public IStorageItem Location { get; private set; }

        public IParameterBinders ParameterBinders { get; set; }

        public IHost Host
        {
            get; private set;
        }

        public void Reflush(bool addLock)
        {
            
        }

        readonly static TypeInfo IControllerTypeInfo = typeof(IController).GetTypeInfo();
        readonly static Type NonActionAttributeType = typeof(NonActionAttribute);
        
        void InternalReflush() {
            if (this.Location.StorageType == StorageTypes.Directory) {this.Compile();}
            var types = this.Artifact.Assembly.GetTypes();
            this.Commands = new SortedDiamond<string, string, ICommand>();
            foreach (var type in types) {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsAssignableFrom(IControllerTypeInfo) || typeInfo.Name.EndsWith("Controller")) {
                    var controllerName = typeInfo.Name;
                    GenCommands(controllerName,typeInfo);
                }
            }
        }

        void Compile() {
            //查找目录下的所有cs文件，并编译出来
            //var codes = GetCodes(null);
            //var compiler = this.CreateCompiler();
            //foreach (var code in codes) {
            //    compiler.AddCode(code);
            //}

            //var assembly = compiler.Compile();
            
        }
        IList<string> GetCodes(IStorage storage, IList<string> codes=null) {
            var items= storage.ListItems("");
            foreach (var item in items) {

            }
            return codes;
        }

        void GenCommands(string controllerName,TypeInfo typeInfo) {
            var methodInfos = typeInfo.DeclaredMethods;
            foreach (var methodInfo in methodInfos) {
                if (!methodInfo.IsPublic) continue;
                var attrs = methodInfo.GetCustomAttributes(false);
                if (attrs.FirstOrDefault(p => p.GetType() == NonActionAttributeType)!=null) continue;
                var cmd = new Command(methodInfo,this.ParameterBinders);
                var existed = Commands[controllerName, methodInfo.Name] ;
                if (existed != null) {
                    var cmds = existed as MethodCommands;
                    if (cmds == null) {
                        cmds = new MethodCommands();
                        cmds.AddCommand(existed);
                        cmds.AddCommand(cmd);
                        Commands[controllerName, methodInfo.Name] = cmds;
                    }
                } else {
                    Commands[controllerName, methodInfo.Name] = cmd;
                }
            }
        }

        
        

        public event Action OnRelush;

        public Task<bool> HandleRequestAsync(object rawContext)
        {
            throw new NotImplementedException();
        }

        private SortedDiamond<string, string, ICommand> Commands { get; set; }

        public ICommand GetCommand(string controllerName, string actionName) {
            throw new NotImplementedException();
        }

    }
}
