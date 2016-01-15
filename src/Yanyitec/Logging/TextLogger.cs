using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanyitec.Logging
{
    public abstract class TextLogger : ILogger
    {
        public TextLogger(bool simpleMode = true) {
            this.SimpleMode = true;
        }
        protected TextLogger() { this.Locker = new object(); }

        protected object Locker { get; private set; }

        public bool SimpleMode { get; protected set; }

        protected abstract void Print(string content,DateTime now, LogTypes logType);
        public void Log(string content, LogTypes logType = LogTypes.Log)
        {
            var now = DateTime.Now;
            if (this.SimpleMode) {
                this.Print(content,now,logType);
                return;
            }
            
            var output = "====================================================\r\n";
            output += "@Log:" + Enum.GetName(typeof(LogTypes), logType) + "\r\n";
            output += "@At:" + now.ToString("yyyy-MM-dd hh:mm:ss") + "." + now.Millisecond.ToString() + "\r\n";
            output += content + "\r\n\r\n";
            lock (this.Locker) {
                this.Print(output,now,logType);
            }
            
        }

        public void Error(string content)
        {
            this.Log(content, LogTypes.Error);
        }

        public void Exception(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("@Type:").Append(exception.GetType().Name).Append("\r\n");
            sb.Append("@Message:").Append(exception.Message).Append("\r\n");
            sb.Append("@StackTrace:").Append(exception.StackTrace).Append("\r\n");
            var inner = exception.InnerException;
            while (inner != null) {
                sb.Append("@InnerException[").Append(inner.GetType().Name).Append("]:\r\n");
                sb.Append("@@Message:").Append(inner.Message).Append("\r\n");
                sb.Append("@@StackTrace:").Append(inner.StackTrace).Append("\r\n");
                inner = inner.InnerException;
            }
            
            this.Log(sb.ToString(), LogTypes.Exception);
        }

        

        public void Message(string content)
        {
            this.Log(content, LogTypes.Message);
        }

        public void Notice(string content)
        {
            this.Log(content, LogTypes.Notice);
        }

        public void Success(string content)
        {
            this.Log(content, LogTypes.Success);
        }

        public void Warning(string content)
        {
            this.Log(content, LogTypes.Warning);
        }
    }
}
