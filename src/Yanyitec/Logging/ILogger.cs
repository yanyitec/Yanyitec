using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yanyitec.Logging
{
    public interface ILogger
    {
        void Log(string content, LogTypes logType = LogTypes.Log);

        void Message(string content);

        void Warning(string content);

        void Error(string content);

        void Success(string content);

        void Exception(Exception exception);

        void Notice(string content);
    }
}
