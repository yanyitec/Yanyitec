using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Mvc
{
    public class MethodCommands : ICommand
    {
        SortedDictionary<HttpMethods, ICommand> commands;
        public MethodCommands() { }

        public HttpMethods AcceptMethods
        {
            get
            {
                return HttpMethods.ALL;
            }
        }

        public void AddCommand(ICommand cmd) {
            commands.Add(cmd.AcceptMethods,cmd);
        }
        public object Execute(object controller, IInputData inputData)
        {
            if (inputData.HttpMethod.HasValue) {
                foreach (var pair in commands) {
                    if (pair.Key.Is(inputData.HttpMethod.Value)) return pair.Value.Execute(controller,inputData);
                }
            }
            return commands.First().Value.Execute(controller,inputData);
        }
    }
}
