using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Logging
{
    public class ConsoleLogger : TextLogger
    {
        private ConsoleLogger():base(true){ }

        readonly static Dictionary<LogTypes, ConsoleColor> Colors = new Dictionary<LogTypes, ConsoleColor>(){
            {LogTypes.Error, ConsoleColor.Red}
            , { LogTypes.Exception, ConsoleColor.DarkRed}
            , { LogTypes.Log, ConsoleColor.White}
            , { LogTypes.Message, ConsoleColor.Blue}
            , { LogTypes.Notice, ConsoleColor.Magenta}
            , { LogTypes.Success , ConsoleColor.Green}
            , { LogTypes.Warning , ConsoleColor.Yellow}
        };

        public static readonly ConsoleLogger Default = new ConsoleLogger();

        protected override void Print(string content, DateTime now, LogTypes logType)
        {
            System.Console.ForegroundColor = Colors[logType];
            System.Console.BackgroundColor = ConsoleColor.Black;
            System.Console.Write(content);
        }
    }
}
