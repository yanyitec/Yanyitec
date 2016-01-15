using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Yanyitec.Mvc
{
    using Compilation;
    using System.Reflection;
    using Yanyitec.Runtime;
    public interface IModule
    {
        IArtifact Artifact { get; }

        IHost Host { get; }

        void Reflush(bool addLock);

        event Action OnRelush;

        Task<bool> HandleRequestAsync(object rawContext);

        ICommand GetCommand(string controllerName, string actionName);
    }
}
