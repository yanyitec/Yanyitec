using Yanyitec.Storaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Logging
{
    public class FileLogger : TextLogger
    {
       
        public FileLogger(string basePath) {
            this.Storage = new Storage(basePath);           
        }

        public FileLogger(IStorage storage) {
            this.Storage = storage;
        }

        public IStorage Storage { get; private set; }

        public static readonly FileLogger Default = new FileLogger(System.IO.Directory.GetCurrentDirectory().TrimEnd('/','\\') + "/logs");

        protected override void Print(string content, DateTime now, LogTypes logType)
        {
            var file = this.GetFilename(now, logType);
            this.Storage.AppendText(file,content);
        }

        protected virtual string GetFilename(DateTime now, LogTypes logType) {
            var path = now.Year.ToString("0000") + now.Month.ToString("00") + "/" + now.Day.ToString("00");
            path += "/" + Enum.GetName(typeof(LogTypes), logType) + "." + now.Hour.ToString("00") + ".log";
            return path;
        }
        
    }
}
