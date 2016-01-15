


namespace Yanyitec.Json
{
    using System;
    public class ParseException : Exception
    {
        public ParseException(string msg, string jsonString, int charAt, int lineAt, int columnAt) : base(msg)
        {
            this.JsonString = jsonString;
            this.CharAt = charAt;
            this.LineAt = lineAt;
            this.ColumnAt = columnAt;
        }

        public ParseException(string msg, Reader reader)
            : base(msg)
        {
            this.JsonString = reader.JsonString;
            this.CharAt = reader.At;
            this.LineAt = reader.LineAt;
            this.ColumnAt = reader.ColumnAt;
        }


        public string JsonString { get; private set; }

        public int CharAt { get; private set; }

        public int LineAt { get; private set; }

        public int ColumnAt { get; private set; }
    }
}
